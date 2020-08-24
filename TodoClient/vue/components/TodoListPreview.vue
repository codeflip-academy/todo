<template>
  <b-button
    class="sidebar-list"
    :to="{ path: `/lists/${todoList.id}` }"
    :class="{ 'selected': $route.params.todoListId == todoList.id }"
  >
    {{ todoList.listTitle }}
    <div class="sidebar-list-completed-state">8</div>
  </b-button>
</template>

<script>
import axios from "axios";

export default {
  props: ["todoList"],
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
$light-gray: #f5f6f7;
$gray: #455a64;
$blue: #1e88e5;
$orange: #ff7043;
$green: #4caf50;
$red: #b71c1c;

.sidebar-list {
  display: flex;
  align-items: center;
  color: $gray;
  text-align: left;
  width: 100%;
  border: none;
  border-radius: 100px;
  border-top-left-radius: 0;
  border-bottom-left-radius: 0;
  background: #fff;
  padding: 7px 10px 7px 30px;

  &:hover,
  &:active {
    color: $gray;
    background: transparentize($blue, 0.8);
  }

  &:not(:last-child) {
    margin-bottom: 15px;
  }

  &.selected {
    background: transparentize($blue, 0.8);
  }

  &.completed {
    background: transparentize($green, 0.8);

    .sidebar-list-completed-state {
      background: $green;
    }
  }

  svg {
    margin-right: 14px;
  }

  .sidebar-list-completed-state {
    display: flex;
    align-items: center;
    justify-content: center;
    height: 22px;
    width: 22px;
    border-radius: 100px;
    font-size: 10px;
    color: #fff;
    background: $blue;
    margin-left: auto;
  }
}
</style>