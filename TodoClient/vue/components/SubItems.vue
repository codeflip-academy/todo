<template>
  <div class="sub-items-wrapper">
    <b-list-group v-if="!loadingSubItems">
      <Draggable
        handle=".sub-item-handle"
        v-model="subItemsLayout"
        @end="dispatchUpdateSubItemPosition"
      >
        <SubItem
          v-for="subItem in subItems"
          :key="subItem.id"
          :subItem="subItem"
          :listId="todoListItem.listId"
          @checkbox-clicked="dispatchSetSubItemCompletedState"
          @delete-sub-item="dispatchDeleteSubItem"
        ></SubItem>
        <b-list-group-item v-if="subItems.length < 1">There are no sub-items.</b-list-group-item>
      </Draggable>
    </b-list-group>
    <AddSubItemForm :todoListItem="todoListItem" @add-sub-item="dispatchAddSubItem"></AddSubItemForm>
  </div>
</template>

<script>
import axios from "axios";

import Draggable from "vuedraggable";
import SubItem from "./SubItem";
import AddSubItemForm from "./AddSubItemForm";

export default {
  props: ["todoListItem"],
  components: {
    Draggable,
    SubItem,
    AddSubItemForm,
  },
  data() {
    return {
      subItems: [],
      subItemsLayout: [],
      loadingSubItems: true,
    };
  },
  async created() {
    await this.dispatchGetSubItems();
  },
  methods: {
    async dispatchGetSubItems() {
      this.commitSetLoadingSubItemsState(true);

      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/subitems`,
      });

      this.commitSetSubItems(response.data);

      this.commitSetLoadingSubItemsState(false);
    },
    async dispatchAddSubItem(subItemName) {
      const response = await axios({
        method: "POST",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/subitems`,
        headers: { "content-type": "application/json" },
        data: JSON.stringify({ name: subItemName }),
      });

      const newSubItem = response.data;

      this.commitAddSubItem(newSubItem);
    },
    async dispatchSetSubItemCompletedState({ subItemId, completed }) {
      this.commitSetSubItemCompletedState(subItemId, completed);

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/subitems/${subItemId}/completed`,
        headers: {
          "content-type": "application/json",
        },
        data: completed,
      });
    },
    async dispatchDeleteSubItem(subItemId) {
      this.commitDeleteSubItem(subItemId);

      await axios({
        method: "DELETE",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/subitems/${subItemId}`,
      });
    },
    async dispatchUpdateSubItemPosition() {},
    commitSetLoadingSubItemsState(state) {
      this.loadingSubItems = state;
    },
    commitDeleteSubItem(subItemId) {
      this.subItems.splice(
        this.subItems.findIndex((s) => s.id == subItemId),
        1
      );
    },
    commitSetSubItems(subItems) {
      this.subItems = subItems;
    },
    commitAddSubItem(subItem) {
      this.subItems.unshift(subItem);
    },
    commitSetSubItemCompletedState(subItemId, completed) {
      this.subItems[
        this.subItems.findIndex((s) => s.id == subItemId)
      ].completed = completed;

      this.triggerSubItemsCompletedEvent();
    },
    triggerSubItemsCompletedEvent() {
      const hasSubItems = this.subItems.length > 0;
      const subItemsCompleted =
        hasSubItems && this.subItems.every((s) => s.completed);

      if (subItemsCompleted) {
        this.$emit("sub-items-completed");
      } else if (hasSubItems) {
        this.$emit("sub-items-uncompleted");
      }
    },
    triggerSubItemCountChangedEvent() {
      if (this.subItems.length > 0) {
        this.$emit("sub-item-count-changed", { disabled: true });
      } else {
        this.$emit("sub-item-count-changed", { disabled: false });
      }
    },
  },
  watch: {
    subItems() {
      this.triggerSubItemCountChangedEvent();
      this.triggerSubItemsCompletedEvent();
    },
  },
};
</script>