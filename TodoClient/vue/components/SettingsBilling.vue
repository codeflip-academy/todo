<template>
  <section id="settings-plan">
    <PaymentMethod
      class="mb-3"
      :loading="loadingPaymentInfo"
      :paymentMethod="paymentMethod"
      @remove-payment-method="removePaymentMethod"
    ></PaymentMethod>

    <CheckoutForm @form-submitted="getPaymentMethod" class="mb-3"></CheckoutForm>

    <ChangePlan :paymentMethod="paymentMethod" v-if="!loadingPlan"></ChangePlan>
  </section>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import PaymentMethod from "../components/PaymentMethod";
import CheckoutForm from "../components/CheckoutForm";
import ChangePlan from "../components/ChangePlan";

export default {
  name: "SettingsBilling",
  data() {
    return {
      plan: {},
      paymentMethod: null,
      loadingPaymentInfo: true,
      loadingCheckoutForm: true,
    };
  },
  async created() {
    await this.getPaymentMethod();
  },
  methods: {
    async getPaymentMethod() {
      this.loadingPaymentInfo = true;

      try {
        const response = await axios({
          method: "GET",
          url: "api/payments",
        });

        this.paymentMethod = response.data;
      } finally {
        this.loadingPaymentInfo = false;
      }
    },
    removePaymentMethod() {
      this.paymentMethod = null;
    },
  },
  components: {
    PaymentMethod,
    CheckoutForm,
    ChangePlan,
  },
  computed: {
    loading() {
      return this.loadingCheckoutForm || this.loadingPaymentInfo;
    },
    ...mapState({
      loadingPlan: (state) => state.loadingPlan,
    }),
  },
};
</script>

<style lang="scss" scoped>
.showing-overlay {
  min-height: 133px;
  margin-bottom: 16px;
}
</style>