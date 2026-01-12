import { createRouter as _createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import LandingPage from '../views/LandingPage.vue'
import StorefrontHome from '../views/storefront/StorefrontHome.vue'
import StorefrontSearch from '../views/storefront/StorefrontSearch.vue'
import ProductDetails from '../views/storefront/ProductDetails.vue'
import Checkout from '../views/storefront/Checkout.vue'

import Login from '../views/auth/Login.vue'
import RoleLogin from '../views/auth/RoleLogin.vue'

import AccountHome from '../views/account/AccountHome.vue'
import MyOrders from '../views/account/MyOrders.vue'
import OrderDetails from '../views/account/OrderDetails.vue'
import MyReturns from '../views/account/MyReturns.vue'

import VendorLayout from '../layouts/VendorLayout.vue'
import VendorDashboard from '../views/vendor/VendorDashboard.vue'
import VendorInventory from '../views/vendor/VendorInventory.vue'
import VendorOrders from '../views/vendor/VendorOrders.vue'
import VendorPayouts from '../views/vendor/VendorPayouts.vue'
import VendorCreateListing from '../views/vendor/VendorCreateListing.vue'

import AdminLayout from '../layouts/AdminLayout.vue'
import AdminModeration from '../views/admin/AdminModeration.vue'
import AdminFinance from '../views/admin/AdminFinance.vue'
import AdminSystemHealth from '../views/admin/AdminSystemHealth.vue'
import AdminMarketing from '../views/admin/AdminMarketing.vue'

export function createRouter() {
  const router = _createRouter({
    history: createWebHistory(),
    routes: [
      { path: '/', name: 'landing', component: LandingPage },
      { path: '/store', name: 'home', component: StorefrontHome },
      { path: '/search', name: 'search', component: StorefrontSearch },
      { path: '/p/:slug', name: 'product', component: ProductDetails, props: true },
      { path: '/checkout', name: 'checkout', component: Checkout },

      { path: '/login', name: 'login', component: Login },
      { path: '/login/customer', name: 'login.customer', component: RoleLogin, meta: { loginRole: 'Customer' } },
      { path: '/login/vendor', name: 'login.vendor', component: RoleLogin, meta: { loginRole: 'Vendor' } },
      { path: '/login/admin', name: 'login.admin', component: RoleLogin, meta: { loginRole: 'Admin' } },

      { path: '/account', name: 'account', component: AccountHome, meta: { requiresAuth: true, role: 'Customer' } },
      { path: '/account/orders', name: 'account.orders', component: MyOrders, meta: { requiresAuth: true, role: 'Customer' } },
      { path: '/account/orders/:orderId', name: 'account.orders.details', component: OrderDetails, meta: { requiresAuth: true, role: 'Customer' } },
      { path: '/account/returns', name: 'account.returns', component: MyReturns, meta: { requiresAuth: true, role: 'Customer' } },

      {
        path: '/vendor',
        component: VendorLayout,
        meta: { requiresAuth: true, role: 'Vendor' },
        children: [
          { path: '', name: 'vendor.dashboard', component: VendorDashboard },
          { path: 'listings/new', name: 'vendor.listings.new', component: VendorCreateListing },
          { path: 'inventory', name: 'vendor.inventory', component: VendorInventory },
          { path: 'orders', name: 'vendor.orders', component: VendorOrders },
          { path: 'payouts', name: 'vendor.payouts', component: VendorPayouts }
        ]
      },

      {
        path: '/admin',
        component: AdminLayout,
        meta: { requiresAuth: true, role: 'Admin' },
        children: [
          { path: '', name: 'admin.moderation', component: AdminModeration },
          { path: 'finance', name: 'admin.finance', component: AdminFinance },
          { path: 'system', name: 'admin.system', component: AdminSystemHealth },
          { path: 'marketing', name: 'admin.marketing', component: AdminMarketing }
        ]
      }
    ]
  })

  router.beforeEach((to) => {
    const auth = useAuthStore()
    if (to.meta?.requiresAuth && !auth.isAuthenticated) {
      return { name: 'login', query: { redirect: to.fullPath } }
    }
    if (to.meta?.role && auth.role !== to.meta.role) {
      return { name: 'home' }
    }
    return true
  })

  return router
}

