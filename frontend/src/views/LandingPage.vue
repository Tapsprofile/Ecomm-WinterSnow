<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const chatOpen = ref(false)
const form = ref({
  fullName: '',
  email: '',
  projectType: 'E-commerce Migration',
  details: ''
})

const year = computed(() => new Date().getFullYear())

function toggleChat(next) {
  chatOpen.value = typeof next === 'boolean' ? next : !chatOpen.value
}

function goStore() {
  router.push('/store')
}

function goLogin() {
  router.push('/login')
}

function submitContact() {
  // Marketing-only scaffold for now (no backend endpoint yet)
  alert('Thanks! Our team will reach out shortly (demo stub).')
  form.value = { fullName: '', email: '', projectType: 'E-commerce Migration', details: '' }
}

function onKeydown(e) {
  if (e.key === 'Escape') chatOpen.value = false
}

function onScroll() {
  const nav = document.querySelector('[data-ws-landing-nav="1"]')
  if (!nav) return
  nav.classList.toggle('ws-nav-shadow', window.scrollY > 50)
}

onMounted(() => {
  window.addEventListener('keydown', onKeydown)
  window.addEventListener('scroll', onScroll, { passive: true })
  onScroll()
})

onUnmounted(() => {
  window.removeEventListener('keydown', onKeydown)
  window.removeEventListener('scroll', onScroll)
})
</script>

