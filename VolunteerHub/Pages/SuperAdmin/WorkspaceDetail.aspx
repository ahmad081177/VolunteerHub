<%@ Page Title="Workspace Detail" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="WorkspaceDetail.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.WorkspaceDetail" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Workspaces.aspx") %>" class="vh-breadcrumb-item">Workspaces</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active" id="breadcrumbName" runat="server">Detail</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title" id="pageTitle" runat="server">Workspace</h1>
            <p class="vh-page-desc" id="pageSubtitle" runat="server"></p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/SuperAdmin/Workspaces.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <!-- Stats -->
    <div class="vh-stats-grid mb-4">
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-people-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statVolunteers" runat="server">0</div>
                <div class="vh-stat-label">Volunteers</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-emerald">
            <div class="vh-stat-icon"><i class="bi bi-folder2-open"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statProjects" runat="server">0</div>
                <div class="vh-stat-label">Projects</div>
            </div>
        </div>
    </div>

    <!-- Admins -->
    <div class="vh-card mb-4">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-shield-check"></i> Administrators</h2>
            <a href='<%# "~/Pages/SuperAdmin/CreateAdmin.aspx?wsId=" + Request.QueryString["id"] %>' class="btn vh-btn-primary btn-sm">
                <i class="bi bi-plus-lg"></i> Add Admin
            </a>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No administrators yet.">
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <div class="vh-user-row">
                                <div class="vh-avatar-sm"><%# Eval("Initials") %></div>
                                <span><%# Eval("FullName") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsActive") ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted" %>'>
                                <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LastLoginAt" HeaderText="Last Login" DataFormatString="{0:MMM dd, yyyy}" NullDisplayText="Never" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Volunteers -->
    <div class="vh-card">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-people"></i> Volunteers</h2>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No volunteers yet.">
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <div class="vh-user-row">
                                <div class="vh-avatar-sm"><%# Eval("Initials") %></div>
                                <span><%# Eval("FullName") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsActive") ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted" %>'>
                                <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedAt" HeaderText="Joined" DataFormatString="{0:MMM dd, yyyy}" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
