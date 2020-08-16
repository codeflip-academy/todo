<template>
  <div class="sub-items-wrapper">
    <b-list-group v-if="!loadingSubItems">
      <Draggable
        handle=".sub-item-handle"
        v-model="subItemsLayout"
        @end="dispatchUpdateSubItemPosition"
      >
        <SubItem
          v-for="subItemId in subItemsLayout"
          :key="subItemId"
          :subItem="subItems.find(s => s.id == subItemId)"
          :listId="todoListItem.listId"
          @checkbox-clicked="dispatchSetSubItemCompletedState"
          @update-sub-item-name="dispatchUpdateSubItemName"
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
    await this.dispatchGetSubItemsAndLayout();
  },
  mounted() {
    // Sub-item created
    this.$store.state.connection.on("SubItemCreated", (subItem) => {
      if (this.subItemsBelongToItem(subItem.listItemId)) {
        this.commitAddSubItem(subItem);
      }
    });

    // Sub-item trashed
    this.$store.state.connection.on("SubItemTrashed", (itemId, subItem) => {
      if (this.subItemsBelongToItem(itemId)) {
        this.commitDeleteSubItem(subItem.id);
      }
    });

    // Sub-item completed state changed
    this.$store.state.connection.on(
      "SubItemCompletedStateChanged",
      (subItem) => {
        if (this.subItemsBelongToItem(subItem.listItemId)) {
          this.commitSetSubItemCompletedState(subItem.id, subItem.completed);
        }
      }
    );
  },
  methods: {
    async dispatchGetSubItemsAndLayout() {
      await this.dispatchGetSubItemsLayout();
      await this.dispatchGetSubItems();
    },
    async dispatchGetSubItems() {
      this.commitSetLoadingSubItemsState(true);

      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/subitems`,
      });

      this.commitSetSubItems(response.data);

      this.commitSetLoadingSubItemsState(false);
    },
    async dispatchGetSubItemsLayout() {
      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/layout`,
      });

      this.commitSetSubItemsLayout(response.data.layout);
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
    async dispatchUpdateSubItemName({ subItemId, subItemName }) {
      this.commitUpdateSubItemName(subItemId, subItemName);

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/subitems/${subItemId}`,
        headers: { "content-type": "application/json" },
        data: JSON.stringify({ name: subItemName }),
      });
    },
    async dispatchUpdateSubItemPosition(event) {
      const subItemId = event.item.getAttribute("data-id");
      const position = event.newIndex;

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/layout`,
        headers: {
          "content-type": "application/json",
        },
        data: JSON.stringify({ subItemId, position }),
      });
    },
    commitSetLoadingSubItemsState(state) {
      this.loadingSubItems = state;
    },
    commitDeleteSubItem(subItemId) {
      this.commitRemoveSubItemLayoutPosition(subItemId);

      this.subItems.splice(
        this.subItems.findIndex((s) => s.id == subItemId),
        1
      );

      this.triggerSubItemsCompletedEvent();
    },
    commitSetSubItems(subItems) {
      this.subItems = subItems;
    },
    commitSetSubItemsLayout(layout) {
      this.subItemsLayout = layout;
    },
    commitAddSubItemLayoutPosition(subItemId) {
      this.subItemsLayout.unshift(subItemId);
    },
    commitRemoveSubItemLayoutPosition(subItemId) {
      this.subItemsLayout.splice(
        this.subItemsLayout.findIndex((l) => l == subItemId),
        1
      );
    },
    commitAddSubItem(subItem) {
      this.subItems.unshift(subItem);

      this.commitAddSubItemLayoutPosition(subItem.id);

      this.triggerSubItemsCompletedEvent();
    },
    commitUpdateSubItemName(subItemId, subItemName) {
      this.subItems[
        this.subItems.findIndex((s) => s.id == subItemId)
      ].name = subItemName;
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
    subItemsBelongToItem(itemId) {
      return this.todoListItem.id === itemId;
    },
  },
  watch: {
    subItems() {
      this.triggerSubItemCountChangedEvent();
    },
  },
};
</script>