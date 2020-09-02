<template>
  <RouterView :key="$route.fullPath" v-if="!loadingTodoLists"></RouterView>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import todoListImage from "../../images/todo-list-bg.jpg";
import Draggable from "vuedraggable";
import TodoLists from "../components/TodoLists";
import TodoList from "../components/TodoList";
import Invitations from "../components/Invitations";

export default {
  name: "Home",
  data() {
    return {
      todoListImage: todoListImage,
    };
  },
  computed: mapState({
    loadingTodoLists: (state) => state.loadingTodoLists,
  }),
  components: {
    TodoLists,
    TodoList,
    Draggable,
    Invitations,
  },
};
</script>

<style lang="scss">
$light-gray: #f5f6f7;
$gray: #455a64;
$blue: #1e88e5;
$orange: #ff7043;
$green: #4caf50;
$red: #b71c1c;

.item-categories {
  margin-left: 30px;
  margin-top: 40px;
}

.item-category {
  display: flex;
  align-items: center;
  color: $gray;

  .item-category-icon {
    margin-right: 15px;
  }

  &:not(:last-child) {
    margin-bottom: 20px;
  }
}

.todo-list-bg {
  background-position: center;
  background-size: 70%;
  background-repeat: no-repeat;

  &.list-selected {
    background-image: none !important;
  }
}

.account-dropdown {
  .btn {
    display: flex;
    align-items: center;
    padding: 0px;
    background: transparent;
    border: none;

    &::after {
      display: none;
    }

    &:hover,
    &:focus,
    &:active {
      background: transparent !important;
      box-shadow: none;
    }
  }

  .dropdown-menu {
    border-radius: 0;
    margin-top: 12px;
  }
}

.show > .btn-secondary.dropdown-toggle {
  background-color: transparent !important;
}

.secondary:not(:disabled):not(.disabled):active:focus,
.show > .btn-secondary.dropdown-toggle:focus {
  box-shadow: none !important;
}
</style>