import { useAuthStore } from '../stores/auth'

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000'

export async function apiGet(path) {
  return request(path, { method: 'GET' })
}

export async function apiPost(path, body) {
  return request(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined })
}

export async function apiPut(path, body) {
  return request(path, { method: 'PUT', body: body ? JSON.stringify(body) : undefined })
}

export async function apiDelete(path) {
  return request(path, { method: 'DELETE' })
}

async function request(path, options) {
  const auth = useAuthStore()
  const headers = {
    'Content-Type': 'application/json',
    ...(options.headers || {})
  }
  if (auth?.accessToken) headers.Authorization = `Bearer ${auth.accessToken}`

  const res = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  })

  if (res.status === 204) return null

  const text = await res.text()
  const data = text ? safeJsonParse(text) : null

  if (!res.ok) {
    const msg = data?.message || data?.title || `Request failed (${res.status})`
    throw new Error(msg)
  }

  return data
}

function safeJsonParse(v) {
  try {
    return JSON.parse(v)
  } catch {
    return v
  }
}

