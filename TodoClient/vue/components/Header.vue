<template>
  <b-navbar toggleable="sm" type="light" variant="light" class="fixed-top" id="navbar">
    <b-navbar-brand>Todo</b-navbar-brand>

    <div class="user-info ml-auto" v-if="user.id">
      <b-button class="ml-auto mr-2" @click="logout">Logout</b-button>
      <b-avatar :src="user.pictureUrl"></b-avatar>
    </div>
  </b-navbar>
</template>

<script>
import axios from "axios";
export default {
  name: "Header",
  async created() {
    await this.checkAuthState();
    await this.getPlan();
  },
  computed: {
    user() {
      return this.$store.getters.user;
    }
  },
  methods: {
    async checkAuthState() {
      try {
        await axios({
          method: "GET",
          url: "api/accounts/login"
        });
        const user = await axios({
          method: "GET",
          url: "api/accounts"
        });
        this.$store.commit("setUserData", user.data);
      } catch {
        if (this.$router.name !== "Login") {
          this.$router.push("/login");
        }
      }
    },
    async getPlan() {
      await this.$store.dispatch("getPlan");
    },
    logout() {
      axios({
        method: "GET",
        url: "api/accounts/logout"
      }).then(() => {
        this.$router.push("/login");
      });
    }
  }
};
</script>

<style lang="scss">
#navbar {
  z-index: auto;
}
</style>