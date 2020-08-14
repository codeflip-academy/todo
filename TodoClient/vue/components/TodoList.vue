<template>
  <div class="todo-list-wrapper">
    <Confetti v-if="todoList.completed"></Confetti>

    <div class="todo-list">
      <h1
        class="todo-list-title mb-2"
        @click="showTitleEditor"
        v-if="!editingTitle"
      >{{ todoList.listTitle }}</h1>

      <b-form class="list-title-editor" v-if="editingTitle" @submit.prevent="updateListTitle">
        <b-form-group>
          <b-form-input
            ref="listTitleInput"
            v-model="todoListForm.listTitle"
            id="title"
            maxlength="50"
            required
          ></b-form-input>
        </b-form-group>

        <b-button variant="success" type="submit" class="mb-3">Save</b-button>
      </b-form>

      <Contributors
        class="mb-4"
        :todoListContributors="todoList.contributors"
        :accountContributors="contributors"
      ></Contributors>

      <b-row>
        <b-col class="mb-3" :class="{ 'col-md-8': todoList.role == 3 }">
          <TodoListItems
            :todoListId="todoListId"
            @todo-list-completed="setTodoListCompleted"
            @todo-list-uncompleted="setTodoListUncompleted"
          ></TodoListItems>
        </b-col>

        <b-col md="4" v-if="todoList.role == 3">
          <InviteContributorsForm :listId="this.todoListId"></InviteContributorsForm>
        </b-col>
      </b-row>
    </div>
  </div>
</template>

<script>
import { mapState, mapGetters } from "vuex";

import TodoListItems from "./TodoListItems";
import Contributors from "./Contributors";
import InviteContributorsForm from "./InviteContributorsForm";
import Confetti from "./Confetti";

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
    }),
  },
  components: {
    TodoListItems,
    Contributors,
    InviteContributorsForm,
    Confetti,
  },
  methods: {
    showTitleEditor() {
      this.editingTitle = true;
      this.$nextTick(() => {
        this.$refs.listTitleInput.focus();
      });
      this.form.title = this.list.listTitle;
    },
    async updateListTitle() {
      this.editingTitle = false;

      await this.$store.dispatch("updateTodoListTitle", {
        todoListId: this.todoListId,
        listTitle: this.form.title,
      });

      this.todoListForm.listTitle = "";
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