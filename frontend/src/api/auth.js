// Аутентификация: POST /api/auth/login (ЛР11).
// Способ входа — учебный JWT в заголовке Authorization (выбран один способ).
import { requestJson } from './http.js'

export function loginRequest(login, password) {
  return requestJson('/api/auth/login', {
    method: 'POST',
    data: { login, password }
  })
}
