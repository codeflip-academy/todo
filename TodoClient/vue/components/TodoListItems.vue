<template>
  <b-list-group class="todo-list-items">
    <Draggable v-model="layout" @end="updateLayout" handle=".item-handle">
      <TodoListItem
        v-for="position in layout"
        :key="position"
        :todoListItem="todoListItems.find(x => x.id === position)"
        class="todo-list-item"
      ></TodoListItem>
    </Draggable>
  </b-list-group>
</template>

<script>
import axios from "axios";
import Draggable from "vuedraggable";
import TodoListItem from "./TodoListItem";

export default {
  name: "TodoListItems",
  props: ["listId", "todoListItems"],
  data() {
    return {
      layout: [],
    };
  },
  components: {
    Draggable,
    TodoListItem,
  },
};
</script>