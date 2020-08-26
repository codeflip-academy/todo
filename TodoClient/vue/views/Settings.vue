<template>
  <section id="settings-page">
    <b-container>
      <b-tabs pills vertical nav-wrapper-class="w-25">
        <b-tab title="Account">
          <h2>Account</h2>
          <SettingsAccount></SettingsAccount>
        </b-tab>
        <b-tab title="Billing" active>
          <h2>Billing</h2>
          <SettingsBilling></SettingsBilling>
        </b-tab>
        <b-tab title="Notifications" active>
          <h2>Notifications</h2>
          <h3>Emails:</h3>
          <p class="text-muted">Send notifications when:</p>
          <b-form class="notifications-form">
            <strong>
              <b-form-checkbox inline v-model="emailDueDate">Items Due Today</b-form-checkbox>
              <b-form-checkbox inline v-model="emailCompleted">List Completed</b-form-checkbox>
            </strong>
          </b-form>
        </b-tab>
      </b-tabs>
    </b-container>
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
      emailCompleted: false,
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
          emailCompleted: this.emailCompleted,
        }),
      });
    },

    async getSettings() {
      const response = await axios({
        method: "GET",
        url: "api/accounts/emailFilter",
      });

      this.emailDueDate = response.data.emailDueDate;
      this.emailCompleted = response.data.emailCompleted;
    },
  },
  watch: {
    emailDueDate() {
      this.updateSettings();
    },
    emailCompleted() {
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
  .nav-link {
    margin-bottom: 10px;
  }

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
    padding-left: 30px;
  }
}
</style>