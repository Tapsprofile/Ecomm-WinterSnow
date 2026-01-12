# WinterSnow — Detailed User Guide (How to Use the Application)

This guide explains **what features are currently implemented** in WinterSnow and **how to use** them as:

- **Customer (Storefront shopper)**
- **Vendor (Seller cockpit)**
- **System Admin (Platform command center)**

It also includes **how to run** the backend + frontend and the most important **API endpoints**.

---

## 1) What features are added (implemented today)

### 1.1 Core architecture
- **Backend**: ASP.NET Core 8 Web API in a nopCommerce-style layered structure:
  - `WinterSnow.Core` (domain/entities)
  - `WinterSnow.Data` (EF Core DbContext/repositories)
  - `WinterSnow.Services` (business services)
  - `WinterSnow.WebApi` (controllers/host)
- **Frontend**: Vue 3 (JavaScript) SPA + Pinia + Vue Router using **Bootstrap 5.3** styling.

### 1.2 Identity & access (IAM)
- **JWT login** with roles:
  - `Customer`, `Vendor`, `Admin`
- **Separate login pages**:
  - `/login/customer`, `/login/vendor`, `/login/admin`
- **Role-based routing protection** for Vendor/Admin sections.

### 1.3 Consumer Storefront (customer UI)
- Homepage sections:
  - Top Picks, New Arrivals, Winter Collections
- Search + discovery:
  - Autocomplete suggestions
  - Faceted filtering: size/color/material + min/max price
  - Only shows **approved + published + in-stock** listings
- Product details:
  - Gallery + zoom
  - Variants (size/color)
  - Reviews & UGC section (seeded demo reviews)
- Checkout:
  - 3-step flow: address validation → split order summary → payment session creation
  - Split orders (one cart → multiple vendor orders)
- Discounts:
  - Listing-level discount percent + start/end schedule
  - Displayed in search and product pages
- Coupons:
  - Admin creates coupons
  - Customer can apply coupon at checkout (basic allocation across split orders)

### 1.4 Vendor Dashboard (seller UI)
- Sales overview dashboard (basic)
- Inventory manager:
  - Bulk update stock and override prices (variant level)
  - Set listing discount fields + coupon eligibility
- Create product listing wizard:
  - Category selection (basic taxonomy)
  - Variants + inventory
  - Media URLs (URL-based placeholder)
  - Discounts + allow-coupons flag
- Orders list + basic status filtering
- Payouts view (placeholder totals)

### 1.5 Admin & System Panel
- Vendor moderation:
  - Approve vendor KYC (boolean approval)
- Product moderation:
  - Approve product/listing to go live
- Finance summary (gross revenue + commissions summary)
- System health:
  - Uptime, API latency p50/p95, recent errors
- Marketing tools:
  - Banners
  - Coupons
  - Flash sales
- Settings foundation:
  - Key/value settings API
- Webhook foundation:
  - Admin can register webhook subscriptions
- Audit log foundation:
  - API calls are recorded (basic audit entry)

### 1.6 Multi-gateway payments (architected for multiple providers)
- Payment providers:
  - `PaymentProvider` table (systemName/displayName/isActive/configJson)
  - `VendorPaymentProvider` mapping (vendor → provider)
- Checkout creates **provider-group payment sessions**:
  - If different vendors route to different providers, the checkout returns **multiple sessions**.
- Transactions are queryable by:
  - Transaction ID
  - PaymentGroupId
  - VendorId
  - ProviderId / ProviderSystemName
  - ProviderOrderId / ProviderPaymentId / ProviderPaymentSessionId
- Admin APIs to manage providers + mappings + query transactions
- Vendor API to query only their own transactions

> Note: Gateways are **stubs** right now (Cashfree/Razorpay session IDs are generated for demo). Real gateway integration requires server-side order creation + webhook verification.

### 1.7 Notifications (In-App + Email + SMS via internal queue)
- Notifications module with:
  - In-app persisted notifications
  - Provider abstraction for Email/SMS (currently stubs)
  - Internal queue provider (in-memory Channel-based queue)
  - Background worker that dispatches queued notifications
- UI bell dropdown (top navigation) for **customer and vendor**:
  - Unread count
  - Latest notifications list
  - Mark read / mark all read
- Triggers included:
  - Vendor listing submitted
  - Listing approved by admin
  - Customer order placed
  - Vendor new order

### 1.8 Returns / Claims (starter)
- Customer can create return request
- Vendor can approve/reject return request
- Label URL is placeholder (ready for real logistics integration)

---

## 2) How to run WinterSnow (backend + frontend)

### 2.1 Backend (ASP.NET Core 8)

From repo root:

```bash
dotnet run --project backend/src/WinterSnow.WebApi
```

Default backend URLs (from `launchSettings.json`):
- `http://localhost:5009`
- `https://localhost:7170`

#### Development DB reset

