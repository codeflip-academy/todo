<template>
  <b-container v-if="user.email">
    <h1 class="mb-4">My Lists</h1>
    <TodoLists></TodoLists>
  </b-container>
</template>

<script>
import TodoLists from "../components/TodoLists";

export default {
  name: "Home",
  computed: {
    user() {
      return this.$store.getters.user;
    },
  },
  components: {
    TodoLists,
  },
};
</script>