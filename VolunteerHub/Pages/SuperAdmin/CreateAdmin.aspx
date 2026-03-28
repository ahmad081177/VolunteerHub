<%@ Page Title="Create Administrator" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="CreateAdmin.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.CreateAdmin" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Admins.aspx") %>" class="vh-breadcrumb-item">Administrators</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active">Create</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Create Administrator</h1>
            <p class="vh-page-desc">Add a new workspace administrator account.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Admins.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <div class="vh-card" style="max-width:640px;">
        <div class="vh-card-body">
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <div class="row g-3">
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">First Name <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="vh-input" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                            CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">Last Name <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="vh-input" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                            CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    </div>
                </div>
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Email Address <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="vh-input" TextMode="Email" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                    CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Temporary Password <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtPassword" runat="server" CssClass="vh-input" TextMode="Password" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                    CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPassword"
                    ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$"
                    CssClass="vh-field-error"
                    ErrorMessage="Min 8 chars, uppercase, lowercase, digit, special char." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Workspace <span class="vh-required">*</span></label>
                <asp:DropDownList ID="ddlWorkspace" runat="server" CssClass="vh-input" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlWorkspace"
                    CssClass="vh-field-error" ErrorMessage="Select a workspace." Display="Dynamic" />
            </div>

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Create Admin" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Admins.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>
