<template>
  <section id="settings-page">
    <b-container>
      <h1>Settings</h1>
      <hr class="mb-4" />

      <section id="settings-subscription">
        <h2 class="text-muted mb-3">Change Plan</h2>
        <b-form-group>
          <b-form-select :state="canChangePlan" v-model="planName" :options="plans"></b-form-select>
        </b-form-group>
        <p class="text-danger text-sm" v-if="canChangePlan === false">{{ errorMessage }}</p>
      </section>
    </b-container>
  </section>
</template>

<script>
export default {
  name: "Settings",
  data() {
    return {
      plans: ["Free", "Basic", "Premium"],
      canChangePlan: null,
      plan: {},
      errorMessage: "",
    };
  },
  computed: {
    planName: {
      get() {
        return this.$store.getters.planName;
      },
      async set(value) {
        try {
          await this.$store.dispatch("changePlan", { planName: value });
          this.canChangePlan = null;
        } catch {
          this.canChangePlan = false;
          this.plan = this.$store.getters.plan;
          this.errorMessage = `Unable to change your plan to ${value}. You have too many lists.`;
        }
      },
    },
  },
};
</script>