<%@ Page Title="VolunteerHub" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="VolunteerHub._Default" %>

<asp:Content ID="HeadExtra" ContentPlaceHolderID="HeadContent" runat="server">
<style>
/* ── Landing-page-only styles ─────────────────────────────── */
.vh-landing-nav {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 2rem;
    height: 64px;
    background: #0F172A;
    position: sticky;
    top: 0;
    z-index: 100;
}
.vh-landing-nav-brand {
    display: flex;
    align-items: center;
    gap: .6rem;
    color: #fff;
    font-weight: 700;
    font-size: 1.15rem;
    text-decoration: none;
}
.vh-landing-nav-brand .vh-brand-icon {
    width: 34px; height: 34px;
    background: var(--vh-primary);
    border-radius: var(--vh-radius-sm);
    display: flex; align-items: center; justify-content: center;
    font-size: 1rem;
    color: #fff;
}
.vh-landing-nav-actions { display: flex; gap: .75rem; }

.vh-hero {
    background: linear-gradient(135deg, #0F172A 0%, #1E3A5F 60%, #312E81 100%);
    color: #fff;
    padding: 96px 0 80px;
    text-align: center;
}
.vh-hero h1 {
    font-size: clamp(2rem, 5vw, 3.25rem);
    font-weight: 800;
    line-height: 1.15;
    color: #fff;
    margin-bottom: 1rem;
}
.vh-hero-sub {
    font-size: 1.125rem;
    color: rgba(255,255,255,.72);
    max-width: 540px;
    margin: 0 auto 2.25rem;
    line-height: 1.6;
}
.vh-hero-actions { display: flex; justify-content: center; gap: 1rem; flex-wrap: wrap; }
.vh-hero-badge {
    display: inline-flex; align-items: center; gap: .4rem;
    background: rgba(79,70,229,.25);
    color: #A5B4FC;
    border: 1px solid rgba(99,102,241,.35);
    border-radius: 999px;
    padding: .3rem .9rem;
    font-size: .8rem;
    font-weight: 600;
    margin-bottom: 1.5rem;
}

.vh-features {
    padding: 80px 0;
    background: var(--vh-body-bg);
}
.vh-feature-card {
    background: var(--vh-card-bg);
    border-radius: var(--vh-radius-lg);
    padding: 2rem 1.75rem;
    text-align: center;
    box-shadow: var(--vh-shadow-sm);
    height: 100%;
    transition: box-shadow var(--vh-transition), transform var(--vh-transition);
}
.vh-feature-card:hover {
    box-shadow: var(--vh-shadow-md);
    transform: translateY(-3px);
}
.vh-feature-icon {
    width: 56px; height: 56px;
    border-radius: var(--vh-radius);
    display: flex; align-items: center; justify-content: center;
    font-size: 1.5rem;
    margin: 0 auto 1.25rem;
}
.vh-feature-icon.indigo  { background: #EEF2FF; color: var(--vh-primary); }
.vh-feature-icon.emerald { background: #D1FAE5; color: var(--vh-success); }
.vh-feature-icon.amber   { background: #FEF3C7; color: var(--vh-warning); }
.vh-feature-card h3 { font-size: 1.05rem; font-weight: 700; margin-bottom: .5rem; }
.vh-feature-card p  { font-size: .9rem; color: var(--vh-text-secondary); margin: 0; }

.vh-steps {
    padding: 80px 0;
    background: #fff;
}
.vh-steps h2, .vh-features h2 {
    font-size: 1.875rem;
    font-weight: 800;
    text-align: center;
    margin-bottom: .6rem;
}
.vh-section-sub {
    text-align: center;
    color: var(--vh-text-secondary);
    margin-bottom: 3rem;
    font-size: .95rem;
}
.vh-step {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    position: relative;
}
.vh-step-num {
    width: 48px; height: 48px;
    border-radius: 50%;
    background: var(--vh-primary);
    color: #fff;
    font-size: 1.1rem;
    font-weight: 700;
    display: flex; align-items: center; justify-content: center;
    margin-bottom: 1rem;
    flex-shrink: 0;
}
.vh-step h3 { font-size: 1rem; font-weight: 700; margin-bottom: .35rem; }
.vh-step p  { font-size: .875rem; color: var(--vh-text-secondary); margin: 0; max-width: 180px; }
.vh-step-connector {
    display: none;
}
@media (min-width: 768px) {
    .vh-step-connector {
        display: block;
        flex: 1;
        height: 2px;
        background: var(--vh-border);
        margin-top: -1.5rem;
        align-self: flex-start;
        margin-top: 24px;
    }
}

.vh-cta-strip {
    background: linear-gradient(135deg, #312E81 0%, #0F172A 100%);
    padding: 64px 0;
    text-align: center;
    color: #fff;
}
.vh-cta-strip h2 { font-size: 1.75rem; font-weight: 800; color: #fff; margin-bottom: .6rem; }
.vh-cta-strip p  { color: rgba(255,255,255,.7); margin-bottom: 1.75rem; }

.vh-landing-footer {
    background: #0F172A;
    color: rgba(255,255,255,.4);
    text-align: center;
    padding: 1.25rem 1rem;
    font-size: .8rem;
}
</style>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <!-- ── Navbar ─────────────────────────────────────────────── -->
    <nav class="vh-landing-nav">
        <a href="<%= ResolveUrl("~/") %>" class="vh-landing-nav-brand">
            <div class="vh-brand-icon"><i class="bi bi-heart-fill"></i></div>
            VolunteerHub
        </a>
        <div class="vh-landing-nav-actions">
            <a href="<%= ResolveUrl("~/Pages/Login.aspx") %>" class="btn vh-btn-outline btn-sm" style="color:#fff;border-color:rgba(255,255,255,.35);">Sign In</a>
            <a href="<%= ResolveUrl("~/Pages/Register.aspx") %>" class="btn vh-btn-primary btn-sm">Get Started</a>
        </div>
    </nav>

    <!-- ── Hero ──────────────────────────────────────────────── -->
    <section class="vh-hero">
        <div class="container">
            <div class="vh-hero-badge"><i class="bi bi-heart-fill"></i> Built for volunteer management</div>
            <h1>Manage volunteer hours,<br />effortlessly.</h1>
            <p class="vh-hero-sub">VolunteerHub gives workspace admins and volunteers one place to track projects, log hours, and see progress — no spreadsheets needed.</p>
            <div class="vh-hero-actions">
                <a href="<%= ResolveUrl("~/Pages/Register.aspx") %>" class="btn vh-btn-primary">Get Started — it&#39;s free</a>
                <a href="<%= ResolveUrl("~/Pages/Login.aspx") %>" class="btn vh-btn-outline" style="color:#fff;border-color:rgba(255,255,255,.4);">Sign In <i class="bi bi-arrow-right ms-1"></i></a>
            </div>
        </div>
    </section>

    <!-- ── Features ─────────────────────────────────────────── -->
    <section class="vh-features">
        <div class="container">
            <h2>Everything you need</h2>
            <p class="vh-section-sub">A complete toolkit for admins and volunteers alike.</p>
            <div class="row g-4">
                <div class="col-md-4">
                    <div class="vh-feature-card">
                        <div class="vh-feature-icon indigo"><i class="bi bi-folder2-open"></i></div>
                        <h3>Manage Projects</h3>
                        <p>Create workspace projects, set hour targets, and track volunteer enrollment — all in one dashboard.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="vh-feature-card">
                        <div class="vh-feature-icon emerald"><i class="bi bi-clock-fill"></i></div>
                        <h3>Track Hours</h3>
                        <p>Volunteers log events with dates, durations, and notes. Admins see totals per project and per volunteer instantly.</p>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="vh-feature-card">
                        <div class="vh-feature-icon amber"><i class="bi bi-bar-chart-fill"></i></div>
                        <h3>See Reports</h3>
                        <p>Visual charts show hours per project, volunteers per project, and individual progress — no extra tools required.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- ── How It Works ─────────────────────────────────────── -->
    <section class="vh-steps">
        <div class="container">
            <h2>How it works</h2>
            <p class="vh-section-sub">Three steps from sign-up to tracked hours.</p>
            <div class="d-flex flex-column flex-md-row align-items-md-start justify-content-center gap-0">
                <div class="vh-step px-3">
                    <div class="vh-step-num">1</div>
                    <h3>Create an account</h3>
                    <p>Register as a volunteer and join your organisation&#39;s workspace.</p>
                </div>
                <div class="vh-step-connector mx-2"></div>
                <div class="vh-step px-3">
                    <div class="vh-step-num">2</div>
                    <h3>Join a project</h3>
                    <p>Browse available projects in your workspace and enrol.</p>
                </div>
                <div class="vh-step-connector mx-2"></div>
                <div class="vh-step px-3">
                    <div class="vh-step-num">3</div>
                    <h3>Log your hours</h3>
                    <p>Record events as you go. Your progress updates in real time.</p>
                </div>
            </div>
        </div>
    </section>

    <!-- ── CTA Strip ─────────────────────────────────────────── -->
    <section class="vh-cta-strip">
        <div class="container">
            <h2>Ready to get started?</h2>
            <p>Join VolunteerHub today and make every hour count.</p>
            <div class="d-flex justify-content-center gap-3 flex-wrap">
                <a href="<%= ResolveUrl("~/Pages/Register.aspx") %>" class="btn vh-btn-primary">Create Free Account</a>
                <a href="<%= ResolveUrl("~/Pages/Login.aspx") %>" class="btn vh-btn-outline" style="color:#fff;border-color:rgba(255,255,255,.4);">Sign In</a>
            </div>
        </div>
    </section>

    <!-- ── Footer ────────────────────────────────────────────── -->
    <footer class="vh-landing-footer">
        &copy; <%= DateTime.Now.Year %> VolunteerHub. All rights reserved.
    </footer>

</asp:Content>
