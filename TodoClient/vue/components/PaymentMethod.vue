<template>
  <b-card
    :title="`${paymentMethod.cardType ? paymentMethod.cardType : 'Loading'} ••••${paymentMethod.lastFourDigits ? paymentMethod.lastFourDigits : '1111'}`"
    :sub-title="`Expires on: ${paymentMethod.expirationDate ? paymentMethod.expirationDate : '...'}`"
  >
    <b-link class="card-link mt-3 d-block">Remove card</b-link>
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