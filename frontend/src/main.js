import { createApp } from 'vue'
import App from './App.vue'
import { router } from './router.js'
import './assets/main.css'

const app = createApp(App)

// Роутер подключается до mount (ЛР4)
app.use(router)

app.mount('#app')
