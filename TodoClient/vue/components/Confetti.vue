<template></template>

<script>
import Vue from "vue";
import VueConfetti from "vue-confetti";

Vue.use(VueConfetti);

export default {
  name: "Confetti",
  mounted() {
    this.$nextTick(() => {
      this.$confetti.start({
        particles: [{ type: "rect" }],
        particlesPerFrame: 0.4,
        dropRate: 8,
      });
    });
  },
  beforeDestroy() {
    this.$confetti.stop();
  },
};
</script>