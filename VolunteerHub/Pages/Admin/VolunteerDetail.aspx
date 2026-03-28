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
            <span class="vh-badge" id="profileStatus" runat="server"></span>
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
    </div>

    <!-- Event History -->
    <div class="vh-card">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-clock-history"></i> Volunteer History</h2>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No hours logged yet.">
                <Columns>
                    <asp:BoundField DataField="ProjectTitle" HeaderText="Project" />
                    <asp:BoundField DataField="EventDate"    HeaderText="Date"    DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="HoursLogged"  HeaderText="Hours"   DataFormatString="{0:0.#}" />
                    <asp:BoundField DataField="Notes"        HeaderText="Notes"   NullDisplayText="—" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
