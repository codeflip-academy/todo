<template>
  <section id="settings-page">
    <b-container>
      <b-tabs pills vertical nav-wrapper-class="w-25">
        <b-tab title="Account" active>
          <h2>Account</h2>
          <SettingsAccount></SettingsAccount>
        </b-tab>
        <b-tab title="Billing">
          <h2>Billing</h2>
          <SettingsBilling></SettingsBilling>
        </b-tab>
      </b-tabs>
    </b-container>
  </section>
</template>

<script>
import SettingsAccount from "../components/SettingsAccount";
import SettingsBilling from "../components/SettingsBilling";

export default {
  name: "Settings",
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

  .tab-content {
    padding-left: 30px;
  }
}
</style>