<template>
  <div class="list-wrapper">
    <header class="list-header">
      <b-row class="align-items-center">
        <b-col>
          <h2 class="list-title">All</h2>
        </b-col>
      </b-row>
    </header>
    <div class="items" v-if="!loadingItems">
      <TodoListItem
        v-for="item in items"
        :item="item"
        :key="item.id"
        @item-selected="navigateToList"
      ></TodoListItem>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import TodoListItem from "./TodoListItem";

export default {
  name: "AllItems",
  components: {
    TodoListItem,
  },
  data() {
    return {
      items: [],
      loadingItems: false,
    };
  },
  async created() {
    this.loadingItems = true;

    const response = await axios.get("api/lists/allItems");

    this.loadingItems = false;

    this.items = response.data;
  },
  methods: {
    navigateToList(item) {
      this.$router.push({ path: `/lists/${item.listId}` });
    },
  },
};
</script>