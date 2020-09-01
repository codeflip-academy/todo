<template>
  <div class="account-options">
    <Invitations></Invitations>
    <b-dropdown class="account-dropdown" right>
      <template v-slot:button-content>
        <div class="profile-picture">
          <img :src="user.pictureUrl" alt />
        </div>
        <div class="profile-name">{{ user.fullName }}</div>
        <div class="dropdown-toggler">
          <b-icon-chevron-down></b-icon-chevron-down>
        </div>
      </template>
      <b-dropdown-item to="/settings">
        <b-icon-gear class="mr-2"></b-icon-gear>Settings
      </b-dropdown-item>
      <b-dropdown-item-button @click="signOut">
        <b-icon-box-arrow-left class="mr-2"></b-icon-box-arrow-left>Sign out
      </b-dropdown-item-button>
    </b-dropdown>
  </div>
</template>

<script>
import axios from "axios";
import Invitations from "./Invitations";

export default {
  name: "Header",
  components: {
    Invitations,
  },
  computed: {
    user() {
      return this.$store.getters.user;
    },
  },
  methods: {
    async signOut() {
      await axios({
        method: "GET",
        url: "api/accounts/logout",
      });

      this.$router.push("/login");
    },
  },
};
</script>