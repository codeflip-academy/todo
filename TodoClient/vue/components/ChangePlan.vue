<template>
  <b-card>
    <template v-slot:header>
      <b-row class="align-items-center">
        <b-col>
          <h6 class="mb-0 current-plan">Todo {{ selectedPlan }}</h6>
          <p class="text-muted mb-0">
            <small v-if="selectedPlan === 'Free'">Maximum of 10 lists.</small>
            <small
              v-if="selectedPlan === 'Basic'"
            >50 lists, 5 contributors per list, due dates, and notifications.</small>
            <small v-if="selectedPlan === 'Premium'">Unlimited lists and contributors.</small>
          </p>
        </b-col>
        <b-col class="text-right">
          <b-button variant="success" size="sm" :disabled="selectedPlan === currentPlan">Choose plan</b-button>
        </b-col>
      </b-row>
    </template>
    <b-card-text>
      <b-row class="align-items-center">
        <b-col>
          <div>
            <b-form-select v-model="selectedPlan" :options="plans"></b-form-select>
          </div>
        </b-col>
        <b-col class="text-right">
          <strong v-if="selectedPlan === 'Free'">$0.00</strong>
          <strong v-if="selectedPlan === 'Basic'">$5.00 / month</strong>
          <strong v-if="selectedPlan === 'Premium'">$10.00 / month</strong>
        </b-col>
      </b-row>
    </b-card-text>
  </b-card>
</template>

<script>
export default {
  name: "ChangePlan",
  data() {
    return {
      plans: ["Free", "Basic", "Premium"],
      selectedPlan: "Free",
    };
  },
  computed: {
    currentPlan() {
      return this.$store.getters.planName;
    },
  },
};
</script>

<style lang="scss">
.current-plan {
  font-family: "Nunito", sans-serif;
  font-weight: bold;
  line-height: 1;
}
</style>