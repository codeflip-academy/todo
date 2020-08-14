<template>
  <b-list-group class="todo-list-items">
    <Draggable
      v-model="itemsLayout"
      @end="dispatchUpdateItemsLayout"
      handle=".item-handle"
      v-if="!loadingItems"
    >
      <!-- Replace items loop with itemsLayout and find items with items.find(i => i.id === position) -->
      <TodoListItem
        v-for="item in items"
        :key="item.id"
        :item="item"
        @checkbox-clicked="dispatchSetItemCompletedState"
        @delete-item="dispatchDeleteItem"
      ></TodoListItem>
    </Draggable>

    <b-list-group-item v-if="items.length === 0">Add an item to get started.</b-list-group-item>

    <AddTodoListItemForm class="mt-3" :todoListId="todoListId" @add-item="dispatchAddItem"></AddTodoListItemForm>
  </b-list-group>
</template>

<script>
import axios from "axios";

import Draggable from "vuedraggable";
import TodoListItem from "./TodoListItem";
import AddTodoListItemForm from "./AddTodoListItemForm.vue";

export default {
  name: "TodoListItems",
  props: ["todoListId"],
  components: {
    Draggable,
    TodoListItem,
    AddTodoListItemForm,
  },
  data() {
    return {
      items: [],
      itemsLayout: [],
      loadingItems: true,
    };
  },
  methods: {
    async dispatchGetItems() {
      this.commitSetLoadingItemsState(true);

      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListId}/todos`,
      });

      this.commitSetItems(response.data);

      this.commitSetLoadingItemsState(false);
    },
    async dispatchAddItem(item) {
      const response = await axios({
        method: "POST",
        url: `api/lists/${this.todoListId}/todos`,
        data: JSON.stringify(item),
        headers: {
          "content-type": "application/json",
        },
      });

      this.commitAddItem(response.data);
    },
    async dispatchSetItemCompletedState({ itemId, completed }) {
      this.commitSetItemCompletedState(itemId, completed);

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListId}/todos/${itemId}/completed`,
        data: JSON.stringify({ completed }),
        headers: {
          "content-type": "application/json",
        },
      });
    },
    async dispatchDeleteItem(itemId) {
      this.commitDeleteItem(itemId);

      await axios({
        method: "DELETE",
        url: `api/lists/${this.todoListId}/todos/${itemId}`,
      });
    },
    async dispatchUpdateItemsLayout() {},
    commitSetLoadingItemsState(state) {
      this.loadingItems = state;
    },
    commitSetItems(items) {
      this.items = items;
    },
    commitAddItem(item) {
      this.items.unshift(item);
    },
    commitSetItemCompletedState(itemId, completed) {
      this.items[
        this.items.findIndex((i) => i.id === itemId)
      ].completed = completed;

      this.triggerTodoListCompletedEvent();
    },
    commitDeleteItem(itemId) {
      this.items.splice(
        this.items.findIndex((i) => i.id == itemId),
        1
      );
    },
    triggerTodoListCompletedEvent() {
      const listCompleted =
        this.items.length > 0 && this.items.every((item) => item.completed);

      if (listCompleted) {
        this.$emit("todo-list-completed");
      } else {
        this.$emit("todo-list-uncompleted");
      }
    },
  },
  async created() {
    await this.dispatchGetItems();
  },
  watch: {
    items() {
      this.triggerTodoListCompletedEvent();
    },
  },
};
</script>