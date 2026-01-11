# WinterSnow vs Amazon — Feature Gap Checklist (What’s still pending)

WinterSnow is currently a **multi-vendor marketplace MVP scaffold**. Amazon is a hyper-mature ecosystem. Below is a practical checklist of **major Amazon-like capabilities** that are still missing or only partially implemented.

> This list is intentionally grouped by “customer experience”, “seller tools”, and “platform operations”.

---

## 1) Customer experience gaps (Amazon-like)

### Search & discovery
- Advanced ranking: personalization, sponsored placement, relevance tuning
- “Shop by category” mega navigation + curated landing pages at scale
- Saved searches, wishlists, and “buy again”
- Rich filters: brand, rating, delivery ETA, seller, condition, etc.

### Product detail page (PDP)
- Buy Box + “Other sellers on Amazon” list (multi-seller per ASIN style)
- Q&A section, product comparisons, rich attributes tables
- Frequently bought together, bundles, cross-sell
- Verified purchase enforcement + moderation tooling

### Checkout & delivery
- Real payment lifecycle: authorization/capture/settlement, retries, webhooks
- Address book, delivery instructions
- Shipping options by SLA (same-day/next-day), delivery ETA calculation
- Gift options, gift wrap, gift receipts

### Orders
- Customer account: order history, invoices, shipment tracking pages
- Partial shipments tracking + delivery proof
- Subscription/reorder flows

---

## 2) Seller (vendor) portal gaps (Amazon Seller Central-like)

### Onboarding & compliance
- Full vendor onboarding: signup + KYC document uploads + bank details + verification workflow
- Seller performance metrics + policy violations tracking

### Catalog/listings
- Category-driven attribute templates (“ASIN-like” structured catalog)
- Bulk import/export (CSV, feeds)
- Image/video management (real file uploads, CDN) and moderation
- Brand registry / trademark workflows

### Inventory & fulfillment
- Multi-warehouse inventory, inbound shipments, transfers
- Reservations/holds to prevent oversell during payment window (needs production-grade implementation)
- Seller shipping templates and label purchase
- Returns handling workflows (inspection, restocking)

### Advertising & promotions
- Sponsored products/brands, bid management, reporting
- Advanced promo engine: BOGO, tiered discounts, stacking rules

---

## 3) Platform/admin gaps (Amazon ops-like)

### Payments, settlement, and accounting
- Real gateway integrations (Cashfree, Razorpay, etc.) and reconciliation
- Refunds, chargebacks, dispute handling
- Vendor payout automation with bank transfer integrations
- Tax/GST invoice generation and reporting

### Fraud & trust
- Fraud/risk scoring, velocity checks
- Review abuse detection and moderation pipelines
- Audit trails UI and compliance exports

### Observability & reliability
- Background job system with retries (queue + DLQ) for payments/notifications/webhooks
- Rate limiting, DDOS protection, security hardening
- Real monitoring + alerting (APM, logs aggregation)

### Ecosystem
- Plugin/app marketplace model (extension points like Shopify/Amazon integrations)
- Analytics/BI: cohort, LTV, funnel, attribution, seller performance deep analytics

---

## 4) What WinterSnow already has that aligns (current)

- Marketplace split-orders (one cart → per-vendor orders)
- Listing lifecycle: Draft/Active/EOL + “visible in storefront” toggle
- Postcode-restricted visibility (only show listings for allowed postcodes)
- Vendor listing + inventory management (variants, stock)
- Admin approval workflow (vendor KYC boolean + product approval)
- Multi-gateway architecture (provider registry + vendor routing) **with stub gateways**
- Notifications module: in-app persisted + internal queue + UI bell dropdown

---

## 5) Recommended “Amazon-like” next steps (highest ROI)

1. Real payments + webhooks + reconciliation + inventory reservations
2. Full vendor onboarding (KYC docs + bank + verification)
3. Real media uploads (S3/local) + image processing + moderation
4. Shipping + tracking + returns logistics integration
5. Buy Box / other sellers per product (true multi-seller catalog)
6. Fraud/risk + customer account pages + operational dashboards

