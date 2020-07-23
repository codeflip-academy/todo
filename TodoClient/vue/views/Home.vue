<template>
  <b-container v-if="user.email">
    <h1 class="mb-4">My Lists</h1>

    <TodoLists></TodoLists>

    <AddTodoListForm></AddTodoListForm>
  </b-container>
</template>

<script>
import TodoLists from "../components/TodoLists";
import AddTodoListForm from "../components/AddTodoListForm";

export default {
  name: "Home",
  data() {
    return {};
  },
  computed: {
    user() {
      return this.$store.getters.user;
    }
  },
  components: {
    TodoLists,
    AddTodoListForm
  }
};
</script>