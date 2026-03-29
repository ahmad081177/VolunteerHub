<%@ Page Title="Projects" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Projects.aspx.cs" Inherits="VolunteerHub.Pages.Admin.Projects" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Projects</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Projects</h1>
            <p class="vh-page-desc">Manage all volunteering projects in your workspace.</p>
        </div>
        <div class="d-flex gap-2">
            <a href="<%= ResolveUrl("~/Helpers/ExportHandler.ashx?type=projects") %>" class="btn vh-btn-outline">
                <i class="bi bi-file-earmark-excel"></i> Export Excel
            </a>
            <a href="<%= ResolveUrl("~/Pages/Admin/CreateProject.aspx") %>" class="btn vh-btn-primary">
                <i class="bi bi-plus-lg"></i> New Project
            </a>
        </div>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <div class="vh-card">
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No projects yet. Create your first project!"
                OnRowCommand="gvProjects_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Title"     HeaderText="Project Title" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# "vh-badge " + GetStatusBadgeClass((string)Eval("Status")) %>'><%# Eval("Status") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Location"  HeaderText="Location"  NullDisplayText="—" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start"     DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate"   HeaderText="End"       DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/Admin/ProjectDetail.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-outline btn-sm">
                                <i class="bi bi-eye"></i>
                            </a>
                            <a href='<%# ResolveUrl("~/Pages/Admin/EditProject.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-ghost btn-sm">
                                <i class="bi bi-pencil"></i>
                            </a>
                            <asp:LinkButton runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                CssClass="btn vh-btn-danger btn-sm"
                                OnClientClick="return confirm('Delete this project? All events and enrollments will be removed. This cannot be undone.');">
                                <i class="bi bi-trash"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
