<template>
  <div class="layout">
    <AppHeader />

    <router-view />

    <SiteFooter />
  </div>
</template>

<style>
.layout {
  min-height: 100vh;

  background-image: url('./assets/Background_view.png');
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
}
</style>