// API-модуль обращений (ЛР6). Страницы знают только getTickets и createTicket.
// Источник данных переключается в одном месте: VITE_USE_MOCK в .env
// (true — mock, false — настоящий fetch на VITE_API_BASE_URL).
import { requestJson, ApiError } from './http.js'
import { mockGetTickets, mockCreateTicket } from './mockTickets.js'
import { getToken } from '../services/auth.js'

const useMock = (import.meta.env.VITE_USE_MOCK ?? 'true') !== 'false'

export async function getTickets() {
  if (useMock) {
    const result = await mockGetTickets()
    if (!result.ok) {
      throw new ApiError(mockMessage(result.status, result.data?.title), result.status)
    }
    return result.data
  }
  return requestJson('/api/tickets', { token: getToken() })
}

export async function createTicket(body) {
  if (useMock) {
    const result = await mockCreateTicket(body)
    if (!result.ok) {
      throw new ApiError(mockMessage(result.status, result.data?.title), result.status)
    }
    return result.data
  }
  return requestJson('/api/tickets', { method: 'POST', data: body, token: getToken() })
}

// Тексты ошибок мока совпадают с текстами настоящего клиента (ЛР6, шаг G)
function mockMessage(status, title) {
  if (status === 400) {
    return `Проверьте поля формы. ${title ?? 'Отказ мока'}`
  }
  if (status === 500) {
    return 'Сервер не смог обработать запрос'
  }
  return title ?? 'Ошибка мока'
}
