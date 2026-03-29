<%@ Page Title="Sign In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="VolunteerHub.Pages.Login" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-auth-page">
        <div class="vh-auth-card">

            <!-- Header: logo + title + subtitle -->
            <div class="vh-auth-card-header">
                <div class="vh-auth-logo">
                    <i class="bi bi-heart-fill" style="color:white;font-size:24px;"></i>
                </div>
                <h1 class="vh-auth-title">VolunteerHub</h1>
                <p class="vh-auth-subtitle">Sign in to your account</p>
            </div>

            <!-- Alert / error block -->
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <!-- Form body -->
            <div class="vh-auth-body">

                <div class="vh-form-group">
                    <label class="vh-form-label" for="txtEmail">
                        <i class="bi bi-envelope me-1"></i>Email address
                    </label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="vh-form-control" TextMode="Email"
                        placeholder="you@example.com" autocomplete="email" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                        ControlToValidate="txtEmail" CssClass="vh-field-error"
                        ErrorMessage="Email is required." Display="Dynamic" />
                </div>

                <div class="vh-form-group">
                    <label class="vh-form-label" for="txtPassword">
                        <i class="bi bi-lock me-1"></i>Password
                    </label>
                    <div class="vh-input-group">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="vh-form-control" TextMode="Password"
                            placeholder="Enter your password" autocomplete="current-password" />
                        <button type="button" class="vh-input-addon vh-toggle-password" tabindex="-1"
                                aria-label="Toggle password visibility">
                            <i class="bi bi-eye"></i>
                        </button>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server"
                        ControlToValidate="txtPassword" CssClass="vh-field-error"
                        ErrorMessage="Password is required." Display="Dynamic" />
                </div>

                <div class="mb-3">
                    <label class="vh-check-label">
                        <asp:CheckBox ID="chkRemember" runat="server" />
                        <span>Remember me for 30 days</span>
                    </label>
                </div>

                <asp:Button ID="btnLogin" runat="server" Text="Sign In"
                    CssClass="vh-btn vh-btn-primary w-100 mt-2"
                    OnClick="btnLogin_Click" />
            </div>

            <!-- Footer -->
            <div class="vh-auth-footer">
                <p class="mb-0">Don&rsquo;t have an account?
                    <a href="<%= ResolveUrl("~/Pages/Register.aspx") %>">Create one</a>
                </p>
            </div>

        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
<script>
document.querySelector('.vh-toggle-password')?.addEventListener('click', function () {
    var pwd = document.getElementById('<%= txtPassword.ClientID %>');
    var icon = this.querySelector('i');
    if (pwd.type === 'password') { pwd.type = 'text';     icon.className = 'bi bi-eye-slash'; }
    else                         { pwd.type = 'password'; icon.className = 'bi bi-eye'; }
});
</script>
</asp:Content>
