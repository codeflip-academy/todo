<template>
  <div>
    <div v-if="!loadingTodoLists">
      <todo-list-preview v-for="todoList in todoLists" :key="todoList.id" :todoList="todoList"></todo-list-preview>
    </div>
    <AddTodoListForm @add-todo-list="addTodoList"></AddTodoListForm>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import TodoListPreview from "./TodoListPreview";
import AddTodoListForm from "../components/AddTodoListForm";

export default {
  components: {
    TodoListPreview,
    AddTodoListForm,
  },
  computed: mapState({
    todoLists: (state) => state.todoLists,
    contributors: (state) => state.contributors,
    loadingTodoLists: (state) => state.loadingTodoLists,
  }),
  methods: {
    async addTodoList(listTitle) {
      await this.$store.dispatch("addTodoList", { listTitle });
    },
  },
};
</script>