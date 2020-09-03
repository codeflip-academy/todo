<template>
  <b-col>
    <div class="item-content">
      <b-form-group class="mb-0">
        <b-input
          type="text"
          maxlength="50"
          class="item-name item-name-input"
          v-model="form.name"
          @keyup.enter="$event.target.blur()"
          @change="sendItemEditedEvent"
          :state="itemNameValid"
        ></b-input>
      </b-form-group>

      <div class="item-meta mb-3">
        <b-form-group v-if="plan.canAddDueDates">
          <b-form-datepicker
            id="due-date"
            v-model="form.dueDate"
            class="item-due-date"
            @input="sendItemEditedEvent"
          ></b-form-datepicker>
        </b-form-group>
      </div>
      <b-form-group class="item-notes">
        <b-form-textarea
          id="item-notes"
          :state="itemNotesValid"
          maxlength="200"
          v-model="form.notes"
          @change="sendItemEditedEvent"
          placeholder="Add notes here..."
          rows="1"
          max-rows="12"
        ></b-form-textarea>
      </b-form-group>

      <SubItems
        :key="item.id"
        :todoListItem="item"
        @sub-items-completed="sendSubItemsCompletedEvent"
        @sub-items-uncompleted="sendSubItemsUncompletedEvent"
        @sub-item-count-changed="sendSubItemCountChangedEvent"
      ></SubItems>
    </div>
  </b-col>
</template>

<script>
import Vue from "vue";
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
      form: Object.assign({}, this.item),
    };
  },
  watch: {
    item: function () {
      this.form = Object.assign({}, this.item);
    },
  },
  methods: {
    sendSubItemCountChangedEvent({ hasSubItems }) {
      this.$emit("sub-item-count-changed", {
        itemId: this.item.id,
        hasSubItems,
      });
    },
    sendItemEditedEvent() {
      if (this.itemNameValid === null && this.itemNotesValid === null) {
        this.$emit("item-edited", JSON.stringify(this.form));
      }
    },
    sendSubItemsCompletedEvent() {
      this.$emit("sub-items-completed", {
        itemId: this.item.id,
        completed: true,
      });
    },
    sendSubItemsUncompletedEvent() {
      this.$emit("sub-items-uncompleted", {
        itemId: this.item.id,
        completed: false,
      });
    },
  },
  computed: {
    ...mapState({
      plan: (state) => state.plan,
    }),
    itemNameValid() {
      return this.form.name.length > 0 && this.form.name.length <= 50
        ? null
        : false;
    },
    itemNotesValid() {
      return !this.form.notes || this.form.notes.length <= 200 ? null : false;
    },
  },
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

    textarea {
      overflow-y: auto !important;
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
}

.btn {
  border-radius: 0;
}
</style>