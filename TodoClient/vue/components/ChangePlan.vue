<template>
  <b-overlay :show="changingPlan" rounded="sm">
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
              :disabled="selectedPlan === plan.name || !paymentMethod || changingPlan"
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
        <b-alert
          variant="warning"
          class="mt-3"
          :show="isDowngrading(selectedPlan, plan.name) && getPlanMaxListCountByPlanName(selectedPlan) < todoLists.length"
        >
          <strong>Warning:</strong> You have more lists than allowed on the
          <strong>{{ selectedPlan }}</strong> plan. Additional lists will be removed at the end of your billing cycle.
        </b-alert>
      </b-card-text>
    </b-card>
  </b-overlay>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import CouponForm from "./CouponForm";

export default {
  name: "ChangePlan",
  props: ["paymentMethod"],
  data() {
    return {
      plans: ["Free", "Basic", "Premium"],
      selectedPlan: "Free",
      changingPlan: false,
    };
  },
  components: {
    CouponForm,
  },
  created() {
    this.setSelectedPlan(this.plan.name);
  },
  methods: {
    async changePlan() {
      try {
        this.changingPlan = true;

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

        this.$bvToast.toast(`You have successfully updated your plan.`, {
          title: "Plan was changed",
          variant: "success",
          autoHideDelay: 5000,
          appendToast: true,
        });
      } catch (err) {
        this.$bvToast.toast(
          `We are unable to update your plan at this time. Please try again later.`,
          {
            title: "Error updating your plan.",
            variant: "danger",
            autoHideDelay: 5000,
            appendToast: true,
          }
        );
      } finally {
        this.changingPlan = false;
      }
    },
    setSelectedPlan(planName) {
      this.selectedPlan = planName;
    },
    getPlanMaxListCountByPlanName(planName) {
      switch (planName) {
        case "Free":
          return 5;
        case "Basic":
          return 10;
        case "Premium":
          return 9999999;
        default:
          return 0;
      }
    },
    convertPlanNameToId(planName) {
      switch (planName) {
        case "Free":
          return 1;
        case "Basic":
          return 2;
        case "Premium":
          return 3;
        default:
          return -1;
      }
    },
    isDowngrading(selectedPlan, currentPlan) {
      let selectedPlanId = this.convertPlanNameToId(selectedPlan);
      let currentPlanId = this.convertPlanNameToId(currentPlan);

      if (selectedPlanId < currentPlanId) {
        return true;
      }

      return false;
    },
  },
  computed: {
    ...mapState({
      plan: (state) => state.plan,
      todoLists: (state) => state.todoLists,
    }),
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