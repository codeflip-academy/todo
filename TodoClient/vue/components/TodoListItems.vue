<template>
  <b-list-group class="todo-list-items">
    <b-list-group-item v-if="todoListItems.length < 1">Add an item to get started.</b-list-group-item>

    <transition-group name="fadeLeft">
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
      layout: []
    };
  },
  created() {
    this.getLayout();
  },
  mounted() {
    this.$store.state.connection.on("ListLayoutChanged", listId =>
      this.refreshLayout(listId)
    );
    this.$store.state.connection.on("ItemTrashed", (listId, item) =>
      this.refreshLayout(listId)
    );
  },
  methods: {
    getLayout() {
      axios({
        method: "GET",
        url: `api/lists/${this.listId}/layout`
      }).then(response => {
        this.layout = response.data;
      });
    },
    updateLayout(e) {
      let position = e.newIndex;
      let itemId = e.item.getAttribute("data-id");

      axios({
        method: "PUT",
        url: `api/lists/${this.listId}/layout`,
        data: JSON.stringify({ itemId, position }),
        headers: {
          "content-type": "application/json"
        }
      });
    },
    refreshLayout(listId) {
      if (this.listId === listId) {
        this.getLayout();
      }
    }
  },
  components: {
    Draggable,
    TodoListItem
  }
};
</script>