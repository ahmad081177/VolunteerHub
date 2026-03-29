<%@ Page Title="My Hours" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyEvents.aspx.cs" Inherits="VolunteerHub.Pages.Volunteer.MyEvents" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <span class="vh-breadcrumb-item">My Hours</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">My Hours</h1>
            <p class="vh-page-desc">Complete log of all your volunteer events.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Volunteer/LogEvent.aspx") %>" class="btn vh-btn-primary">
            <i class="bi bi-plus-lg"></i> Log Hours
        </a>
    </div>

    <asp:Literal ID="litAlert" runat="server"></asp:Literal>

    <!-- Summary stat -->
    <div class="vh-stats-grid mb-4" style="--vh-stats-cols: 3;">
        <div class="vh-stat-card vh-stat-indigo">
            <div class="vh-stat-icon"><i class="bi bi-clock-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statTotal" runat="server">0</div>
                <div class="vh-stat-label">Total Hours</div>
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
            <div class="vh-stat-icon"><i class="bi bi-bar-chart-fill"></i></div>
            <div class="vh-stat-body">
                <div class="vh-stat-value" id="statAvg" runat="server">0</div>
                <div class="vh-stat-label">Avg Hrs/Event</div>
            </div>
        </div>
    </div>

    <div class="vh-card">
        <div class="vh-card-body p-0">
            <asp:GridView ID="gvEvents" runat="server" AutoGenerateColumns="false"
                CssClass="vh-table" GridLines="None" EmptyDataText="No hours logged yet. Start volunteering!"
                OnRowCommand="gvEvents_RowCommand">
                <Columns>
                    <asp:BoundField DataField="ProjectTitle" HeaderText="Project" />
                    <asp:BoundField DataField="EventDate"    HeaderText="Date"    DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Time">
                        <ItemTemplate><%# Eval("DurationDisplay") %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="HoursLogged"  HeaderText="Hours"   DataFormatString="{0:0.#}" />
                    <asp:BoundField DataField="Notes"        HeaderText="Notes"   NullDisplayText="—" />
                    <asp:TemplateField HeaderText="" ItemStyle-CssClass="vh-table-actions">
                        <ItemTemplate>
                            <div class="d-flex gap-1">
                                <a href='<%# ResolveUrl("~/Pages/Volunteer/EditEvent.aspx") + "?id=" + Eval("Id") %>'
                                   class="btn btn-sm vh-btn-outline" title="Edit">
                                    <i class="bi bi-pencil"></i>
                                </a>
                                <asp:LinkButton CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn btn-sm vh-btn-danger"
                                    runat="server" OnClientClick="return confirm('Delete this event?')">
                                    <i class="bi bi-trash"></i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
