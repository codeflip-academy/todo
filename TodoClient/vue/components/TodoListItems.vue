<template>
  <b-list-group class="todo-list-items">
    <Draggable
      v-model="itemsLayout"
      @end="dispatchUpdateItemPosition"
      handle=".item-handle"
      v-if="!loadingItems"
    >
      <TodoListItem
        v-for="itemId in itemsLayout"
        :key="itemId"
        :item="items.find(i => i.id === itemId)"
        @checkbox-clicked="dispatchSetItemCompletedState"
        @item-edited="dispatchUpdateItem"
        @delete-item="dispatchDeleteItem"
        @sub-item-count-changed="commitUpdateHasSubItems"
      ></TodoListItem>
    </Draggable>

    <b-list-group-item v-if="items.length === 0">Add an item to get started.</b-list-group-item>

    <AddTodoListItemForm class="mt-3" :todoListId="todoListId" @add-item="dispatchAddItem"></AddTodoListItemForm>
  </b-list-group>
</template>

<script>
import Vue from "vue";
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
    async dispatchGetItemsAndLayout() {
      await this.dispatchGetItemsLayout();
      await this.dispatchGetItems();
    },
    async dispatchGetItemsLayout() {
      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListId}/layout`,
      });

      this.commitSetItemsLayout(response.data);
    },
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
    async dispatchUpdateItem(item) {
      this.commitUpdateItem(item);

      await axios({
        method: "PUT",
        url: `api/lists/${item.listId}/todos/${item.id}`,
        data: JSON.stringify({
          name: item.name,
          notes: item.notes,
          dueDate: item.dueDate,
        }),
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
    async dispatchUpdateItemPosition(event) {
      let itemId = event.item.getAttribute("data-id");
      let position = event.newIndex;

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListId}/layout`,
        data: JSON.stringify({ itemId, position }),
        headers: {
          "content-type": "application/json",
        },
      });
    },
    commitSetLoadingItemsState(state) {
      this.loadingItems = state;
    },
    commitSetItems(items) {
      this.items = items;
    },
    commitAddItem(item) {
      this.items.unshift(item);
      this.commitAddItemLayoutPosition(item.id);
    },
    commitAddItemLayoutPosition(itemId) {
      this.itemsLayout.unshift(itemId);
    },
    commitSetItemCompletedState(itemId, completed) {
      this.items[
        this.items.findIndex((i) => i.id === itemId)
      ].completed = completed;

      this.triggerTodoListCompletedEvent();
    },
    commitUpdateItem(item) {
      const itemIndex = this.items.findIndex((i) => i.id === item.id);

      Vue.set(this.items, itemIndex, item);
    },
    commitDeleteItem(itemId) {
      this.commitRemoveItemLayoutPosition(itemId);

      this.items.splice(
        this.items.findIndex((i) => i.id === itemId),
        1
      );
    },
    commitRemoveItemLayoutPosition(itemId) {
      this.itemsLayout.splice(
        this.itemsLayout.findIndex((l) => l === itemId),
        1
      );
    },
    commitUpdateHasSubItems({ itemId, hasSubItems }) {
      this.items[
        this.items.findIndex((i) => i.id == itemId)
      ].hasSubItems = hasSubItems;
    },
    commitSetItemsLayout(itemsLayout) {
      this.itemsLayout = itemsLayout;
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
    itemBelongsToList(todoListId) {
      return this.todoListId === todoListId;
    },
  },
  async created() {
    await this.dispatchGetItemsAndLayout();
  },
  mounted() {
    // Item created
    this.$store.state.connection.on("ItemCreated", (todoListId, item) => {
      if (this.itemBelongsToList(todoListId)) {
        this.commitAddItem(item);
      }
    });
  },
  watch: {
    items() {
      this.triggerTodoListCompletedEvent();
    },
  },
};
</script>