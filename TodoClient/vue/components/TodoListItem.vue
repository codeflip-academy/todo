<template>
  <div :data-id="item.id">
    <EditTodoItemForm
      :todoListItem="item"
      @item-edited="sendItemEditedEvent"
      @sub-items-completed="sendCheckboxClickedEvent(true)"
      @sub-items-uncompleted="sendCheckboxClickedEvent(false)"
      @sub-item-count-changed="sendSubItemCountChangedEvent"
    ></EditTodoItemForm>

    <div class="item" @click="$emit('item-selected', item)">
      <div class="item-checkbox">
        <label
          :for="item.id"
          class="custom-checkbox"
          :class="{ 'checked': itemCompletedState, }"
          :disabled="item.hasSubItems"
        >
          <b-icon-check></b-icon-check>
          <input :id="item.id" v-model="itemCompletedState" type="checkbox" class="sr-only" />
        </label>
      </div>
      <div class="item-details">
        <div class="item-name">{{ item.name }}</div>
        <div class="item-content-preview">
          <div class="item-due-date" v-if="item.dueDate">
            <b-icon-calendar></b-icon-calendar>
            {{ item.dueDate | formatDate }}
          </div>
          <div class="item-notes" v-if="item.notes">
            <b-icon-file-earmark-text></b-icon-file-earmark-text>
          </div>
        </div>
      </div>
      <div class="item-controls">
        <b-button variant="link" class="btn-view" @click="$bvModal.show(`modal-${item.id}`)">
          <b-icon-info-circle></b-icon-info-circle>
        </b-button>
        <b-button variant="link" class="btn-trash" @click="$emit('delete-item', item.id)">
          <b-icon-trash></b-icon-trash>
        </b-button>
      </div>
    </div>
  </div>
</template>

<script>
import moment from "moment";
import EditTodoItemForm from "./EditTodoItemForm";

export default {
  name: "TodoListItem",
  props: ["item"],
  components: {
    EditTodoItemForm,
  },
  computed: {
    itemCompletedState: {
      get() {
        return this.item.completed;
      },
      set(val) {
        if (val !== null) {
          this.sendCheckboxClickedEvent(val);
        }
      },
    },
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
    sendItemEditedEvent(item) {
      this.$emit("item-edited", item);
    },
    sendCheckboxClickedEvent(completed) {
      this.$emit("checkbox-clicked", {
        itemId: this.item.id,
        completed: completed,
      });
    },
    sendSubItemCountChangedEvent({ disabled }) {
      this.$emit("sub-item-count-changed", {
        itemId: this.item.id,
        hasSubItems: disabled,
      });
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

.item {
  padding: 25px;
  background: $light-gray;
  display: flex;
  align-items: center;
  transition: background-color 0.3s ease;
  border-bottom: 1px solid darken($light-gray, 5%);

  &:hover {
    background-color: lighten($light-gray, 2%);
    cursor: pointer;
  }

  &:active {
    background-color: lighten($light-gray, 10%);
  }

  .custom-checkbox {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 40px;
    height: 40px;
    border-radius: 100px;
    background-color: transparent;
    border: solid 1px $blue;
    margin-bottom: 0;
    margin-right: 16px;
    font-size: 36px;
    color: #fff;
    transition: all 0.2s ease;

    svg {
      transition: opacity 0.2s ease;
      opacity: 0;
    }

    &:hover {
      box-shadow: 0 10px 30px transparentize($blue, 0.7);
      cursor: pointer;
    }

    &.checked {
      background-color: $blue;
      box-shadow: 0 10px 30px transparentize($blue, 0.7);

      svg {
        opacity: 1;
      }
    }

    &[disabled="disabled"] {
      opacity: 0.4;
      pointer-events: none;
    }
  }

  .item-details {
    color: darken($light-gray, 40%);
    font-size: 13px;

    .item-name {
      color: $gray;
      font-size: 16px;
    }

    .item-due-date {
      margin-right: 16px;

      svg {
        margin-right: 6px;
      }
    }
  }

  .item-content-preview {
    display: flex;
  }

  .item-controls {
    margin-left: auto;

    .btn-trash {
      color: $gray;
      opacity: 0.5;
      transition: all 0.3s ease;

      &:hover {
        color: red;
        opacity: 1;
      }
    }

    .btn-view {
      color: $gray;
      opacity: 0.5;
      transition: all 0.3s ease;

      &:hover {
        color: $blue;
        opacity: 1;
      }
    }
  }
}
</style>