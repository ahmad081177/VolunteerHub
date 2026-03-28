<%@ Page Title="Super Admin Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.Dashboard" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Dashboard</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">System Overview</h1>
            <p class="vh-page-desc">Monitor all workspaces and system health.</p>
        </div>
    </div>

    <!-- Stats Strip -->
    <div class="vh-stats-grid">
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-buildings-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statWorkspaces" runat="server">0</div>
                <div class="vh-stat-label">Active Workspaces</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-shield-check-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statAdmins" runat="server">0</div>
                <div class="vh-stat-label">Administrators</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-emerald">
            <div class="vh-stat-icon"><i class="bi bi-people-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statVolunteers" runat="server">0</div>
                <div class="vh-stat-label">Total Volunteers</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-amber">
            <div class="vh-stat-icon"><i class="bi bi-folder2-open"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statProjects" runat="server">0</div>
                <div class="vh-stat-label">Total Projects</div>
            </div>
        </div>
    </div>

    <!-- Workspaces Table -->
    <div class="vh-card mt-4">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-buildings"></i> Workspaces</h2>
            <a href="<%= ResolveUrl("~/Pages/SuperAdmin/CreateWorkspace.aspx") %>" class="btn vh-btn-primary btn-sm">
                <i class="bi bi-plus-lg"></i> New Workspace
            </a>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvWorkspaces" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No workspaces yet."
                OnRowCommand="gvWorkspaces_RowCommand">
                <Columns>
                    <asp:BoundField  DataField="Name"     HeaderText="Name" />
                    <asp:BoundField  DataField="Code"     HeaderText="Code" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsActive") ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted" %>'>
                                <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField  DataField="CreatedAt" HeaderText="Created" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/SuperAdmin/EditWorkspace.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-ghost btn-sm">
                                <i class="bi bi-pencil"></i> Edit
                            </a>
                            <a href='<%# ResolveUrl("~/Pages/SuperAdmin/WorkspaceDetail.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-outline btn-sm">
                                <i class="bi bi-eye"></i> View
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
