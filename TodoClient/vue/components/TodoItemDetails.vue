<template>
  <b-col>
    <div class="item-content">
      <b-form @submit.prevent="saveChanges">
        <b-form-group class="mb-0">
          <b-input type="text" class="item-name item-name-input" v-model.lazy="item.name"></b-input>
          <b-button type="submit" class="sr-only">Save</b-button>
        </b-form-group>
      </b-form>

      <div class="item-meta">
        <b-form-group v-if="plan.canAddDueDates">
          <b-form-datepicker id="due-date" v-model="item.dueDate" class="item-due-date"></b-form-datepicker>
        </b-form-group>
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
import { mapState } from "vuex";

import SubItems from "./SubItems";

export default {
  name: "TodoItemDetails",
  props: ["item"],
  components: {
    SubItems,
  },
  data() {
    return {
      edited: false,
    };
  },
  methods: {
    sendSubItemCountChangedEvent({ disabled }) {
      this.$emit("sub-item-count-changed", { disabled });
    },
    saveChanges() {
      this.edited = true;
    },
  },
  computed: mapState({
    plan: (state) => state.plan,
  }),
};
</script>

<style lang="scss">
$light-gray: #f5f6f7;
$gray: #455a64;
$blue: #1e88e5;
$orange: #ff7043;
$green: #4caf50;
$red: #b71c1c;

.item-content {
  padding: 40px 50px;

  .item-name {
    font-size: 24px;
    font-weight: bold;
    color: $gray;
    line-height: 1.1;
  }

  .item-meta {
    margin-bottom: 36px;

    .item-due-date {
      font-size: 13px;
      color: darken($light-gray, 40%) !important;
    }
  }

  .b-form-btn-label-control.form-control > #due-date {
    padding-right: 6px;
  }

  .form-group .form-control#due-date__value_:hover {
    border: none;
  }

  .item-notes {
    color: $gray;
    font-size: 14px;
    line-height: 1.5;
    margin-bottom: 36px;
  }
}

.form-group {
  color: $gray;
  margin-bottom: 0;
  margin-bottom: 5px;

  .form-control {
    border-radius: 0;
    border: 1px solid transparent;

    &:hover {
      border: 1px solid #ddd;
    }

    &:focus {
      border-color: $blue;
      box-shadow: none;
    }
  }
}

.btn {
  border-radius: 0;
}
</style>