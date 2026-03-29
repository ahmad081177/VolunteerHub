<%@ Page Title="Volunteer Events" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="VolunteerProjectEvents.aspx.cs" Inherits="VolunteerHub.Pages.Admin.VolunteerProjectEvents" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="vh-breadcrumb-item">Projects</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <asp:HyperLink ID="breadcrumbProject" runat="server" CssClass="vh-breadcrumb-item" Text="Project" />
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active" id="breadcrumbVolunteer" runat="server">Events</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title" id="pageTitle" runat="server">Volunteer Events</h1>
            <p class="vh-page-desc" id="pageMeta" runat="server"></p>
        </div>
        <a href='<%= ResolveUrl("~/Pages/Admin/ProjectDetail.aspx") + "?id=" + Request.QueryString["projectId"] %>' class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back to Project
        </a>
    </div>

    <!-- Stats -->
    <div class="vh-stats-grid mb-4" style="--vh-stats-cols: 3;">
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-clock-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statHours" runat="server">0</div>
                <div class="vh-stat-label">Hours Logged</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-blue">
            <div class="vh-stat-icon"><i class="bi bi-calendar3"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statCount" runat="server">0</div>
                <div class="vh-stat-label">Events</div>
            </div>
        </div>
        <div class="vh-stat-card vh-stat-emerald">
            <div class="vh-stat-icon"><i class="bi bi-images"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statPhotos" runat="server">0</div>
                <div class="vh-stat-label">Photos</div>
            </div>
        </div>
    </div>

    <!-- Events Table -->
    <div class="vh-card">
        <div class="vh-card-header">
            <h2 class="vh-card-title"><i class="bi bi-clock-history"></i> Event Log</h2>
        </div>
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No hours logged yet.">
                <Columns>
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
