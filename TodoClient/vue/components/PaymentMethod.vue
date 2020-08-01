<template>
  <b-card :title="`${paymentMethod.cardType} ••••${paymentMethod.lastFourDigits}`" :sub-title="`Expires on: ${paymentMethod.expirationDate}`">
    <b-link class="card-link mt-3 d-block" @click="updatePaymentInfo">Update payment method</b-link>
  </b-card>
</template>

<script>
export default {
  name: "PaymentMethod",
  props: ["paymentMethod"],
  methods: {
    updatePaymentInfo() {
      this.$emit("updatePaymentInfo");
    },
  },
};
</script>