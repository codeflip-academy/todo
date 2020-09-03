<template>
  <b-row no-gutters>
    <b-col>
      <div class="list-wrapper">
        <Confetti v-if="todoList.completed"></Confetti>

        <header class="list-header">
          <b-row class="align-items-center">
            <b-col>
              <h2 class="list-title" @click="showTitleEditor">
                <span v-if="!editingTitle">{{ todoListTitle }}</span>
                <b-form-input
                  ref="title"
                  v-model="todoListTitle"
                  v-if="editingTitle"
                  @blur="hideTitleEditor"
                  @keydown.enter="hideTitleEditor"
                  :state="validFormInput"
                ></b-form-input>
              </h2>
            </b-col>
            <b-col>
              <div class="contributors-wrapper">
                <Contributors
                  :todoListContributors="todoList.contributors"
                  :accountContributors="contributors"
                ></Contributors>
                <div class="list-settings">
                  <InviteContributorsForm :todoList="todoList"></InviteContributorsForm>
                  <b-dropdown class="list-settings-dropdown" right>
                    <template v-slot:button-content>
                      <b-icon-three-dots-vertical font-scale="1"></b-icon-three-dots-vertical>
                    </template>
                    <b-dropdown-item v-if="todoList.role === 2" @click="leaveTodoList">Leave</b-dropdown-item>
                    <b-dropdown-item v-if="todoList.role === 3" @click="deleteTodoList">Delete</b-dropdown-item>
                  </b-dropdown>
                </div>
              </div>
            </b-col>
          </b-row>
        </header>
        <TodoListItems
          v-if="!loadingItems"
          :todoListId="todoList.id"
          :items="items"
          :itemsLayout="itemsLayout"
          @item-selected="selectItem"
          @checkbox-clicked="dispatchSetItemCompletedState"
          @add-item="dispatchAddItem"
          @update-item-position="dispatchUpdateItemPosition"
          @delete-item="dispatchDeleteItem"
        ></TodoListItems>
      </div>
    </b-col>
    <TodoItemDetails
      v-if="selectedItem"
      :item="selectedItem"
      @item-edited="dispatchUpdateItem"
      @sub-items-completed="dispatchSetItemCompletedState"
      @sub-items-uncompleted="dispatchSetItemCompletedState"
      @sub-item-count-changed="commitUpdateHasSubItems"
    ></TodoItemDetails>
  </b-row>
</template>

<script>
import axios from "axios";
import Vue from "vue";
import { mapState } from "vuex";

import Draggable from "vuedraggable";
import Confetti from "./Confetti";
import Contributors from "./Contributors";
import InviteContributorsForm from "./InviteContributorsForm";
import TodoListItems from "./TodoListItems";
import TodoItemDetails from "./TodoItemDetails.vue";

