<%@ Page Title="Create Account" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="VolunteerHub.Pages.Register" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-auth-container">
        <div class="vh-auth-card vh-auth-card-wide">
            <!-- Logo -->
            <div class="vh-auth-logo">
                <div class="vh-auth-logo-icon">
                    <i class="bi bi-heart-fill"></i>
                </div>
                <h1 class="vh-auth-title">Join VolunteerHub</h1>
                <p class="vh-auth-subtitle">Create your volunteer account</p>
            </div>

            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <div class="vh-auth-form">
                <div class="row g-3">
                    <div class="col-md-6">
                        <div class="vh-form-group">
                            <label class="vh-label">First Name</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="vh-input" placeholder="Jane" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="vh-form-group">
                            <label class="vh-label">Last Name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="vh-input" placeholder="Smith" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <div class="vh-form-group">
                    <label class="vh-label"><i class="bi bi-envelope"></i> Email Address</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="vh-input" TextMode="Email"
                        placeholder="you@school.edu" autocomplete="email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                        CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        CssClass="vh-field-error" ErrorMessage="Invalid email." Display="Dynamic" />
                </div>

                <div class="row g-3">
                    <div class="col-md-6">
                        <div class="vh-form-group">
                            <label class="vh-label"><i class="bi bi-lock"></i> Password</label>
                            <div class="vh-input-group">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="vh-input" TextMode="Password"
                                    placeholder="Min. 8 characters" autocomplete="new-password" />
                                <button type="button" class="vh-input-addon vh-toggle-password" tabindex="-1">
                                    <i class="bi bi-eye"></i>
                                </button>
                            </div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPassword"
                                ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$"
                                CssClass="vh-field-error"
                                ErrorMessage="Min 8 chars, uppercase, lowercase, digit, special." Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="vh-form-group">
                            <label class="vh-label"><i class="bi bi-lock-fill"></i> Confirm Password</label>
                            <asp:TextBox ID="txtConfirm" runat="server" CssClass="vh-input" TextMode="Password"
                                placeholder="Repeat password" autocomplete="new-password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConfirm"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                            <asp:CompareValidator runat="server"
                                ControlToValidate="txtConfirm" ControlToCompare="txtPassword"
                                CssClass="vh-field-error" ErrorMessage="Passwords do not match." Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <div class="row g-3">
                    <div class="col-md-6">
                        <div class="vh-form-group">
                            <label class="vh-label"><i class="bi bi-gender-ambiguous"></i> Gender</label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="vh-input">
                                <asp:ListItem Value="" Text="-- Select --" />
                                <asp:ListItem Value="true"  Text="Male" />
                                <asp:ListItem Value="false" Text="Female" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlGender"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="vh-form-group">
                            <label class="vh-label"><i class="bi bi-calendar"></i> Date of Birth</label>
                            <asp:TextBox ID="txtDob" runat="server" CssClass="vh-input" TextMode="Date" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDob"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <div class="vh-form-group">
                    <label class="vh-label"><i class="bi bi-building"></i> Workspace Code</label>
                    <asp:TextBox ID="txtWorkspaceCode" runat="server" CssClass="vh-input"
                        placeholder="Enter your school's workspace code" MaxLength="20" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtWorkspaceCode"
                        CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    <small class="text-muted">Ask your school administrator for the workspace code.</small>
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="Create Account" CssClass="btn vh-btn-primary w-100 mt-3"
                    OnClick="btnRegister_Click" />
            </div>

            <div class="vh-auth-footer">
                <p>Already have an account? <a href="<%= ResolveUrl("~/Pages/Login.aspx") %>">Sign in</a></p>
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
