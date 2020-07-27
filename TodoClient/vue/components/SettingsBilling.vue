<template>
  <section id="settings-plan">
    <PaymentMethod
      v-if="paymentMethod && !updatingPaymentInfo"
      @updatePaymentInfo="updatingPaymentInfo = true"
      class="mb-3"
    ></PaymentMethod>
    <CheckoutForm v-else @cancelUpdate="updatingPaymentInfo = false;" class="mb-3"></CheckoutForm>

    <ChangePlan class="mb-3"></ChangePlan>
  </section>
</template>

<script>
import PaymentMethod from "../components/PaymentMethod";
import CheckoutForm from "../components/CheckoutForm";
import ChangePlan from "../components/ChangePlan";

export default {
  name: "SettingsBilling",
  data() {
    return {
      plan: {},
      errorMessage: "",
      paymentMethod: "",
      updatingPaymentInfo: false,
    };
  },
  components: {
    PaymentMethod,
    CheckoutForm,
    ChangePlan,
  },
};
</script>