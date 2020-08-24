<template>
  <div>
    <AddTodoListItemForm :todoListId="todoListId" @add-item="dispatchAddItem"></AddTodoListItemForm>

    <Draggable
      v-model="itemsLayout"
      v-if="!loadingItems"
      delay="200"
      @end="dispatchUpdateItemPosition"
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
  </div>
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
    async dispatchGetItem(itemId) {
      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListId}/todos/${itemId}`,
      });

      return response.data;
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
    itemsBelongToList(todoListId) {
      return this.todoListId === todoListId;
    },
  },
  async created() {
    await this.dispatchGetItemsAndLayout();
  },
  async mounted() {
    // Item created
    this.$store.state.connection.on("ItemCreated", (todoListId, item) => {
      if (this.itemsBelongToList(todoListId)) {
        this.commitAddItem(item);
      }
    });

    // Item trashed
    this.$store.state.connection.on("ItemTrashed", (todoListId, item) => {
      if (this.itemsBelongToList(todoListId)) {
        this.commitDeleteItem(item.id);
      }
    });

    // Item updated
    this.$store.state.connection.on("ItemUpdated", (item) => {
      if (this.itemsBelongToList(item.listId)) {
        this.commitUpdateItem(item);
      }
    });

    // Item completed state changed
    this.$store.state.connection.on("ItemCompleted", (item) => {
      if (this.itemsBelongToList(item.listId)) {
        this.commitSetItemCompletedState(item.id, item.completed);
      }
    });

    // Layout changed
    this.$store.state.connection.on("ListLayoutChanged", async (todoListId) => {
      if (this.itemsBelongToList(todoListId)) {
        await this.dispatchGetItemsLayout();
      }
    });

    // Sub-item created
    this.$store.state.connection.on("SubItemCreated", async (subItem) => {
      const itemIndex = this.items.findIndex(
        (i) => i.id === subItem.listItemId
      );
      const item = this.items[itemIndex];

      if (itemIndex !== -1) {
        if (!item.hasSubItems) {
          this.commitUpdateHasSubItems({
            itemId: item.id,
            hasSubItems: true,
          });
        }
      }
    });

    // Sub-item trashed
    this.$store.state.connection.on(
      "SubItemTrashed",
      async (itemId, subItem) => {
        if (this.items.findIndex((i) => i.id === itemId) !== -1) {
          const item = await this.dispatchGetItem(itemId);

          if (!item.hasSubItems) {
            this.commitUpdateHasSubItems({
              itemId: item.id,
              hasSubItems: false,
            });
          }
        }
      }
    );
  },
  watch: {
    items() {
      this.triggerTodoListCompletedEvent();
    },
  },
};
</script>