<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { createTicket } from '../api/tickets.js'
import { getCurrentUser } from '../services/auth.js'

const router = useRouter()
const user = computed(() => getCurrentUser())

// Реактивный объект формы. В script доступ через .value, в template — без него.
const form = ref({
  title: '',
  roomId: 0,
  description: ''
})

// Ошибки показываются рядом с соответствующим полем (ЛР5).
const errors = ref({
  title: '',
  roomId: '',
  description: ''
})

const serverError = ref('')
const isSubmitting = ref(false)

function validate() {
  const title = form.value.title.trim()
  const description = form.value.description.trim()
  let ok = true

  if (title.length < 5 || title.length > 80) {
    errors.value.title = 'Тема: от 5 до 80 символов'
    ok = false
  } else {
    errors.value.title = ''
  }

  if (!Number.isInteger(form.value.roomId) || form.value.roomId < 1) {
    errors.value.roomId = 'Комната: целое число больше нуля'
    ok = false
  } else {
    errors.value.roomId = ''
  }

  if (description.length < 10 || description.length > 500) {
    errors.value.description = 'Описание: от 10 до 500 символов'
    ok = false
  } else {
    errors.value.description = ''
  }

  return ok
}

async function onCreate() {
  serverError.value = ''

  // Защита от повторной отправки (ЛР6)
  if (isSubmitting.value) return

  // Клиентская проверка помогает человеку, серверная повторяется в ЛР8
  if (!validate()) return

  isSubmitting.value = true
  try {
    // id и number берутся из ответа API, на клиенте не вычисляются
    await createTicket({
      title: form.value.title.trim(),
      roomId: form.value.roomId,
      description: form.value.description.trim()
    })
    form.value = { title: '', roomId: 0, description: '' }
    router.push('/')
  } catch (error) {
    if (error.status === 400) {
      serverError.value = 'Проверьте поля формы. ' + error.message
    } else if (error.status === 500) {
      serverError.value = 'Сервер не смог обработать запрос'
    } else if (error.status === 401) {
      serverError.value = 'Требуется вход: войдите в системе'
    } else if (error.status === 0) {
      serverError.value = error.message
    } else {
      serverError.value = error.message
    }
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <main class="page page-narrow">
    <h1>Новое обращение</h1>

    <p v-if="!user" class="notice">
      Систему могут использовать только авторизованные пользователи.
      <router-link to="/login">Войти</router-link>
    </p>

    <!-- .prevent: обычная форма перезагрузила бы страницу и стёрла состояние -->
    <form @submit.prevent="onCreate">
      <div class="field">
        <label for="title">Тема, 5–80 символов</label>
        <input
          id="title"
          name="title"
          v-model="form.title"
          autofocus
          aria-describedby="title-error"
        />
        <p id="title-error" class="field-error" v-if="errors.title">{{ errors.title }}</p>
      </div>

      <div class="field">
        <label for="roomId">Комната (номер из справочника)</label>
        <input
          id="roomId"
          name="roomId"
          type="number"
          min="1"
          v-model.number="form.roomId"
          aria-describedby="roomId-error"
        />
        <p id="roomId-error" class="field-error" v-if="errors.roomId">{{ errors.roomId }}</p>
      </div>

      <div class="field">
        <label for="description">Описание, 10–500 символов</label>
        <textarea
          id="description"
          name="description"
          rows="4"
          v-model="form.description"
          aria-describedby="description-error"
        ></textarea>
        <p id="description-error" class="field-error" v-if="errors.description">{{ errors.description }}</p>
      </div>

      <p class="field-error form-error" role="alert" v-if="serverError">{{ serverError }}</p>

      <button type="submit" :disabled="isSubmitting">
        {{ isSubmitting ? 'Создаём…' : 'Создать' }}
      </button>
    </form>
  </main>
</template>
