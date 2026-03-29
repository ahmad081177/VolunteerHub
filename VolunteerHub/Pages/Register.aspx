<%@ Page Title="Create Account" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="VolunteerHub.Pages.Register" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-auth-page" style="padding: 32px 16px;">
        <div class="vh-auth-card wide">

            <!-- Header -->
            <div class="vh-auth-card-header">
                <div class="vh-auth-logo">
                    <i class="bi bi-heart-fill" style="color:white;font-size:24px;"></i>
                </div>
                <h1 class="vh-auth-title">Join VolunteerHub</h1>
                <p class="vh-auth-subtitle">Create your volunteer account</p>
            </div>

            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <!-- Form body -->
            <div class="vh-auth-body">

                <!-- Row 1: First / Last Name -->
                <div class="row g-3 mb-0">
                    <div class="col-sm-6">
                        <div class="vh-form-group">
                            <label class="vh-form-label">First Name</label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="vh-form-control" placeholder="Jane" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFirstName"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="vh-form-group">
                            <label class="vh-form-label">Last Name</label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="vh-form-control" placeholder="Smith" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLastName"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <!-- Email -->
                <div class="vh-form-group">
                    <label class="vh-form-label">
                        <i class="bi bi-envelope me-1"></i>Email Address
                    </label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="vh-form-control" TextMode="Email"
                        placeholder="you@school.edu" autocomplete="email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail"
                        CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                    <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                        ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                        CssClass="vh-field-error" ErrorMessage="Invalid email." Display="Dynamic" />
                </div>

                <!-- Row 2: Password / Confirm Password -->
                <div class="row g-3 mb-0">
                    <div class="col-sm-6">
                        <div class="vh-form-group">
                            <label class="vh-form-label">
                                <i class="bi bi-lock me-1"></i>Password
                            </label>
                            <div class="vh-input-group">
                                <asp:TextBox ID="txtPassword" runat="server" CssClass="vh-form-control" TextMode="Password"
                                    placeholder="Min. 8 characters" autocomplete="new-password" />
                                <button type="button" class="vh-input-addon vh-toggle-password" tabindex="-1"
                                        aria-label="Toggle password visibility">
                                    <i class="bi bi-eye"></i>
                                </button>
                            </div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPassword"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtPassword"
                                ValidationExpression="^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$"
                                CssClass="vh-field-error"
                                ErrorMessage="Min 8 chars, uppercase, lowercase, digit &amp; special." Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="vh-form-group">
                            <label class="vh-form-label">
                                <i class="bi bi-lock-fill me-1"></i>Confirm Password
                            </label>
                            <asp:TextBox ID="txtConfirm" runat="server" CssClass="vh-form-control" TextMode="Password"
                                placeholder="Repeat password" autocomplete="new-password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtConfirm"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                            <asp:CompareValidator runat="server"
                                ControlToValidate="txtConfirm" ControlToCompare="txtPassword"
                                CssClass="vh-field-error" ErrorMessage="Passwords do not match." Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <!-- Row 3: Gender / Date of Birth -->
                <div class="row g-3 mb-0">
                    <div class="col-sm-6">
                        <div class="vh-form-group">
                            <label class="vh-form-label">
                                <i class="bi bi-gender-ambiguous me-1"></i>Gender
                            </label>
                            <asp:DropDownList ID="ddlGender" runat="server" CssClass="vh-form-control">
                                <asp:ListItem Value="" Text="-- Select --" />
                                <asp:ListItem Value="true"  Text="Male" />
                                <asp:ListItem Value="false" Text="Female" />
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlGender"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                    <div class="col-sm-6">
                        <div class="vh-form-group">
                            <label class="vh-form-label">
                                <i class="bi bi-calendar me-1"></i>Date of Birth
                            </label>
                            <asp:TextBox ID="txtDob" runat="server" CssClass="vh-form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDob"
                                CssClass="vh-field-error" ErrorMessage="Required." Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <!-- Workspace -->
                <div class="vh-form-group">
                    <label class="vh-form-label">
                        <i class="bi bi-building me-1"></i>Workspace
                    </label>
                    <input type="text" id="wsFilterInput" class="vh-form-control mb-1"
                           placeholder="&#x1F50D; Type to filter workspaces…" autocomplete="off" />
                    <asp:DropDownList ID="ddlWorkspaceCode" runat="server" CssClass="vh-form-control">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlWorkspaceCode"
                        InitialValue="" CssClass="vh-field-error" ErrorMessage="Please select a workspace." Display="Dynamic" />
                    <span class="vh-form-hint mt-1"><i class="bi bi-info-circle"></i> Select the workspace your school is registered under.</span>
                </div>

                <asp:Button ID="btnRegister" runat="server" Text="Create Account"
                    CssClass="vh-btn vh-btn-primary w-100 mt-2"
                    OnClick="btnRegister_Click" />
            </div>

            <div class="vh-auth-footer">
                <p class="mb-0">Already have an account?
                    <a href="<%= ResolveUrl("~/Pages/Login.aspx") %>">Sign in</a>
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

// Searchable workspace dropdown
(function () {
    var input = document.getElementById('wsFilterInput');
    var sel   = document.getElementById('<%= ddlWorkspaceCode.ClientID %>');
    if (!input || !sel) return;

    var opts = Array.from(sel.options);

    input.addEventListener('input', function () {
        var q = this.value.trim().toLowerCase();
        opts.forEach(function (o) {
            o.hidden = q.length > 0 && o.value !== '' && !o.text.toLowerCase().includes(q);
        });
        // Auto-select when exactly one non-placeholder option is visible
        var visible = opts.filter(function (o) { return !o.hidden && o.value !== ''; });
        if (visible.length === 1) sel.value = visible[0].value;
        else if (q.length === 0)  sel.value = '';
    });

    // Clear filter when user picks directly from the list
    sel.addEventListener('change', function () {
        input.value = '';
        opts.forEach(function (o) { o.hidden = false; });
    });
})();
</script>
</asp:Content>