<template>
  <b-card>
    <template v-slot:header>
      <b-row class="align-items-center">
        <b-col>
          <h6 class="mb-0 current-plan">Todo {{ selectedPlan }}</h6>
          <p class="text-muted mb-0 plan-description">
            <small v-if="selectedPlan === 'Free'">Maximum of 10 lists.</small>
            <small
              v-if="selectedPlan === 'Basic'"
            >Maximum of 5 lists and contributors, due dates, and notifications.</small>
            <small
              v-if="selectedPlan === 'Premium'"
            >Unlimited lists and contributors, due dates, and notifications.</small>
          </p>
        </b-col>
        <b-col class="text-right">
          <b-button
            variant="success"
            size="sm"
            :disabled="selectedPlan === currentPlan || !paymentMethod"
            @click="changePlan"
          >Choose plan</b-button>
        </b-col>
      </b-row>
    </template>
    <b-card-text>
      <b-row class="align-items-center mb-1">
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
      <CouponForm></CouponForm>
    </b-card-text>
  </b-card>
</template>

<script>
import axios from "axios";

import CouponForm from "./CouponForm";

export default {
  name: "ChangePlan",
  props: ["paymentMethod"],
  data() {
    return {
      plans: ["Free", "Basic", "Premium"],
      selectedPlan: "Free",
    };
  },
  components: {
    CouponForm,
  },
  methods: {
    async changePlan() {
      const response = await axios({
        method: "POST",
        url: "api/payments/subscription/change",
        data: JSON.stringify({
          plan: this.selectedPlan,
        }),
        headers: {
          "content-type": "application/json",
        },
      });
    },
    setSelectedPlan(planName) {
      this.selectedPlan = planName;
    },
  },
  computed: {
    currentPlan() {
      return this.$store.getters.planName;
    },
  },
  watch: {
    currentPlan() {
      this.setSelectedPlan(this.currentPlan);
    },
  },
};
</script>

<style lang="scss">
.current-plan {
  font-family: "Nunito", sans-serif;
  font-weight: bold;
  line-height: 1;
  margin-bottom: 12px;
}

.plan-description {
  line-height: 1.2;
}
</style>