<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { getCurrentUser, logout } from '../services/auth.js'

const router = useRouter()
const user = computed(() => getCurrentUser())

function onLogout() {
  logout()
  router.push('/login')
}
</script>

<template>
  <header class="app-header">
    <nav class="app-nav" aria-label="Основное меню">
      <router-link to="/">Список</router-link>
      <router-link to="/new">Создать</router-link>
      <router-link to="/login">Вход</router-link>
    </nav>
    <div v-if="user" class="app-user">
      <span>{{ user.name }} · {{ user.role }}</span>
      <button type="button" @click="onLogout">Выйти</button>
    </div>
  </header>
</template>
