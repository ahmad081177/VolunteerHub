<%@ Page Title="Project Detail" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ProjectDetail.aspx.cs" Inherits="VolunteerHub.Pages.Admin.ProjectDetail" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="vh-breadcrumb-item">Projects</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active" id="breadcrumbTitle" runat="server">Detail</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title" id="pageTitle" runat="server">Project</h1>
            <p class="vh-page-desc" id="pageMeta" runat="server"></p>
        </div>
        <div class="d-flex gap-2">
            <a href='<%= ResolveUrl("~/Pages/Admin/EditProject.aspx") + "?id=" + Request.QueryString["id"] %>'
               class="btn vh-btn-ghost"><i class="bi bi-pencil"></i> Edit</a>
            <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="btn vh-btn-outline">
                <i class="bi bi-arrow-left"></i> Back
            </a>
        </div>
    </div>

    <!-- Stats Row -->
    <div class="vh-stats-grid mb-4">
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-people-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statEnrolled" runat="server">0</div>
                <div class="vh-stat-label">Enrolled</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-emerald">
            <div class="vh-stat-icon"><i class="bi bi-clock-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statTotalHours" runat="server">0</div>
                <div class="vh-stat-label">Total Hours</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-calendar-range"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statDaysLeft" runat="server">—</div>
                <div class="vh-stat-label">Days Left</div>
            </div>
        </div>
    </div>

    <!-- Description Card -->
    <div class="vh-card mb-4" id="descriptionCard" runat="server">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-info-circle"></i> Description</h2>
        </div>
        <div class="vh-card-body">
            <p id="descriptionText" runat="server" class="mb-0"></p>
        </div>
    </div>

    <!-- Enrolled Volunteers -->
    <div class="vh-card">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-people"></i> Enrolled Volunteers</h2>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No volunteers enrolled yet.">
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
                    <asp:BoundField DataField="HoursLogged"   HeaderText="Hours Logged"  DataFormatString="{0:0.#}" />
                    <asp:BoundField DataField="EnrolledAt"    HeaderText="Enrolled"      DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Progress">
                        <ItemTemplate>
                            <div class="vh-progress-wrap">
                                <div class="vh-progress">
                                    <div class="vh-progress-bar" style='width:<%# Eval("ProgressPct") %>%'></div>
                                </div>
                                <span class="vh-progress-label"><%# Eval("ProgressPct") %>%</span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <a href='<%# ResolveUrl("~/Pages/Admin/VolunteerProjectEvents.aspx") + "?projectId=" + Request.QueryString["id"] + "&userId=" + Eval("UserId") %>'
                               class="btn btn-sm vh-btn-outline" title="View Events">
                                <i class="bi bi-clock-history"></i>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
