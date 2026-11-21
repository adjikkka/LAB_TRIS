<script setup>
// Таблица только рисует то, что ей передали через props (ЛР4).
// Собственного массива нет, копию props не создаём.
defineProps({
  items: { type: Array, required: true }
})

const statusLabels = {
  New: 'Новое',
  InProgress: 'В работе',
  Closed: 'Выполнено',
  Cancelled: 'Отменено'
}
</script>

<template>
  <table class="ticket-table">
    <thead>
      <tr>
        <th>Номер</th>
        <th>Тема</th>
        <th>Комната</th>
        <th>Статус</th>
        <th>Исполнитель</th>
        <th>Создано</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="item in items" :key="item.id">
        <td>{{ item.number }}</td>
        <td>{{ item.title }}</td>
        <td>{{ item.roomId }}</td>
        <td>{{ statusLabels[item.status] || item.status }}</td>
        <td>{{ item.assigneeUserId ?? '—' }}</td>
        <td>{{ item.createdAt }}</td>
      </tr>
    </tbody>
  </table>
</template>
