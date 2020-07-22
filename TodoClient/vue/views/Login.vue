<template>
  <b-container>
    <section id="login">
      <h1 class="title mb-5">Todo</h1>
      <p class="subtitle text-muted mb-4">Welcome. Please sign in to access or create your account.</p>
      <b-button class="btn-dark btn-lg" href="/api/accounts/login">Sign in with GitHub</b-button>
    </section>
  </b-container>
</template>

<script>
import axios from "axios";

export default {
  name: "Login",
  data() {
    return {};
  },
  methods: {
    logout() {
      axios({
        method: "GET",
        url: "/api/accounts/logout",
      });
    },
  },
};
</script>

<style lang="scss">
#login {
  .title {
    font-size: 60px;
  }

  .subtitle {
    font-size: 30px;
    font-weight: bold;
    font-family: "Nunito", sans-serif;
    max-width: 420px;
    line-height: 1.2;
  }
}
</style>