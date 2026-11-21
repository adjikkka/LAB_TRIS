// Mock сервера (ЛР6): имитирует ответы API, пока сервер не подключён
// или для демонстрации без backend. Повторяет контракт ответа:
// при успехе { ok: true, status: 201, data: { id, number, status: 'New', ... } },
// при отказе { ok: false, status: 400 | 500, data: { title } }.
// id и number создаёт mock, а не экран.

const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms))

// Учебный флаг для проверки ветки 500 (ЛР6, шаг G).
// true — mock вернёт 500 на любой запрос.
export const force500 = { value: false }

// Учебные данные. Статусы только из набора курса.
const mockItems = [
  {
    id: 1,
    number: 'ОБР-2026-001',
    title: 'Протекает кран в ванной',
    description: 'Капает кран, лужа под раковиной',
    status: 'New',
    roomId: 305,
    createdByUserId: 1,
    assigneeUserId: null,
    createdAt: '2026-11-02T10:15:00Z'
  },
  {
    id: 2,
    number: 'ОБР-2026-002',
    title: 'Не работает розетка',
    description: 'Розетка у стола не даёт питание',
    status: 'InProgress',
    roomId: 203,
    createdByUserId: 1,
    assigneeUserId: 3,
    createdAt: '2026-11-03T12:40:00Z'
  },
  {
    id: 3,
    number: 'ОБР-2026-003',
    title: 'Скрипит дверь в душевой',
    description: 'Дверь душевой на 4 этаже скрипит',
    status: 'Closed',
    roomId: 410,
    createdByUserId: 1,
    assigneeUserId: 2,
    createdAt: '2026-10-28T09:00:00Z'
  }
]

let nextId = 4

function makeNumber(id) {
  return `ОБР-2026-${String(id).padStart(3, '0')}`
}

export async function mockGetTickets() {
  await delay(500)
  if (force500.value) {
    return { ok: false, status: 500, data: { title: 'Учебная ошибка 500 (force500)' } }
  }
  return { ok: true, status: 200, data: [...mockItems] }
}

export async function mockCreateTicket(body) {
  await delay(500)
  if (force500.value) {
    return { ok: false, status: 500, data: { title: 'Учебная ошибка 500 (force500)' } }
  }

  const title = String(body?.title ?? '').trim()
  if (title.length < 5 || title.length > 80) {
    return { ok: false, status: 400, data: { title: 'Тема: от 5 до 80 символов' } }
  }

  const roomId = body?.roomId
  if (!Number.isInteger(roomId) || roomId < 1) {
    return { ok: false, status: 400, data: { title: 'Комната: целое число больше нуля' } }
  }

  const description = String(body?.description ?? '').trim()
  if (description.length < 10 || description.length > 500) {
    return { ok: false, status: 400, data: { title: 'Описание: от 10 до 500 символов' } }
  }

  const created = {
    id: nextId,
    number: makeNumber(nextId),
    title,
    description,
    status: 'New',
    roomId,
    createdByUserId: 1,
    assigneeUserId: null,
    createdAt: new Date().toISOString()
  }
  nextId += 1

  return { ok: true, status: 201, data: created }
}
