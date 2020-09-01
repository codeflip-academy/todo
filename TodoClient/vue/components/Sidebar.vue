<template>
  <div class="sidebar">
    <div class="sidebar-header mb-3">
      <b-link :to="{ name: 'Home' }" class="sidebar-brand">
        <b-icon-check-all></b-icon-check-all>Todo
      </b-link>
    </div>
    <div class="sidebar-lists">
      <TodoLists></TodoLists>
    </div>
  </div>
</template>

<script>
import TodoLists from "./TodoLists";

export default {
  name: "Sidebar",
  components: {
    TodoLists,
  },
};
</script>