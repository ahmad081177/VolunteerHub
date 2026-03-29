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
                <label class="vh-label">Hours Logged <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtHours" runat="server" CssClass="vh-input" TextMode="Number" placeholder="e.g. 2.5" />
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

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>
