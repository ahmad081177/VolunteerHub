using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using VolunteerHub.DAL;

namespace VolunteerHub.Helpers
{
    /// <summary>
    /// Streams an Excel-compatible HTML table (.xls) to the browser.
    /// Requires an active Admin session. Enforces workspace isolation on all queries.
    ///
    /// Routes (query-string):
    ///   ?type=project&amp;id=X      — volunteers + hours for one project
    ///   ?type=volunteer&amp;id=X   — all events for one volunteer
    ///   ?type=projects             — all projects in the workspace
    ///   ?type=volunteers           — all volunteers in the workspace
    /// </summary>
    public class ExportHandler : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            // ── Auth guard ────────────────────────────────────────────────────────
            var session = context.Session;
            var role    = session["Role"] as string;
            if (role != "Admin")
            {
                context.Response.StatusCode = 403;
                context.Response.End();
                return;
            }

            int wsId = (session["WorkspaceId"] as int?) ?? 0;
            string type = (context.Request.QueryString["type"] ?? "").ToLowerInvariant();

            switch (type)
            {
                case "project":    ExportProject(context, wsId);    break;
                case "volunteer":  ExportVolunteer(context, wsId);  break;
                case "projects":   ExportAllProjects(context, wsId); break;
                case "volunteers": ExportAllVolunteers(context, wsId); break;
                default:
                    context.Response.StatusCode = 400;
                    context.Response.End();
                    break;
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Per-project: volunteers enrolled + hours logged
        // ─────────────────────────────────────────────────────────────────────────
        private static void ExportProject(HttpContext ctx, int wsId)
        {
            int id;
            if (!int.TryParse(ctx.Request.QueryString["id"], out id))
            { ctx.Response.StatusCode = 400; ctx.Response.End(); return; }

            var p = ProjectDAL.GetById(id);
            if (p == null || p.WorkspaceId != wsId)
            { ctx.Response.StatusCode = 403; ctx.Response.End(); return; }

            decimal totalHrs    = EventDAL.GetTotalHoursByProject(id);
            var enrollments     = VolunteerProjectDAL.GetByProject(id);
            var enrolledUserIds = new HashSet<int>(enrollments.ConvertAll(e => e.UserId));
            var participantIds  = EventDAL.GetDistinctUserIdsByProject(id);

            var allUserIds = new List<int>(enrolledUserIds);
            foreach (var uid in participantIds)
                if (!enrolledUserIds.Contains(uid)) allUserIds.Add(uid);

            var sb = StartTable();
            Row(sb, 4, $"{Enc(p.Title)}", "#4F46E5", "#ffffff", "14pt", bold: true);
            Row(sb, 4,
                $"Location: {Enc(p.Location ?? "—")}  •  " +
                $"{p.StartDate:MMM dd, yyyy} – {p.EndDate:MMM dd, yyyy}  •  " +
                $"Total Hours: {totalHrs:0.#}",
                "#F8FAFC", "#475569");
            EmptyRow(sb, 4);
            HeaderRow(sb, new[] { "Volunteer", "Email", "Hours Logged", "Enrolled Date" });

            foreach (var userId in allUserIds)
            {
                var user = UserDAL.GetById(userId);
                if (user == null) continue;

                decimal hrs = 0;
                var evts = EventDAL.GetByUserAndProject(userId, id);
                foreach (var ev in evts) hrs += ev.HoursLogged;

                var enrollment = enrollments.Find(e => e.UserId == userId);
                DateTime enrolledAt = enrollment != null
                    ? enrollment.EnrolledAt
                    : evts.Select(e => e.EventDate).OrderBy(d => d).FirstOrDefault();

                DataRow(sb, new[]
                {
                    Enc(user.FullName),
                    Enc(user.Email),
                    hrs.ToString("0.#"),
                    enrolledAt.ToString("MMM dd, yyyy")
                });
            }

            sb.AppendLine("</table>");
            SendExcel(ctx, sb.ToString(), $"Project-{SafeName(p.Title)}");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Per-volunteer: all events across all projects
        // ─────────────────────────────────────────────────────────────────────────
        private static void ExportVolunteer(HttpContext ctx, int wsId)
        {
            int id;
            if (!int.TryParse(ctx.Request.QueryString["id"], out id))
            { ctx.Response.StatusCode = 400; ctx.Response.End(); return; }

            var user = UserDAL.GetById(id);
            if (user == null || user.WorkspaceId != wsId || user.Role != "Volunteer")
            { ctx.Response.StatusCode = 403; ctx.Response.End(); return; }

            var events   = EventDAL.GetByUser(id);
            decimal total = EventDAL.GetTotalHoursByUser(id);

            var sb = StartTable();
            Row(sb, 5, Enc(user.FullName), "#4F46E5", "#ffffff", "14pt", bold: true);
            Row(sb, 5,
                $"Email: {Enc(user.Email)}  •  Total Hours: {total:0.#}  •  Joined: {user.CreatedAt:MMM dd, yyyy}",
                "#F8FAFC", "#475569");
            EmptyRow(sb, 5);
            HeaderRow(sb, new[] { "Project", "Date", "Duration", "Hours", "Notes" });

            foreach (var ev in events)
            {
                DataRow(sb, new[]
                {
                    Enc(ev.ProjectTitle ?? "—"),
                    ev.EventDate.ToString("MMM dd, yyyy"),
                    Enc(ev.DurationDisplay ?? "—"),
                    ev.HoursLogged.ToString("0.#"),
                    Enc(ev.Notes ?? "")
                });
            }

            sb.AppendLine("</table>");
            SendExcel(ctx, sb.ToString(), $"Volunteer-{SafeName(user.FullName)}");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  All projects in workspace
        // ─────────────────────────────────────────────────────────────────────────
        private static void ExportAllProjects(HttpContext ctx, int wsId)
        {
            var projects = ProjectDAL.GetByWorkspace(wsId);

            var sb = StartTable();
            Row(sb, 6, "All Projects", "#4F46E5", "#ffffff", "14pt", bold: true);
            EmptyRow(sb, 6);
            HeaderRow(sb, new[] { "Project Title", "Status", "Location", "Start", "End", "Total Hours" });

            foreach (var p in projects)
            {
                decimal hrs = EventDAL.GetTotalHoursByProject(p.Id);
                DataRow(sb, new[]
                {
                    Enc(p.Title),
                    Enc(p.Status),
                    Enc(p.Location ?? "—"),
                    p.StartDate.ToString("MMM dd, yyyy"),
                    p.EndDate.ToString("MMM dd, yyyy"),
                    hrs.ToString("0.#")
                });
            }

            sb.AppendLine("</table>");
            SendExcel(ctx, sb.ToString(), "All-Projects");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  All volunteers in workspace
        // ─────────────────────────────────────────────────────────────────────────
        private static void ExportAllVolunteers(HttpContext ctx, int wsId)
        {
            var volunteers = UserDAL.GetAllByWorkspace(wsId);

            var sb = StartTable();
            Row(sb, 5, "All Volunteers", "#4F46E5", "#ffffff", "14pt", bold: true);
            EmptyRow(sb, 5);
            HeaderRow(sb, new[] { "Name", "Email", "Status", "Joined", "Total Hours" });

            foreach (var v in volunteers)
            {
                decimal hrs = EventDAL.GetTotalHoursByUser(v.Id);
                DataRow(sb, new[]
                {
                    Enc(v.FullName),
                    Enc(v.Email),
                    v.IsActive ? "Active" : "Inactive",
                    v.CreatedAt.ToString("MMM dd, yyyy"),
                    hrs.ToString("0.#")
                });
            }

            sb.AppendLine("</table>");
            SendExcel(ctx, sb.ToString(), "All-Volunteers");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  HTML table helpers
        // ─────────────────────────────────────────────────────────────────────────
        private static StringBuilder StartTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table border=\"1\" cellpadding=\"6\" cellspacing=\"0\" style=\"font-family:Calibri,Arial;font-size:11pt;border-collapse:collapse;\">");
            return sb;
        }

        private static void Row(StringBuilder sb, int cols, string content,
            string bgColor = "#ffffff", string color = "#0F172A",
            string fontSize = "11pt", bool bold = false)
        {
            string w = bold ? "bold" : "normal";
            sb.AppendLine($"<tr><td colspan=\"{cols}\" style=\"background:{bgColor};color:{color};font-size:{fontSize};font-weight:{w};padding:8px 6px;\">{content}</td></tr>");
        }

        private static void EmptyRow(StringBuilder sb, int cols)
            => sb.AppendLine($"<tr><td colspan=\"{cols}\" style=\"padding:2px;\"></td></tr>");

        private static void HeaderRow(StringBuilder sb, string[] headers)
        {
            sb.AppendLine("<tr style=\"background:#E8EAF6;font-weight:bold;\">");
            foreach (var h in headers)
                sb.AppendLine($"<td style=\"padding:6px 8px;\">{Enc(h)}</td>");
            sb.AppendLine("</tr>");
        }

        private static void DataRow(StringBuilder sb, string[] cells)
        {
            sb.AppendLine("<tr>");
            foreach (var c in cells)
                sb.AppendLine($"<td style=\"padding:5px 8px;\">{c}</td>");
            sb.AppendLine("</tr>");
        }

        // ─────────────────────────────────────────────────────────────────────────
        //  Response helpers
        // ─────────────────────────────────────────────────────────────────────────
        private static void SendExcel(HttpContext ctx, string tableHtml, string fileBaseName)
        {
            // Wrap in a complete HTML document so Excel recognises UTF-8 encoding
            string doc = "<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body>" +
                         tableHtml + "</body></html>";

            // Prepend UTF-8 BOM so Excel opens the file with the correct code page
            byte[] bom  = Encoding.UTF8.GetPreamble();
            byte[] body = Encoding.UTF8.GetBytes(doc);
            byte[] all  = new byte[bom.Length + body.Length];
            Buffer.BlockCopy(bom,  0, all, 0,          bom.Length);
            Buffer.BlockCopy(body, 0, all, bom.Length, body.Length);

            ctx.Response.Clear();
            ctx.Response.ContentType = "application/vnd.ms-excel";
            ctx.Response.AddHeader("Content-Disposition",
                $"attachment; filename=\"{fileBaseName}.xls\"");
            ctx.Response.BinaryWrite(all);
            ctx.Response.End();
        }

        private static string Enc(string s)
            => string.IsNullOrEmpty(s) ? "" : HttpUtility.HtmlEncode(s);

        private static string SafeName(string s)
        {
            if (string.IsNullOrEmpty(s)) return "export";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                s = s.Replace(c.ToString(), "");
            return s.Length > 40 ? s.Substring(0, 40) : s;
        }
    }
}
