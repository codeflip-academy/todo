<template>
  <b-card
    v-if="todoList.role != 1 && todoList.role != 4"
    class="todo-list-preview bg-light"
    no-body
  >
    <b-card-body class="todo-list-preview-content">
      <b-card-title class="todo-list-preview-title">{{ todoList.listTitle }}</b-card-title>

      <b-badge
        class="todo-list-preview-status"
        pill
        :class="{ 'badge-success': todoList.completed, 'badge-secondary': !todoList.completed }"
        v-if="todoList.role == 2 || todoList.role == 3"
      >{{ todoList.completed ? 'Completed' : 'In Progress' }}</b-badge>

      <Contributors
        :todoListContributors="todoList.contributors"
        :accountContributors="contributors"
        class="todo-list-preview-contributors"
      ></Contributors>

      <div class="todo-list-preview-options">
        <!-- Invited Options -->
        <b-button-group v-if="todoList.role == 0">
          <b-button variant="success" @click="acceptInvitation">Accept</b-button>
          <b-button variant="danger" @click="declineInvitation">Decline</b-button>
        </b-button-group>

        <!-- Contributor Options -->
        <b-button-group v-if="todoList.role == 2">
          <b-button variant="info" @click="$router.push(`/lists/${todoList.id}`);">View</b-button>
          <b-button variant="secondary" @click="leaveTodoList">Leave</b-button>
        </b-button-group>

        <!-- Owner Options -->
        <b-button-group v-if="todoList.role == 3">
          <b-button variant="info" @click="$router.push(`/lists/${todoList.id}`);">View</b-button>
          <b-button variant="danger" @click="deleteTodoList">Delete</b-button>
        </b-button-group>
      </div>
    </b-card-body>
  </b-card>
</template>

<script>
import axios from "axios";

import Contributors from "./Contributors";

export default {
  props: ["todoList", "contributors"],
  components: {
    Contributors,
  },
  methods: {
    deleteTodoList() {
      this.$emit("delete-todo-list", this.todoList.id);
    },
    async acceptInvitation() {
      await axios({
        method: "POST",
        url: `api/lists/${this.todoList.id}/accept`,
      });

      this.$store.commit("changeUserRoleByListId", {
        todoListId: this.todoList.id,
        role: 2,
      });
    },
    async declineInvitation() {
      await axios({
        method: "POST",
        url: `api/lists/${this.todoList.id}/decline`,
      });

      this.$store.commit("deleteTodoList", this.todoList.id);
    },
    async leaveTodoList() {
      await axios({
        method: "POST",
        url: `api/lists/${this.todoList.id}/removeself`,
      });

      this.$store.commit("deleteTodoList", this.todoList.id);
    },
  },
};
</script>

<style lang="scss" scoped>
.todo-list-preview {
  z-index: 0;
  margin-bottom: 24px;

  .todo-list-preview-contributors {
    margin-top: 12px;
  }

  .todo-list-preview-options {
    margin-top: 12px;
  }

  @media screen and (min-width: 768px) {
    .todo-list-preview-content {
      display: flex;
      align-items: center;

      .todo-list-preview-title {
        margin: 0;
        flex: 1 0 auto;
      }

      .todo-list-preview-contributors {
        margin: 0 20px;
      }

      .todo-list-preview-options {
        margin: 0;
      }
    }
  }
}
</style>