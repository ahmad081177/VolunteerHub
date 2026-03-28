<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.Profile" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Profile</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">My Profile</h1>
            <p class="vh-page-desc">Manage your personal information and account security.</p>
        </div>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <div class="row g-4">
        <!-- Profile form -->
        <div class="col-lg-8">
            <div class="vh-card">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-person"></i> Personal Information</h2>
                </div>
                <div class="vh-card-body">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <div class="vh-form-group">
                                <label class="vh-label">First Name <span class="vh-required">*</span></label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="vh-input" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                                    CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" ValidationGroup="profile" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="vh-form-group">
                                <label class="vh-label">Last Name <span class="vh-required">*</span></label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="vh-input" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                                    CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" ValidationGroup="profile" />
                            </div>
                        </div>
                    </div>

                    <div class="vh-form-group">
                        <label class="vh-label">Email Address</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="vh-input" ReadOnly="true"
                            style="background:var(--vh-surface-alt); cursor:not-allowed;" />
                        <small class="text-muted">Email address cannot be changed.</small>
                    </div>

                    <div class="row g-3">
                        <div class="col-md-6">
                            <div class="vh-form-group">
                                <label class="vh-label">Phone</label>
                                <asp:TextBox ID="txtPhone" runat="server" CssClass="vh-input" placeholder="+1 555 000 0000" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="vh-form-group">
                                <label class="vh-label">Date of Birth</label>
                                <asp:TextBox ID="txtDob" runat="server" CssClass="vh-input" ReadOnly="true"
                                    style="background:var(--vh-surface-alt); cursor:not-allowed;" />
                            </div>
                        </div>
                    </div>

                    <div class="vh-form-group">
                        <label class="vh-label">Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="vh-input" placeholder="Your home address" />
                    </div>

                    <div class="vh-form-group">
                        <label class="vh-label">Profile Photo</label>
                        <div class="d-flex align-items-center gap-3 mb-2">
                            <div class="vh-avatar-lg" id="currentAvatarText" runat="server"></div>
                            <div id="currentAvatarImg" runat="server"></div>
                        </div>
                        <div class="vh-upload-zone">
                            <asp:FileUpload ID="fuPhoto" runat="server" CssClass="vh-file-input" accept=".jpg,.jpeg,.png,.gif" />
                            <div class="vh-upload-hint">
                                <i class="bi bi-cloud-upload"></i>
                                <span>Click to upload photo (JPG/PNG, max 2MB)</span>
                            </div>
                            <img id="imgPhotoPreview" class="vh-img-preview" src="#" alt="Preview" style="display:none;" />
                        </div>
                    </div>

                    <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes"
                        CssClass="btn vh-btn-primary" OnClick="btnSaveProfile_Click" ValidationGroup="profile" />
                </div>
            </div>
        </div>

        <!-- Change password + stats sidebar -->
        <div class="col-lg-4">
            <!-- Quick stats -->
            <div class="vh-card mb-4">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-graph-up-arrow"></i> My Stats</h2>
                </div>
                <div class="vh-card-body">
                    <div class="vh-info-list">
                        <div class="vh-info-item">
                            <span class="vh-info-label">Total Hours</span>
                            <span class="vh-info-value fw-bold" id="statHours" runat="server">0</span>
                        </div>
                        <div class="vh-info-item">
                            <span class="vh-info-label">Projects Joined</span>
                            <span class="vh-info-value fw-bold" id="statProjects" runat="server">0</span>
                        </div>
                        <div class="vh-info-item">
                            <span class="vh-info-label">Member Since</span>
                            <span class="vh-info-value" id="statJoined" runat="server"></span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Change password card -->
            <div class="vh-card">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-shield-lock"></i> Change Password</h2>
                </div>
                <div class="vh-card-body">
                    <div class="vh-form-group">
                        <label class="vh-label">Current Password</label>
                        <asp:TextBox ID="txtCurrentPwd" runat="server" CssClass="vh-input" TextMode="Password" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCurrentPwd"
                            CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" ValidationGroup="pwd" />
                    </div>
                    <div class="vh-form-group">
                        <label class="vh-label">New Password</label>
                        <asp:TextBox ID="txtNewPwd" runat="server" CssClass="vh-input" TextMode="Password" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNewPwd"
                            CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" ValidationGroup="pwd" />
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtNewPwd"
                            ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$"
                            CssClass="vh-field-error" ErrorMessage="Min 8 chars, uppercase, lowercase, digit, special." Display="Dynamic" ValidationGroup="pwd" />
                    </div>
                    <div class="vh-form-group">
                        <label class="vh-label">Confirm New Password</label>
                        <asp:TextBox ID="txtConfirmPwd" runat="server" CssClass="vh-input" TextMode="Password" />
                        <asp:CompareValidator runat="server"
                            ControlToValidate="txtConfirmPwd" ControlToCompare="txtNewPwd"
                            CssClass="vh-field-error" ErrorMessage="Passwords do not match." Display="Dynamic" ValidationGroup="pwd" />
                    </div>
                    <asp:Button ID="btnChangePwd" runat="server" Text="Update Password"
                        CssClass="btn vh-btn-outline w-100"
                        OnClick="btnChangePwd_Click" ValidationGroup="pwd" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>VH.initImagePreview('<%= fuPhoto.ClientID %>', 'imgPhotoPreview');</script>
</asp:Content>
