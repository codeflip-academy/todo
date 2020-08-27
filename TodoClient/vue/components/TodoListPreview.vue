<template>
  <div v-if="todoList.role === 2 || todoList.role === 3" class="sidebar-list-wrapper">
    <b-button
      class="sidebar-list"
      :to="{ path: `/lists/${todoList.id}` }"
      :class="{ 'selected': $route.params.todoListId == todoList.id, 'completed': todoList.completed }"
    >
      {{ todoList.listTitle }}
      <div class="sidebar-list-completed-state">{{ todoList.incompleteCount }}</div>
    </b-button>
  </div>
</template>

<script>
import axios from "axios";

export default {
  props: ["todoList"],
  methods: {
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

.sidebar-list-wrapper {
  &:not(:last-child) {
    margin-bottom: 10px;
  }
}

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
  &:focus,
  &:active {
    color: $gray !important;
    background: transparentize($blue, 0.8) !important;
    box-shadow: none !important;

    &.completed {
      background: transparentize($green, 0.8) !important;
    }
  }

  &.selected {
    background: transparentize($blue, 0.8);
  }

  &.completed {
    &.selected {
      background: transparentize($green, 0.8);
    }

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