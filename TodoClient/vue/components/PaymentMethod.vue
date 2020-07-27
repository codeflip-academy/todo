<template>
  <b-card title="Visa ••••4422" sub-title="Expires 10/21">
    <b-link class="card-link mt-3 d-block" @click="updatePaymentInfo">Update payment method</b-link>
  </b-card>
</template>

<script>
export default {
  name: "PaymentMethod",
  methods: {
    updatePaymentInfo() {
      this.$emit("updatePaymentInfo");
    },
  },
};
</script>