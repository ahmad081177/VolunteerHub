<%@ Page Title="Project Details" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ProjectDetail.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.ProjectDetail" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Volunteer/Projects.aspx") %>" class="vh-breadcrumb-item">Projects</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active" id="breadcrumbTitle" runat="server">Detail</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title" id="pageTitle" runat="server">Project</h1>
            <p class="vh-page-desc" id="pageMeta" runat="server"></p>
        </div>
        <div class="d-flex gap-2 align-items-center">
            <span class="vh-badge" id="statusBadge" runat="server"></span>
            <asp:Button ID="btnJoin" runat="server" Text="Join Project" CssClass="btn vh-btn-primary"
                OnClick="btnJoin_Click" Visible="false" />
            <a href="<%= ResolveUrl("~/Pages/Volunteer/Projects.aspx") %>" class="btn vh-btn-outline">
                <i class="bi bi-arrow-left"></i> Back
            </a>
        </div>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <div class="row g-4">
        <div class="col-lg-8">
            <!-- Description -->
            <div class="vh-card mb-4" id="descCard" runat="server">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-info-circle"></i> About this project</h2>
                </div>
                <div class="vh-card-body">
                    <p id="descText" runat="server"></p>
                </div>
            </div>

            <!-- My Hours for this project -->
            <div class="vh-card" id="myHoursCard" runat="server" visible="false">
                <div class="vh-card-header">
                    <h2 class="vh-card-title"><i class="bi bi-clock-history"></i> My Hours</h2>
                    <a href='<%# "~/Pages/Volunteer/LogEvent.aspx?projectId=" + Request.QueryString["id"] %>'
                       class="btn vh-btn-primary btn-sm"><i class="bi bi-plus"></i> Log Hours</a>
                </div>
                <div class="vh-card-body p-0">
                    <asp:GridView ID="gvMyEvents" runat="server" AutoGenerateColumns="false"
                        CssClass="vh-table" GridLines="None" EmptyDataText="No hours logged yet.">
                        <Columns>
                            <asp:BoundField DataField="EventDate"   HeaderText="Date"  DataFormatString="{0:MMM dd, yyyy}" />
                            <asp:BoundField DataField="HoursLogged" HeaderText="Hours" DataFormatString="{0:0.#}" />
                            <asp:BoundField DataField="Notes"       HeaderText="Notes" NullDisplayText="—" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <!-- Stats card -->
            <div class="vh-card mb-4">
                <div class="vh-card-body">
                    <div class="vh-info-list">
                        <div class="vh-info-item">
                            <span class="vh-info-label"><i class="bi bi-calendar"></i> Start Date</span>
                            <span class="vh-info-value" id="infoStart" runat="server"></span>
                        </div>
                        <div class="vh-info-item">
                            <span class="vh-info-label"><i class="bi bi-calendar-check"></i> End Date</span>
                            <span class="vh-info-value" id="infoEnd" runat="server"></span>
                        </div>
                        <div class="vh-info-item" id="infoLocRow" runat="server">
                            <span class="vh-info-label"><i class="bi bi-geo-alt"></i> Location</span>
                            <span class="vh-info-value" id="infoLoc" runat="server"></span>
                        </div>
                        <div class="vh-info-item" id="infoHrsRow" runat="server">
                            <span class="vh-info-label"><i class="bi bi-clock"></i> Hours Required</span>
                            <span class="vh-info-value" id="infoHrs" runat="server"></span>
                        </div>
                        <div class="vh-info-item" id="infoMaxRow" runat="server">
                            <span class="vh-info-label"><i class="bi bi-people"></i> Max Volunteers</span>
                            <span class="vh-info-value" id="infoMax" runat="server"></span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- My progress (shown when enrolled) -->
            <div class="vh-card" id="progressCard" runat="server" visible="false">
                <div class="vh-card-body">
                    <div class="text-center mb-2 fw-semibold">My Progress</div>
                    <div class="d-flex justify-content-between small text-muted mb-1">
                        <span id="progressLabel" runat="server"></span>
                        <span id="progressPct" runat="server"></span>
                    </div>
                    <div class="vh-progress" style="height:12px;">
                        <div class="vh-progress-bar" id="progressBar" runat="server" style="width:0%"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
