<template>
  <div>
    <AddTodoListItemForm :todoListId="todoListId" @add-item="sendAddItemEvent"></AddTodoListItemForm>

    <Draggable v-model="itemsLayout" delay="100" @end="sendUpdateItemPositionEvent">
      <TodoListItem
        v-for="itemId in itemsLayout"
        :key="itemId"
        :item="items.find(i => i.id === itemId)"
        @item-selected="sendItemSelectedEventToList"
        @checkbox-clicked="sendCheckboxClickedEvent"
        @item-edited="sendItemEditedEvent"
        @delete-item="sendDeleteItemEvent"
        @sub-item-count-changed="sendSubItemCountChangedEvent"
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
  props: ["todoListId", "items", "itemsLayout"],
  components: {
    Draggable,
    TodoListItem,
    AddTodoListItemForm,
  },
  methods: {
    sendItemSelectedEventToList(item) {
      this.$emit("item-selected", item);
    },
    sendAddItemEvent(item) {
      this.$emit("add-item", item);
    },
    sendUpdateItemPositionEvent(event) {
      this.$emit("update-item-position", event);
    },
    sendCheckboxClickedEvent({ itemId, completed }) {
      this.$emit("checkbox-clicked", { itemId, completed });
    },
    sendItemEditedEvent(item) {
      this.$emit("item-edited", item);
    },
    sendDeleteItemEvent(itemId) {
      this.$emit("delete-item", itemId);
    },
    sendSubItemCountChangedEvent({ itemId, hasSubItems }) {
      this.$emit("sub-item-count-changed", { itemId, hasSubItems });
    },
  },
};
</script>