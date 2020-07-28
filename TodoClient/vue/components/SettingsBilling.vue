<template>
  <section id="settings-plan">
    <PaymentMethod
      v-if="paymentMethod.cardType && !updatingPaymentInfo"
      @updatePaymentInfo="updatingPaymentInfo = true"
      :paymentMethod="paymentMethod"
      class="mb-3"
    ></PaymentMethod>
    <CheckoutForm v-else @formSubmitted="getPaymentMethod(); updatingPaymentInfo = false;" class="mb-3"
    @formCancelled="updatingPaymentInfo = false;">
    </CheckoutForm>
      
    <ChangePlan class="mb-3"></ChangePlan>
  </section>
</template>

<script>
import axios from 'axios';

import PaymentMethod from "../components/PaymentMethod";
import CheckoutForm from "../components/CheckoutForm";
import ChangePlan from "../components/ChangePlan";

export default {
  name: "SettingsBilling",
  data() {
    return {
      plan: {},
      errorMessage: "",
      paymentMethod: {},
      updatingPaymentInfo: false,
    };
  },
  async created() {
    await this.getPaymentMethod();
  },
  methods: {
    async getPaymentMethod() {
      try {
        const response = await axios({
          method: 'GET',
          url: 'api/payments'
        });

        this.paymentMethod = response.data;
      }
      catch (err) {
        console.log(err);
      }
    }
  },
  components: {
    PaymentMethod,
    CheckoutForm,
    ChangePlan,
  },
};
</script>