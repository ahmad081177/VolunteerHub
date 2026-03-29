<%@ Page Title="Administrators" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Admins.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.Admins" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Administrators</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Administrators</h1>
            <p class="vh-page-desc">Manage workspace administrators across all workspaces.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/SuperAdmin/CreateAdmin.aspx") %>" class="btn vh-btn-primary">
            <i class="bi bi-plus-lg"></i> New Admin
        </a>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <div class="vh-card">
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvAdmins" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No administrators found."
                OnRowCommand="gvAdmins_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <div class="vh-user-row">
                                <div class="vh-avatar-sm"><%# Eval("Initials") %></div>
                                <div>
                                    <div class="fw-semibold"><%# Eval("FullName") %></div>
                                    <div class="text-muted small"><%# Eval("Email") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="WorkspaceName" HeaderText="Workspace" NullDisplayText="—" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsActive") ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted" %>'>
                                <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="LastLoginAt" HeaderText="Last Login" DataFormatString="{0:MMM dd, yyyy}" NullDisplayText="Never" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <div class="d-flex gap-1">
                                <a href='<%# ResolveUrl("~/Pages/SuperAdmin/EditAdmin.aspx") + "?id=" + Eval("Id") %>'
                                   class="btn btn-sm vh-btn-outline" title="Edit">
                                    <i class="bi bi-pencil"></i>
                                </a>
                                <asp:HyperLink ID="lnkWorkspace" runat="server"
                                    NavigateUrl='<%# Eval("WorkspaceId") != null ? ResolveUrl("~/Pages/SuperAdmin/WorkspaceDetail.aspx") + "?id=" + Eval("WorkspaceId") : "#" %>'
                                    Visible='<%# Eval("WorkspaceId") != null %>'
                                    CssClass="btn btn-sm vh-btn-outline" title="View Workspace">
                                    <i class="bi bi-building"></i>
                                </asp:HyperLink>
                                <asp:LinkButton CommandName="Toggle" CommandArgument='<%# Eval("Id") + "," + Eval("IsActive") %>'
                                    CssClass='<%# "btn btn-sm " + ((bool)Eval("IsActive") ? "vh-btn-danger" : "vh-btn-outline") %>'
                                    runat="server" OnClientClick="return confirm('Toggle admin status?')">
                                    <%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
