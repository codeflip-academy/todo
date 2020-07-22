<template>
  <b-list-group-item
    :data-id="todoListItem.id"
    class="todo-item bg-light"
    :class="{ 'align-items-center': !todoListItem.dueDate && !todoListItem.notes }"
  >
    <div class="item-handle mr-2">
      <b-icon-list></b-icon-list>
    </div>

    <b-form-checkbox
      :disabled="subItems && subItems.length > 0"
      class="todo-item-checkbox"
      v-model="itemCompletedState"
    ></b-form-checkbox>

    <div class="todo-item-details">
      <div
        class="todo-item-name"
        :class="{ 'mb-0': !todoListItem.dueDate && !todoListItem.notes }"
      >{{ todoListItem.name }}</div>
      <div class="todo-item-due-date" v-if="todoListItem.dueDate">
        <b-icon-calendar></b-icon-calendar>
        {{ todoListItem.dueDate | formatDate }}
      </div>
      <div class="todo-item-notes" v-if="todoListItem.notes">
        <b-icon-text-left></b-icon-text-left>
        {{ todoListItem.notes | truncate(30, '...') }}
      </div>
    </div>

    <div class="todo-item-options">
      <b-button-group>
        <b-button variant="info" @click="$bvModal.show(`modal-${todoListItem.id}`)">View</b-button>
        <b-button
          variant="danger"
          @click="$store.dispatch('deleteItem', { item: todoListItem })"
        >Delete</b-button>
      </b-button-group>
      <EditTodoItemForm :todoListItem="todoListItem"></EditTodoItemForm>
    </div>
  </b-list-group-item>
</template>

<script>
import moment from "moment";
import EditTodoItemForm from "./EditTodoItemForm";

export default {
  name: "TodoListItem",
  props: ["todoListItem"],
  data() {
    return {
      subItems: this.$store.getters.getSubItemsByItemId(this.todoListItem.id)
    };
  },
  components: {
    EditTodoItemForm
  },
  computed: {
    itemCompletedState: {
      get() {
        return this.$store.getters.getItemCompletedState(
          this.todoListItem.listId,
          this.todoListItem.id
        );
      },
      set(value) {
        this.$store.dispatch("toggleItemCompletedState", {
          listId: this.todoListItem.listId,
          itemId: this.todoListItem.id,
          completed: value
        });
      }
    }
  },
  filters: {
    formatDate: function(value) {
      return moment(value).format("MM/D/YYYY");
    },
    truncate: function(text, length, suffix) {
      return text.substring(0, length) + suffix;
    }
  }
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