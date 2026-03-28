<%@ Page Title="My Projects" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyProjects.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.MyProjects" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">My Projects</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">My Projects</h1>
            <p class="vh-page-desc">Track your progress across all enrolled projects.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Volunteer/Projects.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-search"></i> Browse More
        </a>
    </div>

    <asp:Repeater ID="rptProjects" runat="server">
        <HeaderTemplate><div class="row g-3"></HeaderTemplate>
        <ItemTemplate>
            <div class="col-md-6 col-xl-4">
                <div class="vh-project-card">
                    <div class="vh-project-card-header">
                        <span class='<%# "vh-badge " + Eval("StatusBadgeClass") %>'><%# Eval("Status") %></span>
                    </div>
                    <h3 class="vh-project-card-title"><%# Eval("Title") %></h3>

                    <!-- Progress bar toward required hours -->
                    <div class="my-2">
                        <div class="d-flex justify-content-between small text-muted mb-1">
                            <span><%# Eval("HoursLogged") %> / <%# Eval("HoursRequired") ?? "∞" %> hrs</span>
                            <span><%# Eval("ProgressPct") %>%</span>
                        </div>
                        <div class="vh-progress">
                            <div class="vh-progress-bar" style='width:<%# Eval("ProgressPct") %>%'></div>
                        </div>
                    </div>

                    <div class="vh-project-card-meta">
                        <span><i class="bi bi-calendar"></i> <%# ((System.DateTime)Eval("StartDate")).ToString("MMM dd") %> – <%# ((System.DateTime)Eval("EndDate")).ToString("MMM dd, yyyy") %></span>
                    </div>
                    <div class="vh-project-card-footer">
                        <a href='<%# ResolveUrl("~/Pages/Volunteer/ProjectDetail.aspx?id=" + Eval("Id")) %>'
                           class="btn vh-btn-outline btn-sm"><i class="bi bi-eye"></i> View</a>
                        <a href='<%# ResolveUrl("~/Pages/Volunteer/LogEvent.aspx?projectId=" + Eval("Id")) %>'
                           class="btn vh-btn-primary btn-sm"><i class="bi bi-plus"></i> Log Hours</a>
                    </div>
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
            <%# rptProjects.Items.Count == 0 ? "<div class='vh-empty-state'><i class='bi bi-bookmark'></i><p>You haven't joined any projects yet. <a href='Projects.aspx'>Browse projects</a>.</p></div>" : "" %>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
