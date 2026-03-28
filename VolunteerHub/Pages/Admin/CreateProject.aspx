<%@ Page Title="Create Project" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="CreateProject.aspx.cs" Inherits="VolunteerHub.Pages.Admin.CreateProject" %>

<asp:Content ContentPlaceHolderID="BreadcrumbContent" runat="server">
    <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="vh-breadcrumb-item">Projects</a>
    <i class="bi bi-chevron-right vh-breadcrumb-sep"></i>
    <span class="vh-breadcrumb-item active">Create</span>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-page-header">
        <div>
            <h1 class="vh-page-title">Create Project</h1>
            <p class="vh-page-desc">Set up a new volunteering project for your workspace.</p>
        </div>
        <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="btn vh-btn-outline">
            <i class="bi bi-arrow-left"></i> Back
        </a>
    </div>

    <div class="vh-card" style="max-width:760px;">
        <div class="vh-card-body">
            <asp:Literal ID="litAlert" runat="server"></asp:Literal>

            <div class="vh-form-group">
                <label class="vh-label">Project Title <span class="vh-required">*</span></label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="vh-input" placeholder="e.g. Beach Cleanup Drive" MaxLength="200" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle"
                    CssClass="vh-field-error" ErrorMessage="Title is required." Display="Dynamic" />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="vh-input" TextMode="MultiLine"
                    Rows="4" placeholder="Describe the project goals, activities, and impact..." />
            </div>

            <div class="vh-form-group">
                <label class="vh-label">Location</label>
                <asp:TextBox ID="txtLocation" runat="server" CssClass="vh-input" placeholder="e.g. City Park, Downtown" MaxLength="300" />
            </div>

            <div class="row g-3">
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">Start Date <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="vh-input" TextMode="Date" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtStartDate"
                            CssClass="vh-field-error" ErrorMessage="Start date required." Display="Dynamic" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">End Date <span class="vh-required">*</span></label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="vh-input" TextMode="Date" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEndDate"
                            CssClass="vh-field-error" ErrorMessage="End date required." Display="Dynamic" />
                    </div>
                </div>
            </div>

            <div class="row g-3">
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">Max Volunteers</label>
                        <asp:TextBox ID="txtMaxVols" runat="server" CssClass="vh-input" TextMode="Number" placeholder="Leave blank for unlimited" />
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="vh-form-group">
                        <label class="vh-label">Required Hours per Volunteer</label>
                        <asp:TextBox ID="txtHoursRequired" runat="server" CssClass="vh-input" TextMode="Number" placeholder="e.g. 20" />
                    </div>
                </div>
            </div>

            <div class="mt-4 d-flex gap-2">
                <asp:Button ID="btnSave" runat="server" Text="Create Project" CssClass="btn vh-btn-primary" OnClick="btnSave_Click" />
                <a href="<%= ResolveUrl("~/Pages/Admin/Projects.aspx") %>" class="btn vh-btn-ghost">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>
