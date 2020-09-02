<template>
  <section id="settings-page">
    <b-card no-body>
      <b-tabs pills card>
        <b-tab title="Account" active>
          <h2>Account</h2>
          <SettingsAccount></SettingsAccount>
        </b-tab>
        <b-tab title="Billing">
          <h2>Billing</h2>
          <SettingsBilling></SettingsBilling>
        </b-tab>
        <b-tab title="Notifications">
          <h2>Notifications</h2>
          <h3>Emails:</h3>
          <p class="text-muted">Send notifications when:</p>
          <b-form class="notifications-form">
            <strong>
              <b-form-checkbox v-model="emailDueDate">List items Due Today</b-form-checkbox>
              <b-form-checkbox v-model="emailListCompleted">List Completed</b-form-checkbox>
              <b-form-checkbox v-model="emailItemCompleted">Item Completed</b-form-checkbox>
              <b-form-checkbox v-model="emailInvitation">Invitations</b-form-checkbox>
            </strong>
          </b-form>
        </b-tab>
      </b-tabs>
    </b-card>
  </section>
</template>

<script>
import SettingsAccount from "../components/SettingsAccount";
import SettingsBilling from "../components/SettingsBilling";
import axios from "axios";
import { updateLocale } from "moment";

export default {
  name: "Settings",
  data() {
    return {
      emailDueDate: false,
      emailListCompleted: false,
      emailItemCompleted: false,
      emailInvitation: false,
    };
  },
  async created() {
    this.getSettings();
  },
  methods: {
    async updateSettings() {
      await axios({
        method: "PUT",
        url: "api/accounts/emailFilter",
        headers: { "content-type": "application/json" },
        data: JSON.stringify({
          emailDueDate: this.emailDueDate,
          emailListCompleted: this.emailListCompleted,
          emailItemCompleted: this.emailItemCompleted,
          emailInvitation: this.emailInvitation,
        }),
      });
    },

    async getSettings() {
      const response = await axios({
        method: "GET",
        url: "api/accounts/emailFilter",
      });

      this.emailDueDate = response.data.emailDueDate;
      this.emailListCompleted = response.data.emailListCompleted;
      this.emailItemCompleted = response.data.emailItemCompleted;
      this.emailInvitation = response.data.emailInvitation;
    },
  },
  watch: {
    emailDueDate() {
      this.updateSettings();
    },
    emailListCompleted() {
      this.updateSettings();
    },
    emailItemCompleted() {
      this.updateSettings();
    },
    emailInvitation() {
      this.updateSettings();
    },
  },
  components: {
    SettingsAccount,
    SettingsBilling,
  },
};
</script>

<style lang="scss">
#settings-page {
  padding: 20px;

  h2 {
    padding-bottom: 10px;
    margin-bottom: 20px;
    border-bottom: solid 1px #ddd;
  }

  .notifications-form {
    custom-control-label {
      font-weight: bold !important;
    }
  }

  .tab-content {
    padding: 20px 30px;
  }
}
</style>