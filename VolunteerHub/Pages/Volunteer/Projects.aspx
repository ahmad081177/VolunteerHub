<%@ Page Title="Browse Projects" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.Projects" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Browse Projects</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Browse Projects</h1>
            <p class="vh-page-desc">Discover and join volunteering projects in your workspace.</p>
        </div>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <asp:Repeater ID="rptProjects" runat="server" OnItemCommand="rptProjects_ItemCommand">
        <HeaderTemplate>
            <div class="row g-3">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="col-md-6 col-xl-4">
                <div class="vh-project-card">
                    <div class="vh-project-card-header">
                        <span class='<%# "vh-badge " + Eval("StatusBadgeClass") %>'><%# Eval("Status") %></span>
                        <span class="text-muted small"><%# Eval("Location") ?? "" %></span>
                    </div>
                    <h3 class="vh-project-card-title"><%# Eval("Title") %></h3>
                    <p class="vh-project-card-desc">
                        <%# !string.IsNullOrEmpty(Eval("Description")?.ToString())
                            ? (Eval("Description").ToString().Length > 100
                               ? Eval("Description").ToString().Substring(0, 100) + "…"
                               : Eval("Description"))
                            : "No description provided." %>
                    </p>
                    <div class="vh-project-card-meta">
                        <span><i class="bi bi-calendar"></i> <%# ((DateTime)Eval("StartDate")).ToString("MMM dd") %> – <%# ((DateTime)Eval("EndDate")).ToString("MMM dd, yyyy") %></span>
                        <%# Eval("HoursRequired") != null ? $"<span><i class='bi bi-clock'></i> {Eval(\"HoursRequired\")} hrs required</span>" : "" %>
                    </div>
                    <div class="vh-project-card-footer">
                        <%# (bool)Eval("IsEnrolled")
                            ? "<span class=\"vh-badge vh-badge-success\"><i class='bi bi-check-circle'></i> Enrolled</span>"
                            : (string)Eval("Status") == "Ended"
                              ? "<span class=\"vh-badge vh-badge-muted\">Project Ended</span>"
                              : "" %>
                        <asp:LinkButton ID="btnJoin" runat="server"
                            CommandName="Join"
                            CommandArgument='<%# Eval("Id") %>'
                            CssClass="btn vh-btn-primary btn-sm"
                            Visible='<%# !(bool)Eval("IsEnrolled") && (string)Eval("Status") != "Ended" %>'>
                            <i class="bi bi-plus-circle"></i> Join
                        </asp:LinkButton>
                        <a href='<%# ResolveUrl("~/Pages/Volunteer/ProjectDetail.aspx?id=" + Eval("Id")) %>'
                           class="btn vh-btn-outline btn-sm">
                            <i class="bi bi-eye"></i> Details
                        </a>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
            <%# rptProjects.Items.Count == 0 ? "<div class='vh-empty-state'><i class='bi bi-folder2-open'></i><p>No projects available in your workspace yet.</p></div>" : "" %>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
