<%@ Page Title="Edit Hours" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EditEvent.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.EditEvent" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="vh-breadcrumb-item">My Hours</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active">Edit</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Edit Volunteer Hours</h1>
            <p class="vh-page-desc">Correct the details of a previously logged event.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <div class="vh-card" style="max-width:640px;">
        <div class="vh-card-body">
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>
            <asp:HiddenField ID="hfId" runat="server" />

            <div class="vh-form-group">
                <label class="vh-label">Project</label>
                <p class="fw-semibold mb-0" id="lblProject" runat="server"></p>
                <small class="text-muted">Project cannot be changed after logging.</small>
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
                        <label class="vh-label">Start Time <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="vh-input" TextMode="Time" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartTime"
                            CssClass="vh-field-error" ErrorMessage="Start time is required." Display="Dynamic" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">End Time <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtEndTime" runat="server" CssClass="vh-input" TextMode="Time" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndTime"
                            CssClass="vh-field-error" ErrorMessage="End time is required." Display="Dynamic" />
                    </div>
                </div>
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Hours Logged <small class="text-muted fw-normal">(auto-calculated from times)</small></label>
                <asp:TextBox ID="txtHours" runat="server" CssClass="vh-input" ReadOnly="true"
                    style="background:#F0FDF4;cursor:not-allowed;" />
                <small class="vh-form-hint"><i class="bi bi-calculator"></i> Derived from start and end time — not editable.</small>
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Notes</label>
                <asp:TextBox ID="txtNotes" runat="server" CssClass="vh-input" TextMode="MultiLine"
                    Rows="3" placeholder="What did you do? (optional)" MaxLength="1000" />
            </div>

            <!-- Existing photos -->
            <div class="vh-form-group" id="existingPhotosSection" runat="server">
                <label class="vh-label"><i class="bi bi-images me-1"></i>Current Photos</label>
                <asp:HiddenField ID="hfDeleteImageIds" runat="server" Value="" />
                <div id="existingImgArea" class="d-flex flex-wrap gap-2 mt-1">
                    <asp:Repeater ID="rptExistingImages" runat="server">
                        <ItemTemplate>
                            <div class="position-relative" style="width:72px;">
                                <img src='<%# ResolveUrl((string)Container.DataItem) %>'
                                     style="width:72px;height:72px;object-fit:cover;border-radius:8px;border:1px solid #E2E8F0;" />
                                <button type="button" class="btn-close position-absolute"
                                    style="top:-6px;right:-6px;background:#EF4444;border-radius:50%;padding:3px;opacity:1;filter:invert(1);"
                                    title="Remove photo"
                                    onclick="removeExistingImage(this, '<%# Container.DataItem %>')"></button>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
                <small class="vh-form-hint text-muted">Click ✕ on a photo to mark it for removal when you save.</small>
            </div>

            <!-- Add new photos -->
            <div class="vh-form-group">
                <label class="vh-label"><i class="bi bi-plus-circle me-1"></i>Add More Photos <span class="text-muted fw-normal" style="font-size:12px;">(optional, up to 5 total — JPG/PNG/GIF, max 2 MB each)</span></label>
                <input type="file" id="fuImages" name="fuImages" accept="image/jpeg,image/png,image/gif"
                       multiple class="vh-form-control" onchange="previewImages(this)" />
                <div id="imgPreviewArea" class="d-flex flex-wrap gap-2 mt-2"></div>
                <small class="vh-form-hint" id="imgCountHint"></small>
            </div>

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
// ── Hours auto-calculation ───────────────────────────────────────────────────
(function () {
    var st  = document.getElementById('<%= txtStartTime.ClientID %>');
    var et  = document.getElementById('<%= txtEndTime.ClientID %>');
    var hrs = document.getElementById('<%= txtHours.ClientID %>');
    if (!st || !et || !hrs) return;

    function calcHours() {
        var sv = st.value, ev = et.value;
        if (!sv || !ev) { hrs.value = ''; return; }
        var sm = sv.split(':'), em = ev.split(':');
        var startMin = parseInt(sm[0]) * 60 + parseInt(sm[1]);
        var endMin   = parseInt(em[0]) * 60 + parseInt(em[1]);
        var diff = endMin - startMin;
        if (diff <= 0) { hrs.value = ''; return; }
        hrs.value = (diff / 60).toFixed(2).replace(/\.00$/, '').replace(/0$/, '');
    }

    st.addEventListener('change', calcHours);
    et.addEventListener('change', calcHours);
    st.addEventListener('input',  calcHours);
    et.addEventListener('input',  calcHours);
    // Run once on page load if times are already populated
    calcHours();
})();

// ── Remove existing photo ────────────────────────────────────────────────────
function removeExistingImage(btn, path) {
    // Hide the thumbnail immediately
    btn.parentElement.style.display = 'none';
    // Accumulate deleted paths in the hidden field (comma-separated)
    var hf = document.getElementById('<%= hfDeleteImageIds.ClientID %>');
    var existing = hf.value ? hf.value.split(',') : [];
    if (existing.indexOf(path) === -1) existing.push(path);
    hf.value = existing.join(',');
}

// ── Image preview ─────────────────────────────────────────────────────────────
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
        try {
            var dt = new DataTransfer();
            files.forEach(function(f){ dt.items.add(f); });
            input.files = dt.files;
        } catch(e) {}
    }

    files.forEach(function(file) {
        var reader = new FileReader();
        reader.onload = function(e) {
            var img = document.createElement('img');
            img.src = e.target.result;
            img.style.cssText = 'width:64px;height:64px;object-fit:cover;border-radius:8px;border:1px solid #E2E8F0;';
            area.appendChild(img);
        };
        reader.readAsDataURL(file);
    });
}
</script>
</asp:Content>
