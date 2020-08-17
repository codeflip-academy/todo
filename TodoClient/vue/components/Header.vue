<template>
  <b-navbar v-if="user.email" toggleable="sm" type="light" class="fixed-top" id="navbar">
    <b-container>
      <b-navbar-brand>
        <div class="brand-text">
          <b-icon-check-circle-fill style="margin-right: 7px;"></b-icon-check-circle-fill>Todo
        </div>
      </b-navbar-brand>

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
    </b-container>
  </b-navbar>
</template>

<script>
import axios from "axios";
export default {
  name: "Header",
  data() {
    return {
      user: {},
    };
  },
  async created() {
    try {
      await this.checkAuthState();
      await this.getPlan();
      this.user = this.$store.getters.user;
    } catch (error) {
      console.error(error);
    }
  },
  methods: {
    async checkAuthState() {
      try {
        const user = await axios({
          method: "GET",
          url: "api/accounts",
        });
        this.$store.commit("setUserData", user.data);
      } catch (error) {
        console.error(error);
      }
    },
    async getPlan() {
      await this.$store.dispatch("getPlan");
    },
    logout() {
      this.user = {};
      axios({
        method: "GET",
        url: "api/accounts/logout",
      }).then(() => {
        this.$router.push("/login");
      });
    },
  },
};
</script>

<style lang="scss">
#navbar {
  z-index: 50;
  border-bottom: solid 1px #eee;
  padding: 15px 20px;
}

.brand-text {
  font-family: "Nunito", sans-serif;
  font-weight: bold;
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