In development, the app is configured to reset the SQLite DB on startup:
- `backend/src/WinterSnow.WebApi/appsettings.Development.json` → `Dev:ResetDbOnStartup`

If you want to keep data between runs, set it to `false`.

### 2.2 Frontend (Vue 3 + Pinia)

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

Set API base URL in `.env`:
- `VITE_API_BASE_URL=http://localhost:5009`

---

## 3) Demo accounts and portals

### 3.1 Demo logins
- **Customer**: `customer@demo.local` / `Customer123!`
- **Vendor**: `vendor@demo.local` / `Vendor123!`
- **Admin**: `admin@demo.local` / `Admin123!`

### 3.2 Login pages
- `/login` (portal chooser)
- `/login/customer`
- `/login/vendor`
- `/login/admin`

---

## 4) Customer guide (Storefront)

### 4.1 Browse products
1. Go to `/`
2. Use the search bar or click **Browse**
3. Use `/search` filters (material/size/color/min/max price)

### 4.2 View a product
1. Click a product card
2. Select a variant
3. Click **Add to Cart**

### 4.3 Checkout
1. Go to `/checkout`
2. Step 1: enter shipping address → validate
3. Step 2: optionally enter a coupon code → load summary
4. Step 3: create payment session(s)

If checkout includes items from multiple vendors, the order is split.

### 4.4 Notifications (customer)
1. Login as Customer
2. Use the **bell icon** in the top navigation
3. You will see unread count, list, and can mark as read

### 4.5 Returns (customer)
Using APIs (UI portal not implemented yet):
- Create return: `POST /api/returns`
- View returns: `GET /api/returns/me`

---

## 5) Vendor guide (Seller cockpit)

### 5.1 Create a listing (inventory entry)
1. Login as Vendor → go to `/vendor`
2. Click **Add Listing**
3. Fill listing details:
   - Name, slug, descriptions, category
   - Base price
   - Allow coupons toggle
   - Discount percent + schedule
   - Variants with stock quantities
   - Media URLs
4. Submit

Result:
- Listing is **pending admin approval**
- Vendor receives an in-app notification (“Listing submitted”)

### 5.2 Manage inventory and discounts
1. Go to `/vendor/inventory`
2. Adjust:
   - Variant stock
   - Variant override prices
   - Discount fields
   - Allow coupons
3. Click **Bulk Save**

### 5.3 Orders
1. Go to `/vendor/orders`
2. Filter by status

### 5.4 Vendor notifications
Vendors receive notifications for:
- Listing submitted
- Listing approved (admin)
- New order placed

### 5.5 Vendor payment transactions (API)
Vendor-scoped transaction search:
- `GET /api/vendor/payments/transactions?...`

Common filters:
- `transactionId`
- `paymentGroupId`
- `providerSystemName`
- `providerPaymentSessionId`

---

## 6) Admin guide (System panel)

### 6.1 Approve vendors (KYC)
1. Login as Admin
2. Go to `/admin` → Vendor Moderation
3. Approve vendor

### 6.2 Approve products/listings
1. Login as Admin
2. Go to `/admin` → Product Submissions
3. Approve a product

Result:
- Listing goes live
- Vendor gets an in-app notification (“Listing approved”)

### 6.3 Marketing tools
Go to `/admin/marketing`:
- Create banners
- Create coupon codes
- Create flash sales

### 6.4 Payments administration (APIs)
Providers:
- `GET /api/admin/payments/providers`
- `POST /api/admin/payments/providers`
- `POST /api/admin/payments/providers/{providerId}/active`

Vendor routing:
- `GET /api/admin/payments/vendors/{vendorId}/providers`
- `POST /api/admin/payments/vendors/{vendorId}/providers`

Transaction search:
- `GET /api/admin/payments/transactions?...`

Search by:
- `transactionId`, `paymentGroupId`, `vendorId`, `orderId`
- `providerId`, `providerSystemName`
- `providerOrderId`, `providerPaymentId`, `providerPaymentSessionId`
- `status`

### 6.5 System health
Go to `/admin/system`:
- uptime, latency p50/p95, errors

### 6.6 Settings (API)
- `GET /api/admin/settings`
- `POST /api/admin/settings`

### 6.7 Webhooks (API)
- `GET /api/admin/webhooks`
- `POST /api/admin/webhooks`
- `POST /api/admin/webhooks/{id}/active`

---

## 7) Notifications API (reference)

For authenticated Customer/Vendor:
- `GET /api/notifications` (inbox list)
- `GET /api/notifications/unread-count`
- `POST /api/notifications/{id}/read`
- `POST /api/notifications/read-all`

---

## 8) Current limitations (what is not implemented yet)

- Real payment integration (Cashfree/Razorpay are currently stubs)
- Real Email/SMS sending (providers are currently stubs)
- External MQ integration (RabbitMQ/etc.) — internal queue is used by design for now
- Customer “My Account” pages (order history, returns portal UI)
- Full shipping/tax invoice pipelines
- Full returns logistics label integration

