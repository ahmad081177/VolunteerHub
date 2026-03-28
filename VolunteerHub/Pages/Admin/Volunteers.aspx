<%@ Page Title="Volunteers" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Volunteers.aspx.cs" Inherits="VolunteerHub.Pages.Admin.Volunteers" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Volunteers</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Volunteers</h1>
            <p class="vh-page-desc">All volunteers in your workspace.</p>
        </div>
    </div>

    <div class="vh-card">
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No volunteers registered yet."
                OnRowCommand="gvVolunteers_RowCommand">
                <Columns>
                    <asp:TemplateField HeaderText="Volunteer">
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
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# (bool)Eval("IsActive") ? "vh-badge vh-badge-success" : "vh-badge vh-badge-muted" %>'>
                                <%# (bool)Eval("IsActive") ? "Active" : "Inactive" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreatedAt"   HeaderText="Joined"     DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="LastLoginAt" HeaderText="Last Login"  DataFormatString="{0:MMM dd, yyyy}" NullDisplayText="Never" />
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/Admin/VolunteerDetail.aspx?id=" + Eval("Id")) %>'
                               class="btn vh-btn-outline btn-sm"><i class="bi bi-eye"></i></a>
                            <asp:LinkButton CommandName="Toggle"
                                CommandArgument='<%# Eval("Id") + "," + Eval("IsActive") %>'
                                CssClass='<%# "btn btn-sm " + ((bool)Eval("IsActive") ? "vh-btn-danger" : "vh-btn-outline") %>'
                                runat="server"
                                OnClientClick="return confirm('Toggle volunteer status?')">
                                <%# (bool)Eval("IsActive") ? "Deactivate" : "Activate" %>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
