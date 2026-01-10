# Ecomm-WinterSnow (Vue 3 + .NET 8)

This repository is a starter **multi-vendor eCommerce platform** with:

- **Frontend**: Vue 3 (JavaScript) + Pinia + Vue Router (SPA)
- **Backend**: ASP.NET Core 8 Web API using a **nopCommerce-style layered architecture** (`Core` / `Data` / `Services` / `WebApi`)

It includes the first three UI modules:

- **Consumer Storefront**
- **Vendor Dashboard**
- **Admin & System Panel**

And backend coverage for the requested strategies (IAM, catalog, discovery/search, split-orders, payments stub, ledger/payouts, inventory guard, UGC reviews, marketing tools, BI/health snapshots).

## Run backend (ASP.NET Core 8)

From repo root:

```bash
export DOTNET_ROOT=/home/ubuntu/.dotnet
export PATH=/home/ubuntu/.dotnet:$PATH
dotnet run --project backend/src/WinterSnow.WebApi
```

By default it runs on `http://localhost:5009` (see `backend/src/WinterSnow.WebApi/Properties/launchSettings.json`).

## Run frontend (Vue 3 + Pinia)

```bash
cd frontend
cp .env.example .env
npm install
npm run dev
```

Frontend expects the API at `VITE_API_BASE_URL` (default in `.env.example` is `http://localhost:5009`).

## Demo logins

- **Admin**: `admin@demo.local` / `Admin123!`
- **Vendor**: `vendor@demo.local` / `Vendor123!`
- **Customer**: `customer@demo.local` / `Customer123!`

## Key routes

- **Storefront**: `/`, `/search`, `/p/:slug`, `/checkout`
- **Vendor**: `/vendor`, `/vendor/inventory`, `/vendor/orders`, `/vendor/payouts`
- **Admin**: `/admin`, `/admin/finance`, `/admin/system`, `/admin/marketing`

## Seeded demo product

- **Product**: `Senior Software Developer`
- **Slug**: `senior-software-developer`
