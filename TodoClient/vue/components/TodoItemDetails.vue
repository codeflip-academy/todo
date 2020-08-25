<template>
  <b-col>
    <div class="item-content">
      <h2 class="item-name">{{ item.name }}</h2>
      <div class="item-meta">
        <div class="item-due-date" v-if="item.dueDate">
          <b-icon-calendar></b-icon-calendar>
          {{ item.dueDate | formatDate }}
        </div>
      </div>
      <div class="item-notes" v-if="item.notes">{{ item.notes }}</div>

      <SubItems
        :key="item.id"
        :todoListItem="item"
        @sub-items-completed="$emit('sub-items-completed')"
        @sub-items-uncompleted="$emit('sub-items-uncompleted')"
        @sub-item-count-changed="sendSubItemCountChangedEvent"
      ></SubItems>
    </div>
  </b-col>
</template>

<script>
import moment from "moment";

import SubItems from "./SubItems";

export default {
  name: "TodoItemDetails",
  props: ["item"],
  components: {
    SubItems,
  },
  filters: {
    formatDate: function (value) {
      return moment(value).format("MM/D/YYYY");
    },
    truncate: function (text, length, suffix) {
      return text.substring(0, length) + suffix;
    },
  },
  methods: {
    sendSubItemCountChangedEvent({ disabled }) {
      this.$emit("sub-item-count-changed", { disabled });
    },
  },
};
</script>

<style lang="scss" scoped>
</style>