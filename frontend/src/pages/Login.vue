<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { loginUser, getCurrentUser, logout } from '../services/auth.js'

const router = useRouter()
const user = computed(() => getCurrentUser())

const form = ref({
  login: '',
  password: ''
})
const error = ref('')
const isSubmitting = ref(false)

async function onSubmit() {
  error.value = ''
  if (isSubmitting.value) return

  isSubmitting.value = true
  try {
    await loginUser(form.value.login.trim(), form.value.password)
    router.push('/')
  } catch (e) {
    if (e.status === 401) {
      error.value = 'Неверный логин или пароль'
    } else if (e.status === 0) {
      error.value = e.message
    } else {
      error.value = e.message || 'Не удалось выполнить вход'
    }
  } finally {
    isSubmitting.value = false
  }
}

function onLogout() {
  logout()
}
</script>

<template>
  <main class="page page-narrow">
    <h1>Вход</h1>

    <div v-if="user" class="notice">
      <p>Вы вошли как {{ user.name }} ({{ user.role }}).</p>
      <button type="button" @click="onLogout">Выйти</button>
      <router-link to="/">К списку обращений</router-link>
    </div>

    <form v-else @submit.prevent="onSubmit">
      <div class="field">
        <label for="login">Логин</label>
        <input id="login" name="login" v-model="form.login" autocomplete="username" autofocus />
      </div>

      <div class="field">
        <label for="password">Пароль</label>
        <input
          id="password"
          name="password"
          type="password"
          v-model="form.password"
          autocomplete="current-password"
        />
      </div>

      <p class="field-error form-error" role="alert" v-if="error">{{ error }}</p>

      <button type="submit" :disabled="isSubmitting">
        {{ isSubmitting ? 'Проверяем…' : 'Войти' }}
      </button>
    </form>
  </main>
</template>
