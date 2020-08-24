<template></template>

<script>
import TodoLists from "../components/TodoLists";

export default {
  name: "Home",
  computed: {
    user() {
      return this.$store.getters.user;
    },
    firstName() {
      const fullName = this.user.fullName.split(" ");
      return fullName[0];
    },
  },
  components: {
    TodoLists,
  },
};
</script>