<template>
  <div v-if="todoList.role === 3">
    <b-button class="invitation-btn" size="sm" @click="$bvModal.show('invitation-modal')">
      <span class="sr-only">Invite</span>
      <b-icon-plus></b-icon-plus>
    </b-button>

    <b-modal title="Send Invitation" id="invitation-modal">
      <b-form @submit.prevent="invite" class="invitation-form">
        <b-form-group class="email-group text-secondary" label="Email">
          <b-form-input v-model="form.email" type="email" required></b-form-input>
        </b-form-group>
        <div class="text-right">
          <b-button type="submit">Send</b-button>
        </div>
        <b-alert
          variant="success"
          class="mb-0 mt-3"
          :show="dismissCountDown"
          dismissable
          fade
          @dismissed="dismissCountDown=0"
          @dismiss-count-down="countDownChanged"
        >Invitation sent!</b-alert>
      </b-form>
    </b-modal>
  </div>
</template>

<script>
import axios from "axios";

export default {
  props: ["todoList"],
  data() {
    return {
      form: {
        email: "",
      },
      invitationSent: false,
      dismissSecs: 5,
      dismissCountDown: 0,
    };
  },
  methods: {
    async invite() {
      await axios({
        method: "POST",
        url: `api/lists/${this.todoList.id}/email`,
        data: JSON.stringify({ email: this.form.email }),
        headers: {
          "content-type": "application/json",
        },
      });
      this.form.email = "";
      this.showAlert();
    },
    countDownChanged(dismissCountDown) {
      this.dismissCountDown = dismissCountDown;
    },
    showAlert() {
      this.dismissCountDown = this.dismissSecs;
    },
  },
};
</script>

<style lang="scss" scoped>
h3 {
  font-size: 24px;
}

.invitation-form {
  .email-group {
    font-family: "Nunito", sans-serif;
    font-weight: bold;
  }
}

.invitation-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 20px;
  width: 30px;
  height: 30px;
  border-radius: 100px;
  margin-left: -10px;
}
</style>