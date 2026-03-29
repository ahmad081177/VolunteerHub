<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="VolunteerHub.Pages.Admin.Dashboard" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Dashboard</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Workspace Dashboard</h1>
            <p class="vh-page-desc" id="wsSubtitle" runat="server">Welcome back!</p>
        </div>
    </div>

    <!-- Stats -->
    <div class="vh-stats-grid">
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-folder2-open"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statProjects" runat="server">0</div>
                <div class="vh-stat-label">Projects</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-people-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statVolunteers" runat="server">0</div>
                <div class="vh-stat-label">Volunteers</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-emerald">
            <div class="vh-stat-icon"><i class="bi bi-clock-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statHours" runat="server">0</div>
                <div class="vh-stat-label">Total Hours</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-amber">
            <div class="vh-stat-icon"><i class="bi bi-calendar-check-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statActive" runat="server">0</div>
                <div class="vh-stat-label">Active Projects</div>
            </div>
        </div>
    </div>

    <!-- Charts Row -->
    <div class="row mt-4 g-4">
        <div class="col-lg-6">
            <div class="vh-card h-100">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-bar-chart-fill"></i> Hours per Project</h2>
                </div>
                <div class="vh-card-body">
                    <asp:Panel ID="pnlHrsChart" runat="server">
                        <div class="vh-chart-container">
                            <canvas id="chartHoursPerProject"></canvas>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlHrsEmpty" runat="server" Visible="false">
                        <div class="vh-empty-state py-4">
                            <i class="bi bi-bar-chart vh-empty-icon"></i>
                            <p class="vh-empty-text">No hours logged yet.</p>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="col-lg-6">
            <div class="vh-card h-100">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-people"></i> Volunteers per Project</h2>
                </div>
                <div class="vh-card-body">
                    <asp:Panel ID="pnlVolsChart" runat="server">
                        <div class="vh-chart-container">
                            <canvas id="chartVolsPerProject"></canvas>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlVolsEmpty" runat="server" Visible="false">
                        <div class="vh-empty-state py-4">
                            <i class="bi bi-people vh-empty-icon"></i>
                            <p class="vh-empty-text">No volunteers enrolled yet.</p>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Projects -->
    <div class="vh-card mt-4">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-folder"></i> Recent Projects</h2>
            <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="btn vh-btn-outline btn-sm">View All</a>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvProjects" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No projects yet.">
                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Project" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='<%# "vh-badge " + GetStatusBadgeClass((string)Eval("Status")) %>'><%# Eval("Status") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="StartDate" HeaderText="Start" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate"   HeaderText="End"   DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/Admin/ProjectDetail.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-outline btn-sm">
                                <i class="bi bi-eye"></i>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Recent Volunteers -->
    <div class="vh-card mt-4">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-people"></i> Recent Volunteers</h2>
            <a href="<%= ResolveUrl("~/Pages/Admin/Volunteers.aspx") %>" class="btn vh-btn-outline btn-sm">View All</a>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No volunteers registered yet.">
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
                    <asp:BoundField DataField="CreatedAt" HeaderText="Joined" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/Admin/VolunteerDetail.aspx?id=" + Eval("Id")) %>' class="btn vh-btn-outline btn-sm">
                                <i class="bi bi-eye"></i>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
    <asp:Literal ID="litChartScript" runat="server"></asp:Literal>
</asp:Content>
