import { ref } from 'vue'

// Единственное общее состояние списка обращений (ЛР4–ЛР5).
// Массив объявлен только здесь: список и форма импортируют один и тот же ref.
// ЛР6: данные больше не создаются на клиенте — список заполняется
// ответом API (getTickets) через setItems, id и number приходят из ответа.
export const items = ref([])

export function setItems(list) {
  items.value = list
}

// Учебная функция ЛР5 (запись в память браузера). После ЛР6 форма
// вызывает createTicket из src/api/tickets.js и список обновляется
// повторной загрузкой, поэтому addItem больше не используется на экранах.
export function addItem(ticket) {
  items.value.push(ticket)
}
