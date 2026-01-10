# WinterSnow Product Documentation

This document describes **WinterSnow**, a multi-vendor eCommerce platform with:

- **Frontend**: Vue 3 (JavaScript) SPA + Pinia + Vue Router
- **Backend**: ASP.NET Core 8 Web API using a **nopCommerce-style layered architecture** (`Core` / `Data` / `Services` / `WebApi`)

It is designed as a **headless marketplace**: the UI consumes REST APIs.

---

## Audience

- **Customers (Shoppers)**: browse, search, view product listings, and checkout.
- **Vendors (Sellers)**: create product listings, manage inventory, pricing, discounts, and monitor orders/payouts.
- **System Admins**: approve vendors/products, manage marketing tools, and monitor system health.

---

## Quick start (how to access the product)

### Run backend

```bash
export DOTNET_ROOT=/home/ubuntu/.dotnet
export PATH=/home/ubuntu/.dotnet:$PATH
dotnet run --project backend/src/WinterSnow.WebApi
```

Default URL: `http://localhost:5009`

### Run frontend

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

Default frontend URL (Vite): usually `http://localhost:5173`

### Demo accounts

- **Admin**: `admin@demo.local` / `Admin123!`
- **Vendor**: `vendor@demo.local` / `Vendor123!`
- **Customer**: `customer@demo.local` / `Customer123!`

---

## 1) Customer Documentation (Consumer Storefront)

### 1.1 Main pages

- **Homepage**: `/`
  - Sections: **Top Picks**, **New Arrivals**, **Winter Collections**
  - Only shows **approved + published + in-stock** listings

- **Search & discovery**: `/search`
  - Sticky search bar (top navigation) with **auto-suggestions**
  - **Faceted filtering**:
    - Material
    - Size
    - Color
    - Min/Max price
  - Search only returns listings that are:
    - Approved and published
    - In stock (at least one variant with stock)

- **Product details**: `/p/:slug`
  - Gallery (click-to-zoom)
  - Variant selection (size/color)
  - Review & UGC section (reviews + optional media URLs)
  - Shows **discount badges** and **coupon eligibility** when applicable

- **Checkout**: `/checkout`
  - **3-step checkout journey**
    1. Shipping address (with basic validation)
    2. Order summary (split by vendor)
    3. Payment session creation (Cashfree **stub**, see “Limitations”)

### 1.2 Pricing, discounts, and coupons (what shoppers see)

- **Vendor discount**:
  - Listings can include a **discount percent** and optional start/end schedule.
  - Storefront displays:
    - discounted price
    - original price (strikethrough)
    - discount badge (e.g., `-10%`)

- **Coupon eligibility**:
  - Each listing can be marked **AllowCoupons = true/false**
  - Storefront search/product pages show a **“Coupons”** badge when eligible.

- **Coupon code at checkout**:
  - In step 2 (Order Summary), customers can enter a **coupon code**.
  - Coupon discount is applied (basic implementation) and allocated across split vendor orders.

---

## 2) Vendor Documentation (Vendor Dashboard / Seller Cockpit)

### 2.1 Vendor access and navigation

After logging in as a vendor:

- **Vendor dashboard**: `/vendor`
- **Add listing**: `/vendor/listings/new`
- **Inventory manager**: `/vendor/inventory`
- **Order fulfillment list**: `/vendor/orders`
- **Payouts**: `/vendor/payouts`

### 2.2 Create product listings (eBay-style listings)

**Goal**: Vendors create their own sellable listings (inventory-backed) that customers can search and buy.

Use: **Vendor → Add Listing**

Listing supports:

- **Core listing data**:
  - Name + Slug
  - Short/Full description
  - Material (facet)
  - Category selection (basic taxonomy)

- **Pricing controls**:
  - Base price (INR)
  - Variant override price (optional)

- **Discount information**:
  - Discount percent (0–100)
  - Discount schedule start/end (UTC)

- **Coupon options**:
  - Allow coupons (true/false)

- **Variants / inventory matrix**:
  - Size, Color, SKU
  - Stock quantity per variant

- **Media**:
  - Media URLs (images) for the listing (URL-based placeholder for now)

**Important**: New listings are **not customer-visible** until approved by Admin:

- `Published = false`
- `IsApprovedByAdmin = false`

### 2.3 Update inventory and listing pricing

Use: **Vendor → Inventory Manager**

- **Bulk edit**:
  - Stock quantity per variant
  - Override prices per variant
  - Listing discount fields
  - Allow coupons toggle

