# WinterSnow Shopify Parity Roadmap (Milestones + 12-Strategy Mapping)

This roadmap identifies what is still missing vs a Shopify-standard product and maps delivery to the **12 strategies** you listed.

> Important: Shopify is a mature platform; “parity” is a **multi-phase roadmap**. This document defines **MVP → V1 → V2** milestones that incrementally close the gap.

---

## Milestones (high level)

### MVP (Marketplace MVP + operational safety)
Goal: fully usable marketplace for customers/vendors/admins with safe inventory + real payments + approvals.

Deliverables:
- **Real Cashfree integration** (create order/session, payment verify webhook, refunds)
- **Inventory reservation** (hold stock during payment window; prevent oversells)
- **Vendor onboarding flow** (signup + KYC submission + admin approval)
- **Listing & catalog basics** (categories, attributes, media upload, vendor listing wizard)
- **Returns/claims v1** (customer request → vendor approve/reject → label placeholder)
- **Audit logs + settings + webhooks foundation**

### V1 (Shopify-like commerce engine depth)
Goal: operational parity for “standard ecommerce” features.

Deliverables:
- **Discount engine**: stacking rules, usage limits, min cart, exclusions, free shipping
- **Shipping**: zones, weight/dimensions, carrier integration + labels + tracking
- **Tax/GST invoices**: configurable taxes, invoice generation, exports
- **Customer accounts**: order history, returns portal, address book, password reset
- **Staff roles + permissions** (RBAC beyond 3 roles)
- **Automations**: abandoned checkout emails, low-stock alerts

### V2 (Ecosystem parity + extensibility)
Goal: Shopify ecosystem qualities (apps, analytics, themes).

Deliverables:
- **Plugin/app system** (event bus + extension points + marketplace apps)
- **Themes/page builder** (CMS, landing pages, blog)
- **BI dashboards** (funnels, cohorts, LTV, attribution, exports)
- **Multi-currency, multi-language**
- **Multi-location inventory** + warehouse ops

---

## 12 strategies → features mapping (what to build)

### 1) Identity & Access Management (IAM)
MVP:
- Vendor signup + customer signup
- Email verification + password reset
- RBAC policies for Admin/Staff/Vendor/Customer
- MFA framework (TOTP) for admin + vendor

V1:
- Staff roles with granular permissions
- Session/device management, login history

V2:
- SSO (OIDC), enterprise IAM options

### 2) Universal Product Catalog (UPC)
MVP:
- Category hierarchy + required attributes per category
- Product media upload abstraction (S3/local)
- SEO metadata, canonical URLs

V1:
- Collections (manual + rule-based)
- Metafields/custom attributes

V2:
- Recommendation/ranking engine hooks

### 3) Vendor Self-Service Portal
MVP:
- Vendor onboarding, profile, KYC
- Listing wizard (category → attributes → variants → inventory → media)
- Inventory & pricing bulk updates, CSV import/export

V1:
- Vendor shipping profiles + returns policies
- Performance insights dashboard

V2:
- Vendor CRM tools (messages, promos)

### 4) Headless Discovery & Search
MVP:
- Faceted search includes inventory + discounts + coupon eligibility
- Autocomplete + sorting

V1:
- AI ranking hooks, personalized results
- Search synonyms/typo tolerance

V2:
- Multi-index search (Elastic/OpenSearch)

### 5) Order Orchestration Module (OOM)
MVP:
- Split orders per vendor
- Reservation + payment success/failure transitions
- Email/SMS hooks (outbox + provider abstraction)

V1:
- Partial fulfillment + tracking per shipment
- Order editing/cancellations with policy enforcement

V2:
- Advanced routing (multi-warehouse, vendor SLA)

### 6) Finance & Payment Bridge (Cashfree)
MVP:
- Real Cashfree checkout session + server-side order create
- Webhook verification and final status update
- Refund API integration

V1:
- Settlement reconciliation reports

V2:
- Multi-provider payments abstraction

### 7) Vendor Ledger & Payouts
MVP:
- Commission rules per vendor/category
- Vendor ledger accuracy tied to payment settlement
- Cashfree payouts integration

V1:
- Scheduled payouts, payout failures handling, dispute holds

V2:
- Advanced accounting exports + integrations

### 8) Inventory & Stock Guard
MVP:
- Reservation holds + atomic stock decrement
- Low-stock alerts and hiding out-of-stock listings

V1:
- Multi-location inventory, transfers

V2:
- Forecasting & replenishment

### 9) Social Proof & UGC
MVP:
- Verified purchase reviews
- Photo/video upload (storage)

V1:
- Moderation tools, spam detection

V2:
- Creator/UGC campaigns

### 10) Reverse Logistics & Claims
MVP:
- Returns portal (customer) + approval workflow (vendor)
- Label generation integration (logistics provider)

V1:
- Exchanges, partial refunds, dispute resolution center

V2:
- Automated claims rules + fraud detection

### 11) Marketing & Promotions Engine
MVP:
- Coupon engine v2 (usage limits, min subtotal, allow list, exclude list)
- Flash sales and banners

V1:
- Bundles/BOGO/tiered discounts + free shipping

V2:
- Automations, segmentation, A/B testing

### 12) Admin Business Intelligence (BI)
MVP:
- Operational dashboards: orders, vendors, moderation queues
- Audit logs + exports basics

V1:
- GST/tax reports, vendor performance reports

V2:
- Funnel analytics, cohorts, attribution

---

## What’s already in WinterSnow (baseline)

- Headless Vue 3 UI modules: storefront + vendor + admin
- Vendor listing + variants + inventory + discount + allow-coupons flag
- Search facets (size/color/material) + in-stock filter + discount display
- Split orders per vendor
- Commission ledger entries (basic)
- Admin approvals (vendor KYC boolean + product publish approval)
- Marketing CRUD (banners/coupons/flash sales)
- **Foundations added for parity**: settings, audit logs, webhook subscriptions, returns/claims scaffolding

---

## Next implementation order (recommended)

1. **Cashfree real payment + webhook** + inventory reservation (prevents oversells)
2. **Vendor onboarding workflow** (KYC docs + admin approval)
3. **Shipping + taxes** (minimum viable) + invoices
4. **Returns label integration + refunds**
5. **RBAC (staff roles) + audit UI**
6. **Discount engine expansion + marketing automation**
7. **Plugin system + BI dashboards**

