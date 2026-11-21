<script setup>
// ЛР4: App.vue — только каркас. Шапка снаружи router-view,
// страницы подставляются роутером по адресу.
import AppHeader from './components/AppHeader.vue'
</script>

<template>
  <div class="app-shell">
    <AppHeader />
    <router-view />
    <footer class="app-footer">
      <p>Система обработки обращений жильцов общежития · КГЭУ · ТРИС-2-23</p>
    </footer>
  </div>
</template>
