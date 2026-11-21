import { createRouter, createWebHistory } from 'vue-router'
import TicketList from './pages/TicketList.vue'
import TicketNew from './pages/TicketNew.vue'
import Login from './pages/Login.vue'

// Три маршрута из ЛР4. Список "/", создание "/new", вход "/login".
export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'ticket-list', component: TicketList },
    { path: '/new', name: 'ticket-new', component: TicketNew },
    { path: '/login', name: 'login', component: Login }
  ]
})