export default {
  name: "TodoList",
  props: ["todoListId"],
  data() {
    return {
      items: [],
      itemsLayout: [],
      loadingItems: true,
      selectedItem: null,
      editingTitle: false,
    };
  },
  computed: {
    ...mapState({
      contributors: (state) => state.contributors,
    }),
    todoList() {
      return this.$store.getters.getTodoListById(this.todoListId);
    },
    todoListTitle: {
      get() {
        return this.$store.getters.getTodoListTitleById(this.todoListId);
      },
      set(val) {
        if (this.validFormInput) {
          this.$store.dispatch("updateTodoListTitle", {
            todoListId: this.todoList.id,
            listTitle: val,
          });
        }
      },
    },
    uncompletedItems() {
      return this.items.filter((item) => item.completed === false).length;
    },
    validFormInput() {
      if (this.todoListTitle.length > 0 && this.todoListTitle.length <= 50) {
        return null;
      } else {
        return false;
      }
    },
  },
  components: {
    TodoListItems,
    Contributors,
    InviteContributorsForm,
    Confetti,
    Draggable,
    TodoItemDetails,
  },
  async created() {
    await this.dispatchGetItemsAndLayout();
  },
  methods: {
    showTitleEditor() {
      this.editingTitle = true;

      this.$nextTick(() => {
        this.$refs.title.focus();
      });
    },
    hideTitleEditor() {
      this.editingTitle = false;
    },
    setTodoListCompleted() {
      this.$store.commit("setTodoListCompletedState", {
        todoListId: this.todoListId,
        completed: true,
      });
    },
    setTodoListUncompleted() {
      this.$store.commit("setTodoListCompletedState", {
        todoListId: this.todoListId,
        completed: false,
      });
    },
    selectItem(item) {
      this.selectedItem = item;
    },
    async deleteTodoList() {
      await this.$store.dispatch("deleteTodoList", {
        todoListId: this.todoList.id,
      });

      this.$router.push("/");
    },
    async dispatchGetItemsAndLayout() {
      await this.dispatchGetItemsLayout();
      await this.dispatchGetItems();
    },
    async dispatchGetItemsLayout() {
      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListId}/layout`,
      });

      this.commitSetItemsLayout(response.data);
    },
    async dispatchGetItems() {
      this.commitSetLoadingItemsState(true);

      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListId}/todos`,
      });

      this.commitSetItems(response.data);

      this.commitSetLoadingItemsState(false);
    },
    async dispatchAddItem(item) {
      const response = await axios({
        method: "POST",
        url: `api/lists/${this.todoListId}/todos`,
        data: JSON.stringify(item),
        headers: {
          "content-type": "application/json",
        },
      });

      this.commitAddItem(response.data);
    },
    async dispatchSetItemCompletedState({ itemId, completed }) {
      this.commitSetItemCompletedState(itemId, completed);

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListId}/todos/${itemId}/completed`,
        data: JSON.stringify({ completed }),
        headers: {
          "content-type": "application/json",
        },
      });
    },
    async dispatchUpdateItem(itemJsonString) {
      let item = JSON.parse(itemJsonString);

      this.commitUpdateItem(item);

      await axios({
        method: "PUT",
        url: `api/lists/${item.listId}/todos/${item.id}`,
        data: JSON.stringify({
          name: item.name,
          notes: item.notes,
          dueDate: item.dueDate,
        }),
        headers: {
          "content-type": "application/json",
        },
      });
    },
    async dispatchDeleteItem(itemId) {
      this.commitDeleteItem(itemId);

      await axios({
        method: "DELETE",
        url: `api/lists/${this.todoListId}/todos/${itemId}`,
      });
    },
    async dispatchUpdateItemPosition(event) {
      let itemId = event.item.getAttribute("data-id");
      let position = event.newIndex;

      await axios({
        method: "PUT",
        url: `api/lists/${this.todoListId}/layout`,
        data: JSON.stringify({ itemId, position }),
        headers: {
          "content-type": "application/json",
        },
      });
    },
    async dispatchGetItem(itemId) {
      const response = await axios({
        method: "GET",
        url: `api/lists/${this.todoListId}/todos/${itemId}`,
      });

      return response.data;
    },
    async leaveTodoList() {
      await axios({
        method: "POST",
        url: `api/lists/${this.todoList.id}/removeself`,
      });
      this.$store.commit("deleteTodoList", this.todoList.id);
      this.$router.push("/");
    },
    commitSetLoadingItemsState(state) {
      this.loadingItems = state;
    },
    commitSetItems(items) {
      this.items = items;
    },
    commitAddItem(item) {
      this.items.unshift(item);
      this.commitAddItemLayoutPosition(item.id);
    },
    commitAddItemLayoutPosition(itemId) {
      this.itemsLayout.unshift(itemId);
    },
    commitSetItemCompletedState(itemId, completed) {
      this.items[
        this.items.findIndex((i) => i.id === itemId)
      ].completed = completed;

      this.triggerTodoListCompleted();
    },
    commitUpdateItem(item) {
      const itemIndex = this.items.findIndex((i) => i.id === item.id);

      Vue.set(this.items, itemIndex, item);
    },
    commitDeleteItem(itemId) {
      this.commitRemoveItemLayoutPosition(itemId);

      this.items.splice(
        this.items.findIndex((i) => i.id === itemId),
        1
      );
    },
    commitRemoveItemLayoutPosition(itemId) {
      this.itemsLayout.splice(
        this.itemsLayout.findIndex((l) => l === itemId),
        1
      );
    },
    commitUpdateHasSubItems({ itemId, hasSubItems }) {
      this.items[
        this.items.findIndex((i) => i.id == itemId)
      ].hasSubItems = hasSubItems;
    },
    commitSetItemsLayout(itemsLayout) {
      this.itemsLayout = itemsLayout;
    },
    triggerTodoListCompleted() {
      const listCompleted =
        this.items.length > 0 && this.items.every((item) => item.completed);

      if (listCompleted) {
        this.setTodoListCompleted();
      } else {
        this.setTodoListUncompleted();
      }
    },
    itemsBelongToList(todoListId) {
      return this.todoListId === todoListId;
    },
  },
  async mounted() {
    // Item created
    this.$store.state.connection.on("ItemCreated", (todoListId, item) => {
      if (this.itemsBelongToList(todoListId)) {
        this.commitAddItem(item);
      }
    });

    // Item trashed
    this.$store.state.connection.on("ItemTrashed", (todoListId, item) => {
      if (this.itemsBelongToList(todoListId)) {
        this.commitDeleteItem(item.id);
      }
    });

    // Item updated
    this.$store.state.connection.on("ItemUpdated", (item) => {
      if (this.itemsBelongToList(item.listId)) {
        this.commitUpdateItem(item);
      }
    });

    // Item completed state changed
    this.$store.state.connection.on("ItemCompleted", (item) => {
      if (this.itemsBelongToList(item.listId)) {
        this.commitSetItemCompletedState(item.id, item.completed);
      }
    });

    // Layout changed
    this.$store.state.connection.on("ListLayoutChanged", async (todoListId) => {
      if (this.itemsBelongToList(todoListId)) {
        await this.dispatchGetItemsLayout();
      }
    });

    // Sub-item created
    this.$store.state.connection.on("SubItemCreated", async (subItem) => {
      const itemIndex = this.items.findIndex(
        (i) => i.id === subItem.listItemId
      );
      const item = this.items[itemIndex];

      if (itemIndex !== -1) {
        if (!item.hasSubItems) {
          this.commitUpdateHasSubItems({
            itemId: item.id,
            hasSubItems: true,
          });
        }
      }
    });

    // Sub-item trashed
    this.$store.state.connection.on(
      "SubItemTrashed",
      async (itemId, subItem) => {
        if (this.items.findIndex((i) => i.id === itemId) !== -1) {
          const item = await this.dispatchGetItem(itemId);

          if (!item.hasSubItems) {
            this.commitUpdateHasSubItems({
              itemId: item.id,
              hasSubItems: false,
            });
          }
        }
      }
    );
  },
  watch: {
    items() {
      this.triggerTodoListCompleted();
    },
    uncompletedItems() {
      this.$store.commit("updateTodoListUncompletedItems", {
        listId: this.todoList.id,
        uncompletedItems: this.uncompletedItems,
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

.list-settings {
  display: flex;
  align-items: center;
}

.list-settings-dropdown {
  .dropdown-menu {
    border-radius: 0;
  }

  &.show .btn.dropdown-toggle {
    background: transparent;
    color: $gray;
  }

  .btn {
    font-size: 18px;
    line-height: 1;
    margin-left: 10px;
    background: transparent;
    color: $gray;
    padding: 0;
    border: none;
    border-radius: 0;

    &:focus,
    &:hover,
    &:active {
      color: $gray !important;
      border: none;
      box-shadow: none !important;
      background: transparent !important;
    }

    &::after {
      display: none;
    }
  }
}

.contributors-wrapper {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}
</style>