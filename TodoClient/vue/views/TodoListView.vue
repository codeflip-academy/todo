<template>
  <b-container>
    <b-button class="back-button mb-2" variant="link" to="/">
      <b-icon icon="chevron-left"></b-icon>My Lists
    </b-button>

    <TodoList :todoListId="todoListId" v-if="!loadingTodoLists"></TodoList>
  </b-container>
</template>

<script>
import { mapState } from "vuex";
import TodoList from "../components/TodoList";

export default {
  name: "TodoListView",
  props: ["todoListId"],
  components: {
    TodoList,
  },
  computed: mapState({
    loadingTodoLists: (state) => state.loadingTodoLists,
  }),
};
</script>

<style lang="scss" scoped>
.back-button {
  display: flex;
  align-items: center;
  padding: 0px;
  position: relative;
  left: -7px;
  margin-bottom: 10px;
}
</style>