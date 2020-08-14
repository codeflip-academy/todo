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
      loadingSubItems: false,
    };
  },
  methods: {
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
    async dispatchUpdateSubItemPosition() {},
    commitAddSubItem(subItem) {
      this.subItems.unshift(subItem);
    },
  },
  computed: {},
  watch: {},
};
</script>