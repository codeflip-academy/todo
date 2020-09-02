<template>
  <b-card
    :title="`${paymentMethod.cardType ? paymentMethod.cardType : 'Loading'} ••••${paymentMethod.lastFourDigits ? paymentMethod.lastFourDigits : '1111'}`"
    :sub-title="`Expires on: ${paymentMethod.expirationDate ? paymentMethod.expirationDate : '...'}`"
  >
    <b-button variant="link" @click="removePaymentMethod" class="card-link mt-3 d-block">Remove card</b-button>
  </b-card>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

export default {
  name: "PaymentMethod",
  props: ["paymentMethod"],
  methods: {
    updatePaymentInfo() {
      this.$emit("updatePaymentInfo");
    },
    async removePaymentMethod() {
      await axios({
        method: "DELETE",
        url: "api/payments/paymentMethod/delete",
        headers: { "content-type": "application/json" },
        data: JSON.stringify({
          plan: this.plan,
        }),
      });
    },
  },
  computed: {
    ...mapState({
      plan: (state) => state.plan.name,
    }),
  },
};
</script>