<template>
  <div class="todo-list-wrapper">
    <div class="todo-list">
      <h1
        class="todo-list-title mb-2"
        @click="showTitleEditor"
        v-if="!editingTitle"
      >{{ list.listTitle }}</h1>

      <b-form class="list-title-editor" v-if="editingTitle" @submit.prevent="updateListTitle">
        <b-form-group>
          <b-form-input
            ref="listTitleInput"
            v-model="form.title"
            id="title"
            maxlength="50"
            required
          ></b-form-input>
        </b-form-group>

        <b-button variant="success" type="submit" class="mb-3">Save</b-button>
      </b-form>

      <Contributors
        class="mb-4"
        :todoListContributors="list.contributors"
        :accountContributors="contributors"
      ></Contributors>

      <b-row>
        <b-col class="mb-3" :class="{ 'col-md-8': list.role == 3 }">
          <TodoListItems :listId="todoListId" :todoListItems="items" v-if="!loadingItems"></TodoListItems>
          <b-list-group-item v-if="items && items.length < 1">Add an item to get started.</b-list-group-item>
          <AddTodoListItemForm class="mt-3" :todoListId="todoListId"></AddTodoListItemForm>
        </b-col>

        <b-col md="4" v-if="list.role == 3">
          <InviteContributorsForm :listId="this.todoListId"></InviteContributorsForm>
        </b-col>
      </b-row>
    </div>
  </div>
</template>

<script>
import Vue from "vue";
import AddTodoListItemForm from "./AddTodoListItemForm";
import TodoListItems from "./TodoListItems";
import Contributors from "./Contributors";
import InviteContributorsForm from "./InviteContributorsForm";
import VueConfetti from "vue-confetti";

Vue.use(VueConfetti);

export default {
  name: "TodoList",
  props: ["todoListId"],
  data() {
    return {
      items: [],
      editingTitle: false,
      form: {
        title: "",
      },
      loadingItems: true,
    };
  },
  async created() {
    await this.$store.dispatch("loadItemsByListId", {
      todoListId: this.todoListId,
    });

    this.items = this.getItems();
    this.loadingItems = false;
  },
  destroyed() {
    this.$confetti.stop();
  },
  computed: {
    list() {
      return this.$store.getters.getTodoListById(this.todoListId);
    },
    contributors() {
      return this.$store.getters.contributors;
    },
    allItemsCompleted() {
      return (
        this.items.every((item) => item.completed) && this.items.length > 0
      );
    },
  },
  watch: {
    allItemsCompleted: function () {
      if (this.allItemsCompleted) {
        this.$confetti.start({
          particles: [{ type: "rect" }],
          particlesPerFrame: 0.5,
          dropRate: 8,
        });

        this.$store.commit("setTodoListCompletedState", {
          listId: this.todoListId,
          listCompletedState: true,
        });
      } else {
        this.$confetti.stop();
        this.$store.commit("setTodoListCompletedState", {
          listId: this.todoListId,
          listCompletedState: false,
        });
      }
    },
  },
  components: {
    AddTodoListItemForm,
    TodoListItems,
    Contributors,
    InviteContributorsForm,
  },
  methods: {
    getItems() {
      return this.$store.getters.getItemsByListId(this.todoListId);
    },
    showTitleEditor() {
      this.editingTitle = true;
      this.$nextTick(() => {
        this.$refs.listTitleInput.focus();
      });
      this.form.title = this.list.listTitle;
    },
    async updateListTitle() {
      this.editingTitle = false;
      await this.$store.dispatch("updateListTitle", {
        listId: this.todoListId,
        listTitle: this.form.title,
      });
      this.form.title = "";
    },
  },
};
</script>