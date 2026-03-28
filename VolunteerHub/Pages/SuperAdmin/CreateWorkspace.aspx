<%@ Page Title="Create Workspace" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="CreateWorkspace.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.CreateWorkspace" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Workspaces.aspx") %>" class="vh-breadcrumb-item">Workspaces</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active">Create</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Create Workspace</h1>
            <p class="vh-page-desc">Add a new school or organization workspace.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Workspaces.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <div class="vh-card" style="max-width:640px;">
        <div class="vh-card-body">
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <div class="vh-form-group">
                <label class="vh-label">Workspace Name <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtName" runat="server" CssClass="vh-input" placeholder="e.g. Lincoln High School" MaxLength="200" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName"
                    CssClass="vh-field-error" ErrorMessage="Name is required." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Join Code <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtCode" runat="server" CssClass="vh-input" placeholder="e.g. LINCOLN2024" MaxLength="20" style="text-transform:uppercase;" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCode"
                    CssClass="vh-field-error" ErrorMessage="Code is required." Display="Dynamic" />
                <small class="text-muted">Short unique code volunteers use to join this workspace.</small>
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Workspace Logo</label>
                <div class="vh-upload-zone">
                    <asp:FileUpload ID="fuLogo" runat="server" CssClass="vh-file-input" accept=".jpg,.jpeg,.png,.gif" />
                    <div class="vh-upload-hint">
                        <i class="bi bi-cloud-upload"></i>
                        <span>Click to upload logo (JPG/PNG, max 2MB)</span>
                    </div>
                    <img id="imgLogoPreview" class="vh-img-preview" src="#" alt="Logo preview" style="display:none;" />
                </div>
            </div>

            <div class="vh-form-group">
                <label class="vh-check-label">
                    <asp:CheckBox ID="chkActive" runat="server" Checked="true" />
                    <span>Active (visible to volunteers)</span>
                </label>
            </div>

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Create Workspace" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Workspaces.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>VH.initImagePreview('<%= fuLogo.ClientID %>', 'imgLogoPreview');</script>
</asp:Content>
