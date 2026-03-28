<%@ Page Title="Error" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="VolunteerHub.Pages.Error" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="vh-auth-container">
        <div class="vh-auth-card" style="text-align:center;">
            <div style="font-size:4rem; color:var(--vh-danger); margin-bottom:1rem;">
                <i class="bi bi-exclamation-triangle-fill"></i>
            </div>
            <h2 style="font-size:1.75rem; font-weight:700; margin-bottom:0.5rem;">Something went wrong</h2>
            <p style="color:var(--vh-text-muted); margin-bottom:2rem;">
                An unexpected error occurred. Please try again or contact support if the issue persists.
            </p>
            <asp:Literal ID="litError" runat="server"></asp:Literal>
            <div class="mt-3">
                <a href="<%= ResolveUrl("~/") %>" class="btn vh-btn-primary">
                    <i class="bi bi-house"></i> Go Home
                </a>
            </div>
        </div>
    </div>
</asp:Content>
