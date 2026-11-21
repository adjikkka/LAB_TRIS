// Общая HTTP-часть клиента. Базовый адрес берётся только здесь,
// страницы URL не знают (ЛР6).
const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000'

export function getBaseUrl() {
  return baseUrl
}

// Ошибка API со статусом. status = 0 — ответа не было (сеть, адрес, сервер не запущен).
export class ApiError extends Error {
  constructor(message, status = 0) {
    super(message)
    this.name = 'ApiError'
    this.status = status
  }
}

// 400 и 500 НЕ попадают в catch: fetch считает ошибкой только отсутствие ответа.
// Различаем ветки !response.ok (сервер ответил) и catch (ответа нет).
export async function requestJson(path, { method = 'GET', data, token } = {}) {
  const headers = {}
  if (data !== undefined) {
    headers['Content-Type'] = 'application/json'
  }
  if (token) {
    headers['Authorization'] = `Bearer ${token}`
  }

  let response
  try {
    response = await fetch(`${baseUrl}${path}`, {
      method,
      headers,
      body: data !== undefined ? JSON.stringify(data) : undefined
    })
  } catch {
    throw new ApiError(
      `Нет связи с сервером. Проверьте, что API запущен по адресу ${baseUrl}`,
      0
    )
  }

  if (!response.ok) {
    let message = `Сервер ответил ошибкой ${response.status}`
    try {
      const problem = await response.json()
      if (problem && problem.title) {
        message = problem.title
      } else if (problem && problem.message) {
        message = problem.message
      }
    } catch {
      // тело не JSON — оставляем общий текст
    }

    if (response.status === 400) {
      message = `Проверьте поля формы. ${message}`
    } else if (response.status === 500) {
      message = 'Сервер не смог обработать запрос'
    } else if (response.status === 401) {
      message = 'Требуется вход'
    } else if (response.status === 403) {
      message = `Недостаточно прав. ${message}`
    }
    throw new ApiError(message, response.status)
  }

  if (response.status === 204) {
    return null
  }
  return response.json()
}
