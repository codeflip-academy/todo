<template>
  <b-navbar toggleable="sm" type="light" variant="light" class="fixed-top" id="navbar">
    <b-navbar-brand>Todo</b-navbar-brand>

    <div class="user-info ml-auto" v-if="user.id">
      <b-dropdown class="account-dropdown" right>
        <template v-slot:button-content>
          <b-avatar :src="user.pictureUrl"></b-avatar>
        </template>
        <b-dropdown-item to="/lists">My Lists</b-dropdown-item>
        <b-dropdown-divider></b-dropdown-divider>
        <b-dropdown-item to="/settings">Settings</b-dropdown-item>
        <b-dropdown-item @click="logout">Sign Out</b-dropdown-item>
      </b-dropdown>
    </div>
  </b-navbar>
</template>

<script>
import axios from "axios";
export default {
  name: "Header",
  data() {
    return {
      user: {}
    };
  },
  async created() {
    try {
      await this.checkAuthState();
      await this.getPlan();
      this.user = this.$store.getters.user;
    } catch (error) {}
  },
  methods: {
    async checkAuthState() {
      try {
        const user = await axios({
          method: "GET",
          url: "api/accounts"
        });
        this.$store.commit("setUserData", user.data);
      } catch (error) {}
    },
    async getPlan() {
      await this.$store.dispatch("getPlan");
    },
    logout() {
      this.user = {};
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
  z-index: 50;
}

.account-dropdown {
  .btn {
    padding: 0;
    background: transparent !important;
    border: none !important;

    &:focus {
      outline: none;
      border: none;
    }

    &::after {
      display: none;
    }
  }
}
</style>