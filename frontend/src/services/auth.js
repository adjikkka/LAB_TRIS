// Состояние входа на клиенте (ЛР11). Токен и данные пользователя
// хранятся в localStorage: для учебного контура курса это допустимо.
import { loginRequest } from '../api/auth.js'

const TOKEN_KEY = 'auth_token'
const USER_KEY = 'auth_user'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function getCurrentUser() {
  const raw = localStorage.getItem(USER_KEY)
  if (!raw) return null
  try {
    return JSON.parse(raw)
  } catch {
    return null
  }
}

export async function loginUser(login, password) {
  const session = await loginRequest(login, password)
  localStorage.setItem(TOKEN_KEY, session.token)
  localStorage.setItem(USER_KEY, JSON.stringify(session.user))
  return session.user
}

export function logout() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(USER_KEY)
}
