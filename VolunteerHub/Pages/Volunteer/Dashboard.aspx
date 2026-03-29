<%@ Page Title="My Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.Dashboard" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">Dashboard</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title" id="greeting" runat="server">Welcome back!</h1>
            <p class="vh-page-desc">Track your volunteering journey.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Volunteer/LogEvent.aspx") %>" class="btn vh-btn-primary">
            <i class="bi bi-plus-lg"></i> Log Hours
        </a>
    </div>

    <!-- Stats -->
    <div class="vh-stats-grid">
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-clock-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value vh-counter" id="statHours" runat="server">0</div>
                <div class="vh-stat-label">Total Hours</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-bookmark-check-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statProjects" runat="server">0</div>
                <div class="vh-stat-label">My Projects</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-emerald">
            <div class="vh-stat-icon"><i class="bi bi-calendar3"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statEvents" runat="server">0</div>
                <div class="vh-stat-label">Events Logged</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-amber">
            <div class="vh-stat-icon"><i class="bi bi-trophy-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statCompleted" runat="server">0</div>
                <div class="vh-stat-label">Completed Goals</div>
            </div>
        </div>
    </div>

    <!-- Hours Over Time Chart -->
    <div class="row mt-4 g-4">
        <div class="col-lg-8">
            <div class="vh-card h-100">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-graph-up"></i> Hours Logged (Last 6 Months)</h2>
                </div>
                <div class="vh-card-body">
                    <div class="vh-chart-container">
                        <canvas id="chartHoursOverTime"></canvas>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="vh-card h-100">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-list-check"></i> My Projects</h2>
                </div>
                <div class="vh-card-body p-0">
                    <asp:Repeater ID="rptProjects" runat="server">
                        <ItemTemplate>
                            <div class="vh-project-mini-row">
                                <div class="vh-project-mini-info">
                                    <div class="fw-semibold small"><%# Eval("Title") %></div>
                                    <div class="text-muted" style="font-size:.75rem;">
                                        <%# Eval("HoursLogged") %> / <%# Eval("HoursRequired") ?? "∞" %> hrs
                                    </div>
                                </div>
                                <span class='<%# "vh-badge " + GetStatusBadgeClass((string)Eval("Status")) %>'><%# Eval("Status") %></span>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%# rptProjects.Items.Count == 0 ? "<p class='p-3 text-muted small'>No projects yet. <a href='Projects.aspx'>Browse projects</a></p>" : "" %>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Events -->
    <div class="vh-card mt-4">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-clock-history"></i> Recent Events</h2>
            <a href="<%= ResolveUrl("~/Pages/Volunteer/MyEvents.aspx") %>" class="btn vh-btn-outline btn-sm">View All</a>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No events logged yet.">
                <Columns>
                    <asp:BoundField DataField="ProjectTitle" HeaderText="Project" />
                    <asp:BoundField DataField="EventDate"    HeaderText="Date"    DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="HoursLogged"  HeaderText="Hours"   DataFormatString="{0:0.#}" />
                    <asp:BoundField DataField="Notes"        HeaderText="Notes"   NullDisplayText="—" />
                    <asp:TemplateField HeaderText="" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/Volunteer/EditEvent.aspx") + "?id=" + Eval("Id") %>'
                               class="btn btn-sm vh-btn-outline" title="Edit">
                                <i class="bi bi-pencil"></i>
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
