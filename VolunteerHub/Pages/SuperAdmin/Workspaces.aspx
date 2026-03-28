<%@ Page Title="Workspaces" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Workspaces.aspx.cs" Inherits="VolunteerHub.Pages.SuperAdmin.Workspaces" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Workspaces</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Workspaces</h1>
            <p class="vh-page-desc">Manage all school workspaces.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/SuperAdmin/CreateWorkspace.aspx") %>" class="btn vh-btn-primary">
            <i class="bi bi-plus-lg"></i> New Workspace
        </a>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <div class="vh-card">
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvWorkspaces" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No workspaces created yet.">
                <Columns>
                    <asp:BoundField DataField="Name"     HeaderText="Workspace Name" />
                    <asp:BoundField DataField="Code"     HeaderText="Code" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsActive") ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted" %>'>
                                <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedAt" HeaderText="Created" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/SuperAdmin/WorkspaceDetail.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-outline btn-sm">
                                <i class="bi bi-eye"></i>
                            </a>
                            <a href='<%# ResolveUrl("~/Pages/SuperAdmin/EditWorkspace.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-ghost btn-sm">
                                <i class="bi bi-pencil"></i>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
