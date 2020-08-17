<template>
  <div class="account-settings-wrapper">
    <b-form-group label="Full Name" v-if="user.fullName">
      <p class="form-control">{{ user.fullName }}</p>
    </b-form-group>
    <b-form-group label="Username" v-if="user.username">
      <p class="form-control">{{ user.username }}</p>
    </b-form-group>
    <b-form-group label="Email" v-if="user.email">
      <p class="form-control">{{ user.email }}</p>
    </b-form-group>
  </div>
</template>

<script>
import { mapState } from "vuex";

export default {
  name: "SettingsAccount",
  computed: mapState({
    user: (state) => state.user,
  }),
};
</script>

<style lang="scss" scoped>
.account-avatar {
  width: 100%;
  max-width: 100px;
  border-radius: 100px;
}

.form-control {
  margin-bottom: 0;
}
</style>