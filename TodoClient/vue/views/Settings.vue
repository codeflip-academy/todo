<template>
  <section id="settings-page">
    <b-container>
      <h1>Settings</h1>
      <hr class="mb-4" />

      <section id="settings-subscription">
        <h2 class="text-muted mb-3">Change Plan</h2>
        <b-form-group>
          <b-form-select v-model="planName" :options="plans"></b-form-select>
        </b-form-group>
      </section>
    </b-container>
  </section>
</template>

<script>
export default {
  name: "Settings",
  data() {
    return {
      plans: ["Free", "Basic", "Premium"]
    };
  },
  computed: {
    planName: {
      get() {
        return this.$store.getters.planName;
      },
      set(value) {
        this.$store.dispatch("changePlan", { planName: value });
      }
    }
  }
};
</script>