<template>
  <div class="ws-landing">
    <!-- Navbar -->
    <nav class="ws-nav-glass" data-ws-landing-nav="1">
      <div class="container">
        <div class="d-flex align-items-center justify-content-between" style="height: 72px;">
          <a href="#top" class="d-flex align-items-center gap-2 text-decoration-none">
            <span class="ws-brand-badge">
              <i class="bi bi-shield-check text-white" />
            </span>
            <span class="ws-brand">
              WINTER<span class="ws-brand-accent">SNOW</span>
            </span>
          </a>

          <div class="d-none d-lg-flex align-items-center gap-4 ws-nav-links">
            <a href="#identity" class="ws-nav-link">Who We Are</a>
            <a href="#solutions" class="ws-nav-link">Solutions</a>
            <a href="#reviews" class="ws-nav-link">Reviews</a>
            <a href="#contact" class="ws-nav-link">Contact</a>
          </div>

          <div class="d-flex align-items-center gap-2">
            <button class="btn ws-btn-outline d-none d-xl-inline-flex" type="button" @click="goLogin">
              <i class="bi bi-box-arrow-in-right me-2" />
              Login
            </button>
            <a class="btn ws-btn-danger" href="#contact">
              Get Enterprise Audit
            </a>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero -->
    <section id="top" class="ws-hero">
      <div class="ws-hero-glow ws-hero-glow-blue" />
      <div class="ws-hero-glow ws-hero-glow-red" />

      <div class="container position-relative" style="z-index: 2;">
        <div class="row align-items-center g-5">
          <div class="col-lg-7">
            <div class="ws-pill mb-4">
              <span class="ws-dot" aria-hidden="true" />
              v5.2 Marketplace Edition Now Live
            </div>

            <h1 class="ws-hero-title mb-3">
              Architecting <span class="ws-text-brand">Dominant</span> Commerce.
            </h1>
            <p class="ws-hero-sub mb-4">
              WinterSnow is a nopCommerce-inspired marketplace framework with Shopify-grade UX foundations: multi-vendor catalog, split checkout, payments routing, and notifications.
            </p>

            <div class="d-flex flex-column flex-sm-row gap-3">
              <button class="btn ws-btn-dark" type="button" @click="goStore">
                Enter Storefront
                <i class="bi bi-arrow-right-circle ms-2" />
              </button>
              <a class="btn ws-btn-outline" href="#solutions">
                Watch Demos
                <i class="bi bi-play-circle ms-2" />
              </a>
            </div>

            <div class="d-flex flex-wrap gap-3 mt-4 ws-microproof">
              <div class="ws-chip"><i class="bi bi-lightning-charge me-2" />Fast Discovery</div>
              <div class="ws-chip"><i class="bi bi-diagram-3 me-2" />Split Orders</div>
              <div class="ws-chip"><i class="bi bi-bell me-2" />In‑App Alerts</div>
              <div class="ws-chip"><i class="bi bi-credit-card me-2" />Multi‑Gateway Payments</div>
            </div>
          </div>

          <div class="col-lg-5">
            <div class="ws-hero-card">
              <img
                class="ws-hero-img"
                alt="WinterSnow platform preview"
                src="https://images.unsplash.com/photo-1460925895917-afdab827c52f?auto=format&fit=crop&w=1200&q=80"
              />
            </div>
            <div class="ws-stat d-none d-xl-block">
              <div class="ws-stat-kicker">Growth Engine</div>
              <div class="ws-stat-value">412%</div>
              <div class="ws-stat-foot">Revenue Lift (demo)</div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Identity -->
    <section id="identity" class="py-5 py-lg-6 bg-body">
      <div class="container py-4">
        <div class="row align-items-center g-5">
          <div class="col-lg-6 position-relative">
            <div class="row g-3">
              <div class="col-6">
                <img
                  class="w-100 ws-rounded ws-shadow-lite mt-4 ws-cover"
                  alt="Team"
                  src="https://images.unsplash.com/photo-1522071820081-009f0129c71c?auto=format&fit=crop&w=800&q=80"
                />
              </div>
              <div class="col-6">
                <img
                  class="w-100 ws-rounded ws-shadow-lite ws-cover"
                  alt="Planning"
                  src="https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=800&q=80"
                />
              </div>
            </div>
            <div class="ws-badge-years">
              <div class="ws-badge-years-value">12+</div>
              <div class="ws-badge-years-label">Years of Excellence</div>
            </div>
          </div>

          <div class="col-lg-6">
            <div class="ws-section-kicker">Our Identity</div>
            <h2 class="ws-section-title mb-3">
              We build the <span class="ws-text-brand">operating system</span> for marketplaces.
            </h2>
            <p class="text-secondary mb-4">
              WinterSnow is engineered for teams that want enterprise architecture patterns without sacrificing speed: layered services, audited workflows, configurable providers, and role-based portals.
            </p>

            <div class="d-flex gap-3 align-items-start mb-3">
              <div class="ws-icon-tile"><i class="bi bi-check2-circle" /></div>
              <div>
                <div class="fw-bold">Obsessive Performance</div>
                <div class="text-secondary small">Millisecond-friendly UX: fast search, clean checkout, responsive dashboards.</div>
              </div>
            </div>
            <div class="d-flex gap-3 align-items-start mb-4">
              <div class="ws-icon-tile"><i class="bi bi-check2-circle" /></div>
              <div>
                <div class="fw-bold">Partner-Centric Model</div>
                <div class="text-secondary small">Built for vendors: listings, inventory, payouts, and notifications.</div>
              </div>
            </div>

            <a class="ws-link" href="#solutions">Learn about our mission</a>
          </div>
        </div>
      </div>
    </section>

    <!-- Solutions -->
    <section id="solutions" class="py-5 py-lg-6 ws-alt">
      <div class="container py-4">
        <div class="text-center mb-4">
          <div class="ws-section-kicker">Solutions</div>
          <h2 class="ws-section-title">Enterprise modules, mapped to the 12 strategies</h2>
          <p class="text-secondary mt-2 mb-0">Customer storefront, vendor cockpit, and admin command center — with extensible provider hooks.</p>
        </div>

        <div class="row g-3">
          <div class="col-md-6 col-lg-4">
            <div class="card ws-card card-body h-100">
              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="ws-feature-ic"><i class="bi bi-person-check" /></span>
                <div class="fw-bold">Identity & Access</div>
              </div>
              <div class="text-secondary small">RBAC login flows (Customer/Vendor/Admin) + theme per role.</div>
            </div>
          </div>
          <div class="col-md-6 col-lg-4">
            <div class="card ws-card card-body h-100">
              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="ws-feature-ic"><i class="bi bi-box-seam" /></span>
                <div class="fw-bold">Universal Product Catalog</div>
              </div>
              <div class="text-secondary small">Categories, variants, rich media, SEO slugs, discount and coupon flags.</div>
            </div>
          </div>
          <div class="col-md-6 col-lg-4">
            <div class="card ws-card card-body h-100">
              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="ws-feature-ic"><i class="bi bi-search" /></span>
                <div class="fw-bold">Discovery & Search</div>
              </div>
              <div class="text-secondary small">Autocomplete + faceted filtering (size, material, price) + postal-code visibility rules.</div>
            </div>
          </div>
          <div class="col-md-6 col-lg-4">
            <div class="card ws-card card-body h-100">
              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="ws-feature-ic"><i class="bi bi-diagram-3" /></span>
                <div class="fw-bold">Order Orchestration</div>
              </div>
              <div class="text-secondary small">Split orders by vendor + status tracking + inventory guard patterns.</div>
            </div>
          </div>
          <div class="col-md-6 col-lg-4">
            <div class="card ws-card card-body h-100">
              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="ws-feature-ic"><i class="bi bi-credit-card-2-front" /></span>
                <div class="fw-bold">Payments Routing</div>
              </div>
              <div class="text-secondary small">Per‑vendor gateway mapping + transaction query by IDs/provider name (admin & vendor APIs).</div>
            </div>
          </div>
          <div class="col-md-6 col-lg-4">
            <div class="card ws-card card-body h-100">
              <div class="d-flex align-items-center gap-2 mb-2">
                <span class="ws-feature-ic"><i class="bi bi-bell" /></span>
                <div class="fw-bold">Notifications</div>
              </div>
              <div class="text-secondary small">Internal queue + in-app inbox + email/SMS stubs; UI bell for customers and vendors.</div>
            </div>
          </div>
        </div>

        <div class="d-flex justify-content-center gap-3 mt-4 flex-wrap">
          <button class="btn ws-btn-dark" type="button" @click="goStore">
            Explore the Store
            <i class="bi bi-bag-check ms-2" />
          </button>
          <button class="btn ws-btn-outline" type="button" @click="goLogin">
            Open Portal Login
            <i class="bi bi-grid ms-2" />
          </button>
        </div>
      </div>
    </section>

    <!-- Reviews -->
    <section id="reviews" class="py-5 py-lg-6 bg-body">
      <div class="container py-4">
        <div class="text-center mb-4">
          <h2 class="ws-section-title">Voice of the <span class="ws-text-danger">Industry</span></h2>
          <p class="text-secondary mt-2 mb-0">Demo testimonials for the landing page layout.</p>
        </div>

        <div class="row g-3">
          <div class="col-md-6 col-lg-4">
            <div class="card ws-review card-body h-100">
              <div class="ws-stars mb-3" aria-label="5 stars">
                <i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" />
              </div>
              <p class="text-secondary fst-italic mb-4">
                “Our checkout became simpler overnight. Split orders + payment routing removed a ton of vendor chaos.”
              </p>
              <div class="d-flex align-items-center gap-3">
                <img class="ws-avatar" alt="Reviewer" src="https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&w=120&q=80" />
                <div>
                  <div class="fw-bold small">Thomas K. Reed</div>
                  <div class="text-secondary ws-meta">CTO @ UrbanStride</div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card ws-review card-body h-100 ws-review-pop">
              <div class="ws-stars mb-3" aria-label="5 stars">
                <i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" />
              </div>
              <p class="text-secondary fst-italic mb-4">
                “The vendor portal is finally task-oriented — listings, inventory, payouts, notifications — all in one place.”
              </p>
              <div class="d-flex align-items-center gap-3">
                <img class="ws-avatar" alt="Reviewer" src="https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=120&q=80" />
                <div>
                  <div class="fw-bold small">Elena Rodriguez</div>
                  <div class="text-secondary ws-meta">Founder @ BloomCommerce</div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6 col-lg-4">
            <div class="card ws-review card-body h-100">
              <div class="ws-stars mb-3" aria-label="5 stars">
                <i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" /><i class="bi bi-star-fill" />
              </div>
              <p class="text-secondary fst-italic mb-4">
                “Faceted search + postcode visibility helped us control where products show up without manual catalog splitting.”
              </p>
              <div class="d-flex align-items-center gap-3">
                <img class="ws-avatar" alt="Reviewer" src="https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?auto=format&fit=crop&w=120&q=80" />
                <div>
                  <div class="fw-bold small">Marcus Wei</div>
                  <div class="text-secondary ws-meta">Growth Lead @ TechGear</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Contact -->
    <section id="contact" class="py-5 py-lg-6 ws-contact">
      <div class="container py-4">
        <div class="ws-contact-shell">
          <div class="ws-contact-glow" />
          <div class="row g-4 align-items-center position-relative" style="z-index: 2;">
            <div class="col-lg-6">
              <h2 class="ws-contact-title mb-3">
                Ready to <span class="ws-text-danger">Pivot</span> to Success?
              </h2>
              <p class="ws-contact-sub mb-4">
                Contact our senior architecture team for a complimentary audit and roadmap (demo UI).
              </p>

              <div class="d-flex gap-3 align-items-center mb-3">
                <div class="ws-contact-ic"><i class="bi bi-envelope" /></div>
                <div>
                  <div class="ws-contact-kicker">Email Support</div>
                  <div class="ws-contact-value">architecture@wintersnow.local</div>
                </div>
              </div>
              <div class="d-flex gap-3 align-items-center">
                <div class="ws-contact-ic"><i class="bi bi-telephone" /></div>
                <div>
                  <div class="ws-contact-kicker">Urgent Inquiries</div>
                  <div class="ws-contact-value">+1 (415) 555-0198</div>
                </div>
              </div>
            </div>

            <div class="col-lg-6">
              <div class="card ws-form card-body">
                <form class="row g-3" @submit.prevent="submitContact">
                  <div class="col-md-6">
                    <label class="form-label ws-label">Full Name</label>
                    <input v-model="form.fullName" class="form-control" placeholder="Johnathan Smith" required />
                  </div>
                  <div class="col-md-6">
                    <label class="form-label ws-label">Work Email</label>
                    <input v-model="form.email" type="email" class="form-control" placeholder="john@company.com" required />
                  </div>
                  <div class="col-12">
                    <label class="form-label ws-label">Project Type</label>
                    <select v-model="form.projectType" class="form-select">
                      <option>E-commerce Migration</option>
                      <option>Custom Enterprise Framework</option>
                      <option>Global Speed Optimization</option>
                      <option>White-Glove Managed Support</option>
                    </select>
                  </div>
                  <div class="col-12">
                    <label class="form-label ws-label">Project Details</label>
                    <textarea v-model="form.details" rows="4" class="form-control" placeholder="Briefly describe your current technical challenges..." />
                  </div>
                  <div class="col-12">
                    <button class="btn ws-btn-danger w-100" type="submit">
                      Initiate Discovery Session
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="ws-footer">
      <div class="container py-5">
        <div class="row g-4 pb-4">
          <div class="col-lg-4">
            <div class="d-flex align-items-center gap-2 mb-3">
              <span class="ws-brand-badge ws-brand-badge-sm">
                <i class="bi bi-shield-check text-white" />
              </span>
              <span class="ws-footer-brand">Winter<span class="ws-text-danger">Snow</span></span>
            </div>
            <div class="text-secondary small">
              The marketplace scaffold for high-performance commerce experiences — storefront, vendor portal, and admin governance.
            </div>
          </div>
          <div class="col-6 col-lg-2">
            <div class="ws-footer-title">Product</div>
            <ul class="list-unstyled small mb-0 ws-footer-links">
              <li><a href="#solutions">Fast Checkout</a></li>
              <li><a href="#solutions">Search & Facets</a></li>
              <li><a href="#solutions">SEO Slugs</a></li>
            </ul>
          </div>
          <div class="col-6 col-lg-2">
            <div class="ws-footer-title">Service</div>
            <ul class="list-unstyled small mb-0 ws-footer-links">
              <li><a href="#contact">Managed Support</a></li>
              <li><a href="#contact">Custom Dev</a></li>
              <li><a href="#contact">Audit & CRO</a></li>
            </ul>
          </div>
          <div class="col-6 col-lg-2">
            <div class="ws-footer-title">Resources</div>
            <ul class="list-unstyled small mb-0 ws-footer-links">
              <li><a href="/docs/WinterSnow-User-Guide">User Guide</a></li>
              <li><a href="/docs/WinterSnow-Product-Documentation">Product Docs</a></li>
              <li><a href="/docs/Shopify-Parity-Roadmap">Roadmap</a></li>
            </ul>
          </div>
          <div class="col-6 col-lg-2">
            <div class="ws-footer-title">Trust</div>
            <ul class="list-unstyled small mb-0 ws-footer-links">
              <li><a href="#contact">Privacy</a></li>
              <li><a href="#contact">Security</a></li>
              <li><a href="#contact">Terms</a></li>
            </ul>
          </div>
        </div>

        <div class="d-flex flex-column flex-md-row gap-2 justify-content-between pt-3 ws-footer-bottom">
          <div>© {{ year }} WinterSnow. All Rights Reserved.</div>
          <div class="d-flex gap-3">
            <span>99.9% Uptime SLA</span>
            <span>GDPR Compliant</span>
          </div>
        </div>
      </div>
    </footer>

    <!-- Floating Chat Widget -->
    <div class="ws-chat-wrap">
      <div class="ws-chat-window" :class="{ active: chatOpen }" role="dialog" aria-label="Chat with an architect">
        <div class="ws-chat-head">
          <div class="d-flex align-items-center gap-2">
            <div class="ws-chat-avatar">
              <span class="fw-bold small">WS</span>
              <span class="ws-chat-online" aria-hidden="true" />
            </div>
            <div>
              <div class="fw-bold small">WinterSnow Concierge</div>
              <div class="ws-chat-sub">Average wait: 1 min</div>
            </div>
          </div>
          <button class="btn btn-sm ws-btn-icon" type="button" @click="toggleChat(false)" aria-label="Close chat">
            <i class="bi bi-x-lg" />
          </button>
        </div>

        <div class="ws-chat-body">
          <div class="d-flex gap-2 align-items-start">
            <div class="ws-chat-bubble-ic">AI</div>
            <div class="ws-chat-bubble">
              Welcome to WinterSnow. Would you like a tour of the platform or to speak with an architect?
            </div>
          </div>
          <div class="d-flex gap-2 align-items-start justify-content-end mt-3">
            <div class="ws-chat-bubble ws-chat-bubble-me">
              I’d like to schedule a store audit.
            </div>
          </div>
        </div>

        <div class="ws-chat-foot">
          <input class="form-control" placeholder="Type your inquiry..." />
          <button class="btn ws-btn-dark" type="button" aria-label="Send">
            <i class="bi bi-send" />
          </button>
        </div>
      </div>

      <button class="btn ws-chat-toggle" type="button" @click="toggleChat()" aria-expanded="chatOpen">
        <span class="fw-bold small">Chat with an Architect</span>
        <span class="ws-chat-ic"><i class="bi bi-chat-dots" /></span>
      </button>
    </div>
  </div>
