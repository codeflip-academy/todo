<template>
  <div class="list-wrapper" v-if="!loadingTodoLists">
    <Confetti v-if="todoList.completed"></Confetti>

    <header class="list-header">
      <h2 class="list-title">{{ todoList.listTitle }}</h2>
      <div class="list-controls"></div>
    </header>
    <TodoListItems
      :todoListId="todoList.id"
      @todo-list-completed="setTodoListCompleted"
      @todo-list-uncompleted="setTodoListUncompleted"
    ></TodoListItems>
  </div>
</template>

<script>
import { mapState, mapGetters } from "vuex";

import TodoListItems from "./TodoListItems";
import Contributors from "./Contributors";
import InviteContributorsForm from "./InviteContributorsForm";
import Confetti from "./Confetti";
import Draggable from "vuedraggable";

export default {
  name: "TodoList",
  props: ["todoListId"],
  data() {
    return {
      editingTitle: false,
      todoListForm: {
        listTitle: "",
      },
    };
  },
  computed: {
    todoList() {
      return this.$store.getters.getTodoListById(this.todoListId);
    },
    ...mapState({
      contributors: (state) => state.contributors,
      loadingTodoLists: (state) => state.loadingTodoLists,
    }),
  },
  components: {
    TodoListItems,
    Contributors,
    InviteContributorsForm,
    Confetti,
    Draggable,
  },
  methods: {
    async updateListTitle() {
      this.editingTitle = false;

      await this.$store.dispatch("updateTodoListTitle", {
        todoListId: this.todoListId,
        listTitle: this.todoListForm.listTitle,
      });

      this.todoListForm.listTitle = "";
    },
    showTitleEditor() {
      this.editingTitle = true;
      this.$nextTick(() => {
        this.$refs.listTitleInput.focus();
      });
      this.todoListForm.listTitle = this.todoList.listTitle;
    },
    setTodoListCompleted() {
      this.$store.commit("setTodoListCompletedState", {
        todoListId: this.todoListId,
        completed: true,
      });
    },
    setTodoListUncompleted() {
      this.$store.commit("setTodoListCompletedState", {
        todoListId: this.todoListId,
        completed: false,
      });
    },
  },
};
</script>