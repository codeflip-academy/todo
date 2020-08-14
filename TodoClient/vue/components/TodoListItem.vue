<template>
  <b-list-group-item
    class="todo-item bg-light"
    :class="{ 'align-items-center': !item.dueDate && !item.notes }"
    :data-id="item.id"
  >
    <div class="item-handle mr-2">
      <b-icon-list></b-icon-list>
    </div>

    <b-form-checkbox
      :disabled="/* subItems && subItems.length > 0 */ false"
      class="todo-item-checkbox"
      v-model="itemCompletedState"
    ></b-form-checkbox>

    <div class="todo-item-details">
      <div class="todo-item-name" :class="{ 'mb-0': !item.dueDate && !item.notes }">{{ item.name }}</div>
      <div class="todo-item-due-date" v-if="item.dueDate">
        <b-icon-calendar></b-icon-calendar>
        {{ item.dueDate | formatDate }}
      </div>
      <div class="todo-item-notes" v-if="item.notes">
        <b-icon-text-left></b-icon-text-left>
        {{ item.notes | truncate(30, '...') }}
      </div>
    </div>

    <div class="todo-item-options">
      <b-button-group>
        <b-button variant="info" @click="$bvModal.show(`modal-${item.id}`)">View</b-button>
        <b-button variant="danger" @click="$emit('delete-item', item.id)">Delete</b-button>
      </b-button-group>
      <EditTodoItemForm :todoListItem="item"></EditTodoItemForm>
    </div>
  </b-list-group-item>
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
        this.$emit("checkbox-clicked", {
          itemId: this.item.id,
          completed: val,
        });
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
};
</script>

<style lang="scss" scoped>
.todo-item {
  display: flex !important;
  align-items: flex-start;
  flex-wrap: wrap;

  .item-handle:hover {
    cursor: move;
  }

  .todo-item-details {
    flex: 1 1 auto;
    font-family: "Nunito", sans-serif;

    .todo-item-name {
      display: block;
      font-weight: bold;
      font-size: 18px;
      line-height: 1.3;
      margin-bottom: 7px;
    }

    .todo-item-due-date,
    .todo-item-notes {
      font-size: 14px;
    }
  }

  .todo-item-options {
    align-self: center;
    margin-top: 14px;
    margin-left: 31px;

    @media screen and (min-width: 521px) {
      margin: 0;
    }
  }
}
</style>