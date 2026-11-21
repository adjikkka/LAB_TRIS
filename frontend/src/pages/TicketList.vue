<script setup>
import { ref, onMounted } from 'vue'
import { getTickets } from '../api/tickets.js'
import { items, setItems } from '../ticketsStore.js'
import TicketTable from '../components/TicketTable.vue'

// Четыре состояния экрана списка (ЛР6): loading | empty | data | error.
// Одновременно показывается только одно — взаимоисключающие v-if / v-else-if / v-else.
const screen = ref('loading')
const errorMessage = ref('')

async function load() {
  screen.value = 'loading'
  errorMessage.value = ''
  try {
    const list = await getTickets()
    setItems(list)
    screen.value = items.value.length === 0 ? 'empty' : 'data'
  } catch (error) {
    errorMessage.value = error.message
    screen.value = 'error'
  }
}

onMounted(load)
</script>

<template>
  <main class="page">
    <h1>Обращения жильцов</h1>

    <p v-if="screen === 'loading'">Загрузка…</p>

    <div v-else-if="screen === 'empty'" class="notice">
      <p>Обращений пока нет. Нажмите «Создать», чтобы добавить первое.</p>
      <router-link to="/new">Создать обращение</router-link>
    </div>

    <TicketTable v-else-if="screen === 'data'" :items="items" />

    <div v-else class="notice notice-error" role="alert">
      <p>{{ errorMessage }}</p>
      <button type="button" @click="load">Повторить загрузку</button>
    </div>
  </main>
</template>
