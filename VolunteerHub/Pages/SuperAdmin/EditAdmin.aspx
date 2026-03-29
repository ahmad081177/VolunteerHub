<%@ Page Title="Edit Administrator" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EditAdmin.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.EditAdmin" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Admins.aspx") %>" class="vh-breadcrumb-item">Administrators</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active">Edit</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Edit Administrator</h1>
            <p class="vh-page-desc">Update workspace administrator account details.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Admins.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <div class="vh-card" style="max-width:640px;">
        <div class="vh-card-body">
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>
            <asp:HiddenField ID="hfId" runat="server" />

            <div class="row g-3">
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">First Name <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="vh-input" MaxLength="100" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                            CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">Last Name <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="vh-input" MaxLength="100" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                            CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    </div>
                </div>
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Email Address <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="vh-input" TextMode="Email" MaxLength="255" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                    CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Workspace <span class="vh-required">*</span></label>
                <asp:DropDownList ID="ddlWorkspace" runat="server" CssClass="vh-input" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlWorkspace"
                    InitialValue="" CssClass="vh-field-error" ErrorMessage="Select a workspace." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-check-label">
                    <asp:CheckBox ID="chkActive" runat="server" />
                    <span>Active</span>
                </label>
            </div>

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Admins.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>
