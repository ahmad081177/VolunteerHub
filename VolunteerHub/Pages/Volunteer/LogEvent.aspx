<%@ Page Title="Log Hours" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="LogEvent.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.LogEvent" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="vh-breadcrumb-item">My Hours</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active">Log Hours</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Log Volunteer Hours</h1>
            <p class="vh-page-desc">Record the hours you spent on a project.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <div class="vh-card" style="max-width:640px;">
        <div class="vh-card-body">
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <div class="vh-form-group">
                <label class="vh-label">Project <span class="vh-required">*</span></label>
                <asp:DropDownList ID="ddlProject" runat="server" CssClass="vh-input" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlProject"
                    CssClass="vh-field-error" ErrorMessage="Select a project." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Date <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtDate" runat="server" CssClass="vh-input" TextMode="Date" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDate"
                    CssClass="vh-field-error" ErrorMessage="Date is required." Display="Dynamic" />
            </div>

            <div class="row g-3">
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">Start Time</label>
                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="vh-input" TextMode="Time" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">End Time</label>
                        <asp:TextBox ID="txtEndTime" runat="server" CssClass="vh-input" TextMode="Time" />
                    </div>
                </div>
            </div>

            <div class="vh-form-group">
                <label class="vh-label" id="lblHours">Hours Logged <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtHours" runat="server" CssClass="vh-input" TextMode="Number"
                    placeholder="e.g. 2.5" step="0.5" />
                <small id="hoursHint" class="vh-form-hint" style="display:none;">
                    <i class="bi bi-calculator"></i> Auto-calculated from start/end times
                </small>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtHours"
                    CssClass="vh-field-error" ErrorMessage="Hours are required." Display="Dynamic" />
                <asp:RangeValidator runat="server" ControlToValidate="txtHours"
                    MinimumValue="0.1" MaximumValue="24" Type="Double"
                    CssClass="vh-field-error" ErrorMessage="Enter hours between 0.1 and 24." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Notes</label>
                <asp:TextBox ID="txtNotes" runat="server" CssClass="vh-input" TextMode="MultiLine"
                    Rows="3" placeholder="What did you do? (optional)" MaxLength="1000" />
            </div>

            <!-- Image upload (up to 5) -->
            <div class="vh-form-group">
                <label class="vh-label"><i class="bi bi-images me-1"></i>Photos <span class="text-muted fw-normal" style="font-size:12px;">(optional, up to 5 — JPG/PNG/GIF, max 2 MB each)</span></label>
                <input type="file" id="fuImages" name="fuImages" accept="image/jpeg,image/png,image/gif"
                       multiple class="vh-form-control" onchange="previewImages(this)" />
                <div id="imgPreviewArea" class="d-flex flex-wrap gap-2 mt-2"></div>
                <small class="vh-form-hint" id="imgCountHint"></small>
            </div>

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Save Hours" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// ── Hours auto-calculation ───────────────────────────────────────────────────
(function () {
    var st   = document.getElementById('<%= txtStartTime.ClientID %>');
    var et   = document.getElementById('<%= txtEndTime.ClientID %>');
    var hrs  = document.getElementById('<%= txtHours.ClientID %>');
    var hint = document.getElementById('hoursHint');
    if (!st || !et || !hrs) return;

    function calcHours() {
        var sv = st.value, ev = et.value;
        if (!sv || !ev) {
            hrs.removeAttribute('readonly');
            hrs.style.background = '';
            hint.style.display = 'none';
            return;
        }
        var sm = sv.split(':'), em = ev.split(':');
        var startMin = parseInt(sm[0]) * 60 + parseInt(sm[1]);
        var endMin   = parseInt(em[0]) * 60 + parseInt(em[1]);
        var diff = endMin - startMin;
        if (diff <= 0) {
            hrs.removeAttribute('readonly');
            hrs.style.background = '';
            hint.style.display = 'none';
            return;
        }
        var h = (diff / 60).toFixed(2).replace(/\.00$/, '').replace(/0$/, '');
        hrs.value = h;
        hrs.setAttribute('readonly', 'readonly');
        hrs.style.background = '#F0FDF4';
        hint.style.display = 'block';
    }

    st.addEventListener('change', calcHours);
    et.addEventListener('change', calcHours);
    st.addEventListener('input', calcHours);
    et.addEventListener('input', calcHours);
})();

// ── Image preview ────────────────────────────────────────────────────────────
function previewImages(input) {
    var area  = document.getElementById('imgPreviewArea');
    var chint = document.getElementById('imgCountHint');
    area.innerHTML = '';
    chint.textContent = '';

    var files = Array.from(input.files);
    if (files.length > 5) {
        chint.textContent = 'Only the first 5 images will be uploaded.';
        chint.style.color = '#92400E';
        files = files.slice(0, 5);
        // Replace the FileList with a DataTransfer to enforce the cap
        try {
            var dt = new DataTransfer();
            files.forEach(function(f){ dt.items.add(f); });
            input.files = dt.files;
        } catch(e) {} // DataTransfer not supported in all browsers — server will cap at 5 anyway
    }

    files.forEach(function (f) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var img = document.createElement('img');
            img.src = e.target.result;
            img.style.cssText = 'width:72px;height:72px;object-fit:cover;border-radius:8px;border:1px solid #E2E8F0;';
            area.appendChild(img);
        };
        reader.readAsDataURL(f);
    });

    if (files.length > 0)
        chint.textContent = files.length + ' image' + (files.length > 1 ? 's' : '') + ' selected.';
}
</script>
</asp:Content>