</template>

<style scoped>
.ws-landing {
  overflow-x: hidden;
}

.ws-nav-glass {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  background: color-mix(in srgb, var(--bs-body-bg) 85%, transparent);
  backdrop-filter: blur(16px);
  border-bottom: 1px solid color-mix(in srgb, var(--bs-border-color) 40%, transparent);
  transition: box-shadow 200ms ease;
}
.ws-nav-shadow {
  box-shadow: 0 20px 40px -30px rgba(0, 0, 0, 0.45);
}
.ws-brand-badge {
  width: 36px;
  height: 36px;
  border-radius: 0.75rem;
  background: color-mix(in srgb, var(--bs-primary) 75%, #000);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 40%, transparent);
}
.ws-brand-badge-sm {
  width: 28px;
  height: 28px;
  border-radius: 0.6rem;
}
.ws-brand {
  font-weight: 900;
  letter-spacing: -0.04em;
  color: var(--bs-body-color);
  font-size: 1.25rem;
}
.ws-brand-accent {
  color: var(--ws-danger);
  font-style: italic;
  text-transform: uppercase;
}
.ws-nav-links {
  font-size: 0.75rem;
  font-weight: 800;
  color: var(--bs-secondary-color);
  text-transform: uppercase;
  letter-spacing: 0.16em;
}
.ws-nav-link {
  text-decoration: none;
  color: inherit;
}
.ws-nav-link:hover {
  color: var(--bs-primary);
}

.ws-hero {
  padding-top: 7.5rem;
  padding-bottom: 4.5rem;
  background: color-mix(in srgb, var(--bs-body-bg) 70%, #fff);
  position: relative;
}
.ws-hero-glow {
  position: absolute;
  pointer-events: none;
  opacity: 0.22;
  filter: blur(110px);
  border-radius: 999px;
}
.ws-hero-glow-blue {
  width: 560px;
  height: 560px;
  right: -180px;
  top: 60px;
  background: color-mix(in srgb, var(--bs-primary) 70%, #60a5fa);
}
.ws-hero-glow-red {
  width: 420px;
  height: 420px;
  left: -160px;
  bottom: -160px;
  background: color-mix(in srgb, var(--ws-danger) 70%, #fb7185);
}

.ws-pill {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.35rem 0.8rem;
  border-radius: 999px;
  border: 1px solid color-mix(in srgb, var(--bs-primary) 20%, var(--bs-border-color));
  background: color-mix(in srgb, var(--bs-primary) 8%, transparent);
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.22em;
  color: color-mix(in srgb, var(--bs-primary) 65%, var(--bs-body-color));
}
.ws-dot {
  width: 10px;
  height: 10px;
  border-radius: 999px;
  background: var(--bs-primary);
  box-shadow: 0 0 0 6px color-mix(in srgb, var(--bs-primary) 18%, transparent);
}

.ws-hero-title {
  font-weight: 900;
  letter-spacing: -0.04em;
  line-height: 0.98;
  font-size: clamp(2.4rem, 5vw, 4.25rem);
  color: var(--bs-body-color);
}
.ws-hero-sub {
  font-size: 1.1rem;
  color: var(--bs-secondary-color);
  max-width: 44rem;
}
.ws-text-brand {
  color: color-mix(in srgb, var(--bs-primary) 78%, #000);
}

.ws-btn-dark {
  background: var(--bs-body-color);
  color: var(--bs-body-bg);
  border: 2px solid var(--bs-body-color);
  padding: 0.95rem 1.25rem;
  border-radius: 1rem;
  font-weight: 800;
}
.ws-btn-dark:hover {
  background: var(--bs-primary);
  border-color: var(--bs-primary);
  color: #fff;
}
.ws-btn-outline {
  background: transparent;
  color: var(--bs-body-color);
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 55%, transparent);
  padding: 0.95rem 1.25rem;
  border-radius: 1rem;
  font-weight: 800;
}
.ws-btn-outline:hover {
  border-color: var(--ws-danger);
}

.ws-btn-danger {
  background: var(--ws-danger);
  color: #fff;
  border: 2px solid var(--ws-danger);
  padding: 0.7rem 1rem;
  border-radius: 0.9rem;
  font-weight: 800;
}
.ws-btn-danger:hover {
  filter: brightness(1.05);
}

.ws-microproof .ws-chip {
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 50%, transparent);
  background: color-mix(in srgb, var(--bs-body-bg) 80%, #fff);
  padding: 0.35rem 0.75rem;
  border-radius: 999px;
  font-size: 0.85rem;
  font-weight: 700;
  color: var(--bs-body-color);
}

.ws-hero-card {
  border-radius: 1.5rem;
  padding: 0.75rem;
  background: color-mix(in srgb, var(--bs-card-bg) 90%, transparent);
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 45%, transparent);
}
.ws-hero-img {
  width: 100%;
  border-radius: 1.1rem;
  display: block;
  object-fit: cover;
  aspect-ratio: 4 / 3;
}
.ws-stat {
  position: absolute;
  right: -10px;
  top: -18px;
  transform: rotate(5deg);
  background: var(--ws-danger);
  color: #fff;
  border-radius: 1.25rem;
  padding: 1.1rem 1.25rem;
  border: 2px solid color-mix(in srgb, #fff 35%, transparent);
}
.ws-stat-kicker {
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.18em;
  opacity: 0.8;
}
.ws-stat-value {
  font-size: 2.25rem;
  font-weight: 900;
  line-height: 1;
}
.ws-stat-foot {
  font-size: 0.7rem;
  font-weight: 800;
  opacity: 0.9;
}

.py-lg-6 {
  padding-top: 5rem !important;
  padding-bottom: 5rem !important;
}
.ws-alt {
  background: color-mix(in srgb, var(--bs-body-bg) 90%, #fff);
}
.ws-section-kicker {
  color: var(--ws-danger);
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.3em;
  font-size: 0.75rem;
  margin-bottom: 0.5rem;
}
.ws-section-title {
  font-weight: 900;
  letter-spacing: -0.03em;
}
.ws-rounded {
  border-radius: 1.5rem;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 45%, transparent);
}
.ws-shadow-lite {
  box-shadow: 0 18px 36px -26px rgba(0, 0, 0, 0.5);
}
.ws-badge-years {
  position: absolute;
  inset: 0;
  margin: auto;
  width: 170px;
  height: 170px;
  border-radius: 999px;
  background: color-mix(in srgb, var(--bs-primary) 75%, #000);
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  border: 6px solid color-mix(in srgb, var(--bs-card-bg) 85%, transparent);
}
.ws-badge-years-value {
  font-size: 2rem;
  font-weight: 900;
  line-height: 1;
}
.ws-badge-years-label {
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.16em;
  opacity: 0.9;
}

.ws-icon-tile {
  width: 38px;
  height: 38px;
  border-radius: 0.75rem;
  background: color-mix(in srgb, var(--bs-primary) 10%, transparent);
  border: 2px solid color-mix(in srgb, var(--bs-primary) 20%, var(--bs-border-color));
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: var(--bs-primary);
  flex: 0 0 auto;
}
.ws-link {
  color: var(--bs-primary);
  font-weight: 800;
  text-decoration: none;
  border-bottom: 2px solid color-mix(in srgb, var(--bs-primary) 20%, transparent);
  padding-bottom: 2px;
}
.ws-link:hover {
  border-bottom-color: var(--ws-danger);
}

.ws-card {
  border-radius: 1.5rem;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 50%, transparent);
  background: var(--bs-card-bg);
  transition: transform 200ms ease, box-shadow 200ms ease;
}
.ws-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 24px 48px -34px rgba(0, 0, 0, 0.55);
}
.ws-feature-ic {
  width: 38px;
  height: 38px;
  border-radius: 1rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 50%, transparent);
  background: color-mix(in srgb, var(--bs-body-bg) 80%, #fff);
}

.ws-review {
  border-radius: 1.75rem;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 45%, transparent);
}
.ws-review-pop {
  transform: translateY(14px);
}
.ws-stars {
  color: #f59e0b;
}
.ws-avatar {
  width: 44px;
  height: 44px;
  border-radius: 999px;
  object-fit: cover;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 45%, transparent);
}
.ws-meta {
  font-size: 0.65rem;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 0.18em;
}
.ws-text-danger {
  color: var(--ws-danger);
}

.ws-contact {
  background: color-mix(in srgb, var(--bs-body-bg) 85%, #fff);
}
.ws-contact-shell {
  position: relative;
  border-radius: 2rem;
  background: color-mix(in srgb, var(--bs-primary) 25%, #0b1b3a);
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 35%, transparent);
  overflow: hidden;
  padding: 2rem;
}
.ws-contact-glow {
  position: absolute;
  right: -120px;
  top: -120px;
  width: 420px;
  height: 420px;
  border-radius: 999px;
  background: color-mix(in srgb, var(--ws-danger) 20%, transparent);
  filter: blur(60px);
  pointer-events: none;
}
.ws-contact-title {
  font-weight: 900;
  letter-spacing: -0.03em;
  color: #fff;
  font-size: clamp(2rem, 3vw, 3rem);
}
.ws-contact-sub {
  color: rgba(255, 255, 255, 0.8);
}
.ws-contact-ic {
  width: 44px;
  height: 44px;
  border-radius: 1rem;
  background: rgba(255, 255, 255, 0.12);
  border: 2px solid rgba(255, 255, 255, 0.15);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  color: var(--ws-danger);
  flex: 0 0 auto;
}
.ws-contact-kicker {
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.22em;
  color: rgba(255, 255, 255, 0.65);
}
.ws-contact-value {
  font-weight: 900;
  color: #fff;
}

.ws-form {
  border-radius: 1.5rem;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 40%, transparent);
}
.ws-label {
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.18em;
  color: color-mix(in srgb, var(--bs-body-color) 55%, var(--bs-secondary-color));
}

.ws-footer {
  background: color-mix(in srgb, var(--bs-body-color) 92%, #000);
  color: #fff;
}
.ws-footer-brand {
  font-weight: 900;
  letter-spacing: -0.03em;
  font-size: 1.25rem;
}
.ws-footer-title {
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.28em;
  color: color-mix(in srgb, var(--bs-primary) 65%, #fff);
  margin-bottom: 0.75rem;
}
.ws-footer-links a {
  color: rgba(255, 255, 255, 0.65);
  text-decoration: none;
}
.ws-footer-links a:hover {
  color: #fff;
}
.ws-footer-bottom {
  border-top: 1px solid rgba(255, 255, 255, 0.12);
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.2em;
  color: rgba(255, 255, 255, 0.6);
}

.ws-chat-wrap {
  position: fixed;
  right: 1rem;
  bottom: 1rem;
  z-index: 1100;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.75rem;
}
.ws-chat-window {
  width: min(420px, calc(100vw - 2rem));
  border-radius: 1.5rem;
  overflow: hidden;
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 45%, transparent);
  background: var(--bs-card-bg);
  transform: translateY(30px) scale(0.96);
  opacity: 0;
  pointer-events: none;
  transition: all 260ms cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.ws-chat-window.active {
  transform: translateY(0) scale(1);
  opacity: 1;
  pointer-events: all;
}
.ws-chat-head {
  background: color-mix(in srgb, var(--bs-primary) 85%, #000);
  color: #fff;
  padding: 1rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.ws-chat-avatar {
  position: relative;
  width: 38px;
  height: 38px;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.16);
  border: 2px solid rgba(255, 255, 255, 0.35);
  display: inline-flex;
  align-items: center;
  justify-content: center;
}
.ws-chat-online {
  position: absolute;
  width: 10px;
  height: 10px;
  border-radius: 999px;
  background: #22c55e;
  border: 2px solid color-mix(in srgb, var(--bs-primary) 85%, #000);
  right: -2px;
  bottom: -2px;
}
.ws-chat-sub {
  font-size: 0.65rem;
  font-weight: 900;
  text-transform: uppercase;
  letter-spacing: 0.18em;
  opacity: 0.8;
}
.ws-chat-body {
  background: color-mix(in srgb, var(--bs-body-bg) 92%, #fff);
  padding: 1rem;
  max-height: 320px;
  overflow: auto;
}
.ws-chat-bubble-ic {
  width: 28px;
  height: 28px;
  border-radius: 999px;
  background: color-mix(in srgb, var(--bs-primary) 12%, transparent);
  border: 2px solid color-mix(in srgb, var(--bs-primary) 20%, var(--bs-border-color));
  color: var(--bs-primary);
  font-weight: 900;
  font-size: 0.65rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex: 0 0 auto;
}
.ws-chat-bubble {
  border: 2px solid color-mix(in srgb, var(--bs-border-color) 50%, transparent);
  background: var(--bs-card-bg);
  padding: 0.6rem 0.8rem;
  border-radius: 1rem;
  border-top-left-radius: 0.25rem;
  font-size: 0.9rem;
  color: var(--bs-body-color);
  max-width: 85%;
}
.ws-chat-bubble-me {
  background: color-mix(in srgb, var(--bs-primary) 82%, #000);
  color: #fff;
  border-color: color-mix(in srgb, var(--bs-primary) 82%, #000);
  border-top-left-radius: 1rem;
  border-top-right-radius: 0.25rem;
}
.ws-chat-foot {
  display: flex;
  gap: 0.5rem;
  padding: 0.75rem;
  border-top: 1px solid color-mix(in srgb, var(--bs-border-color) 45%, transparent);
  background: var(--bs-card-bg);
}
.ws-btn-icon {
  border: 2px solid rgba(255, 255, 255, 0.22);
  color: #fff;
}
.ws-btn-icon:hover {
  background: rgba(255, 255, 255, 0.12);
}
.ws-chat-toggle {
  background: var(--ws-danger);
  color: #fff;
  border: 2px solid var(--ws-danger);
  border-radius: 1.1rem;
  padding: 0.8rem 1rem;
  display: inline-flex;
  align-items: center;
  gap: 0.75rem;
  box-shadow: 0 24px 48px -34px rgba(0, 0, 0, 0.65);
}
.ws-chat-toggle:hover {
  filter: brightness(1.05);
}
.ws-chat-ic {
  width: 36px;
  height: 36px;
  border-radius: 0.9rem;
  background: rgba(255, 255, 255, 0.18);
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

@media (max-width: 991.98px) {
  .ws-review-pop {
    transform: none;
  }
  .ws-badge-years {
    display: none;
  }
}
</style>

<style>
:root {
  --ws-danger: #ef4444;
}
</style>
