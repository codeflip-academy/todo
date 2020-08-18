<template>
  <section id="settings-plan">
    <PaymentMethod v-if="!loadingPaymentInfo" :paymentMethod="paymentMethod" class="mb-3"></PaymentMethod>

    <b-overlay :show="false" blur="5px" spinner-variant="primary" spinner-type="grow" spinner-small>
      <CheckoutForm
        @form-submitted="getPaymentMethod"
        @form-ready="loadingCheckoutForm = false;"
        class="mb-3"
      ></CheckoutForm>
    </b-overlay>

    <ChangePlan></ChangePlan>
  </section>
</template>

<script>
import axios from "axios";

import PaymentMethod from "../components/PaymentMethod";
import CheckoutForm from "../components/CheckoutForm";
import ChangePlan from "../components/ChangePlan";

export default {
  name: "SettingsBilling",
  data() {
    return {
      plan: {},
      paymentMethod: {},
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

      const response = await axios({
        method: "GET",
        url: "api/payments",
      });

      this.paymentMethod = response.data;

      this.loadingPaymentInfo = false;
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
  },
};
</script>

<style lang="scss" scoped>
.showing-overlay {
  min-height: 133px;
  margin-bottom: 16px;
}
</style>