### 2.4 Order fulfillment

Use: **Vendor → Orders**

- Filter by:
  - Awaiting Pickup
  - Shipped
  - Completed

### 2.5 Payouts & ledger

Use: **Vendor → Payouts**

- Shows a simplified:
  - Total sales (net)
  - Next payout date

> Note: Cashfree payout APIs are not yet integrated (see “Limitations”).

---

## 3) System Admin Documentation (Admin & System Panel)

### 3.1 Admin access and navigation

After logging in as Admin:

- **Moderation**: `/admin`
- **Finance hub**: `/admin/finance`
- **System health**: `/admin/system`
- **Marketing tools**: `/admin/marketing`

### 3.2 Vendor moderation (onboarding approval)

Admins can review and approve vendors:

- See pending vendors: `GET /api/admin/moderation/vendors`
- Approve vendor KYC: `POST /api/admin/moderation/vendors/{vendorId}/approve-kyc`

### 3.3 Product moderation (listing approval)

Admins can review and approve product submissions:

- See pending products: `GET /api/admin/moderation/products`
- Approve product to publish: `POST /api/admin/moderation/products/{productId}/approve`

When approved:

- `IsApprovedByAdmin = true`
- `Published = true`

### 3.4 Finance hub (commission overview)

Admin can view:

- Platform revenue (gross)
- Total commissions earned
- Orders count

Endpoint:

- `GET /api/admin/finance/summary`

### 3.5 System health monitoring

Admin panel shows:

- Uptime
- API response percentiles (p50/p95)
- Recent captured errors (from middleware)

Endpoint:

- `GET /api/admin/system/health`

### 3.6 Marketing tools

Admin can manage:

- Sitewide banners
- Coupon codes
- Flash sale events

Endpoints:

- `GET/POST /api/admin/marketing/banners`
- `GET/POST /api/admin/marketing/coupons`
- `GET/POST /api/admin/marketing/flash-sales`

---

## 4) API Summary (high-level)

### Auth

- `POST /api/auth/login`

Returns:

- `accessToken` (JWT)
- `role` (`Customer` | `Vendor` | `Admin`)
- `vendorId` (for vendor users)

### Storefront

- `GET /api/storefront/home`
- `GET /api/storefront/search/autocomplete?q=...`
- `GET /api/storefront/search?...facets...`
- `GET /api/storefront/products/{slug}`
- `POST /api/storefront/checkout/validate-address`
- `POST /api/storefront/checkout/preview` (legacy)
- `POST /api/storefront/checkout/preview-v2` (items + couponCode)
- `POST /api/storefront/checkout/submit` (customer auth required)

### Vendor

- `GET /api/vendor/dashboard/overview` (vendor auth)
- `GET /api/vendor/categories` (vendor auth)
- `POST /api/vendor/listings` (vendor auth)
- `PUT /api/vendor/listings/{productId}` (vendor auth)
- `GET /api/vendor/inventory` (vendor auth)
- `POST /api/vendor/inventory/bulk-update` (vendor auth)
- `GET /api/vendor/orders?status=...` (vendor auth)
- `GET /api/vendor/payouts` (vendor auth)

### Admin

- `GET /api/admin/moderation/vendors` (admin auth)
- `POST /api/admin/moderation/vendors/{vendorId}/approve-kyc` (admin auth)
- `GET /api/admin/moderation/products` (admin auth)
- `POST /api/admin/moderation/products/{productId}/approve` (admin auth)
- `GET /api/admin/finance/summary` (admin auth)
- `GET /api/admin/system/health` (admin auth)
- `GET/POST /api/admin/marketing/*` (admin auth)

---

## 5) Current limitations (important)

These are intentionally stubbed/simplified in this scaffold:

- **Cashfree integration**: payment gateway is a **stub** (fake “payment session id”). No webhooks/signature verification yet.
- **Media upload**: vendors provide **media URLs**; no file upload/storage.
- **Category taxonomy**: basic seed; no full attribute/schema per category (Amazon-style spec templates).
- **Inventory reservations**: stock is decremented during order creation in this scaffold; production should add reservation windows and payment webhooks.
- **Returns/claims/reverse logistics**: not implemented yet.
- **Notifications**: email/SMS not implemented yet.

---

## 6) Glossary

- **Listing**: A vendor’s sellable product listing (stock/price/discount/coupon eligibility). Implemented using `Product + ProductVariant` in this version.
- **Variant**: A size/color combination with its own stock and optional override price.
- **Split order**: One cart can create multiple orders (one per vendor).

