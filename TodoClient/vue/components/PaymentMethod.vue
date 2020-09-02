<template>
  <div class="payment-method-card" :class="{ 'loading': loading }">
    <b-card v-if="loading" title="Loading..."></b-card>
    <b-card
      v-else-if="paymentMethod"
      :title="`${paymentMethod.cardType} ••••${paymentMethod.lastFourDigits}`"
      :sub-title="`Expires on: ${paymentMethod.expirationDate}`"
    >
      <b-button
        variant="link"
        @click="removePaymentMethod"
        class="card-link mt-3 p-0 d-block"
      >Remove card</b-button>
    </b-card>
    <b-card
      v-else
      title="No payment method exists."
      sub-title="Use the form below to update your payment method."
    ></b-card>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

export default {
  name: "PaymentMethod",
  props: ["paymentMethod", "loading"],
  methods: {
    updatePaymentInfo() {
      this.$emit("updatePaymentInfo");
    },
    async removePaymentMethod() {
      this.$emit("remove-payment-method");
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

<style lang="scss">
.payment-method-card {
  &.loading {
    .card-title {
      margin-bottom: 0px;
    }
  }
}
</style>