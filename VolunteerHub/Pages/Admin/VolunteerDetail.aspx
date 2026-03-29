<%@ Page Title="Volunteer Detail" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="VolunteerDetail.aspx.cs" Inherits="VolunteerHub.Pages.Admin.VolunteerDetail" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Admin/Volunteers.aspx") %>" class="vh-breadcrumb-item">Volunteers</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active" id="breadcrumbName" runat="server">Detail</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <!-- Profile Header -->
    <div class="vh-profile-header mb-4">
        <div class="vh-profile-avatar" id="profileAvatar" runat="server"></div>
        <div class="vh-profile-info">
            <h1 class="vh-page-title" id="profileName" runat="server">Volunteer</h1>
            <p class="vh-page-desc" id="profileEmail" runat="server"></p>
            <div class="d-flex gap-2 align-items-center mt-1">
                <span class="vh-badge" id="profileStatus" runat="server"></span>
                <span class="text-muted small" id="profileJoined" runat="server"></span>
            </div>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Admin/Volunteers.aspx") %>" class="btn vh-btn-outline ms-auto">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <!-- Stats -->
    <div class="vh-stats-grid mb-4">
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-clock-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statTotalHours" runat="server">0</div>
                <div class="vh-stat-label">Total Hours</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-bookmark-check-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statProjects" runat="server">0</div>
                <div class="vh-stat-label">Projects</div>
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
                <div class="vh-stat-label">Completed Projects</div>
            </div>
        </div>
    </div>

    <!-- Chart + Project Progress -->
    <div class="row g-4 mb-4">
        <div class="col-lg-8">
            <div class="vh-card h-100">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-graph-up"></i> Hours Logged (Last 6 Months)</h2>
                </div>
                <div class="vh-card-body">
                    <asp:Panel ID="pnlChart" runat="server">
                        <div class="vh-chart-container">
                            <canvas id="chartHoursOverTime"></canvas>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnlChartEmpty" runat="server" Visible="false">
                        <div class="vh-empty-state py-4">
                            <i class="bi bi-graph-up vh-empty-icon"></i>
                            <p class="vh-empty-text">No hours logged in the last 6 months.</p>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="vh-card h-100">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-list-check"></i> Project Progress</h2>
                </div>
                <div class="vh-card-body p-0">
                    <asp:Repeater ID="rptProjects" runat="server">
                        <ItemTemplate>
                            <div class="vh-project-mini-row">
                                <div class="vh-project-mini-info">
                                    <div class="fw-semibold small"><%# Eval("Title") %></div>
                                    <div class="text-muted" style="font-size:.75rem;">
                                        <%# Eval("HoursLogged") %> / <%# Eval("HoursRequired") ?? "&#x221e;" %> hrs
                                    </div>
                                    <div class="vh-progress-wrap mt-1">
                                        <div class="vh-progress">
                                            <div class="vh-progress-bar" style='width:<%# Eval("ProgressPct") %>%'></div>
                                        </div>
                                    </div>
                                </div>
                                <a href='<%# ResolveUrl("~/Pages/Admin/VolunteerProjectEvents.aspx") + "?projectId=" + Eval("ProjectId") + "&userId=" + Eval("UserId") %>'
                                   class="btn btn-sm vh-btn-outline" title="View Events">
                                    <i class="bi bi-clock-history"></i>
                                </a>
                            </div>
                        </ItemTemplate>
                        <FooterTemplate>
                            <%# rptProjects.Items.Count == 0 ? "<p class='p-3 text-muted small'>Not enrolled in any projects yet.</p>" : "" %>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Events + Full History -->
    <div class="vh-card">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-clock-history"></i> Recent Events</h2>
            <asp:HyperLink ID="lnkViewAll" runat="server" CssClass="btn vh-btn-outline btn-sm" Text="View Full History" />
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No hours logged yet.">
                <Columns>
                    <asp:BoundField DataField="ProjectTitle" HeaderText="Project" />
                    <asp:BoundField DataField="EventDate"    HeaderText="Date"    DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Time">
                        <ItemTemplate><%#: Eval("DurationDisplay") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="HoursLogged"  HeaderText="Hours"   DataFormatString="{0:0.#}" />
                    <asp:BoundField DataField="Notes"        HeaderText="Notes"   NullDisplayText="&#x2014;" />
                    <asp:TemplateField HeaderText="Photos">
                        <ItemTemplate><%# BuildImageThumb((int)Eval("Id")) %></ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="ScriptContent" runat="server">
    <asp:Literal ID="litChartScript" runat="server"></asp:Literal>
</asp:Content>
