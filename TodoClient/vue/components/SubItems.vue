<template>
  <b-list-group>
    <Draggable handle=".sub-item-handle" v-model="layout" @end="updateSubItemPosition">
      <SubItem
        v-for="itemId in layout"
        :key="itemId"
        :subItem="items.find(x => x.id === itemId)"
        :listId="todoListItem.listId"
      ></SubItem>
      <b-list-group-item v-if="layout.length < 1">There are no sub-items.</b-list-group-item>
    </Draggable>
  </b-list-group>
</template>

<script>
import axios from "axios";
import SubItem from "./SubItem";
import Draggable from "vuedraggable";

export default {
  props: ["todoListItem"],
  components: {
    Draggable,
    SubItem,
  },
  async created() {
    this.items = this.setSubItems();
    await this.getLayout();
  },
  mounted() {
    this.$store.state.connection.on("ItemLayoutUpdated", (itemId) =>
      this.refreshSubItemLayout(itemId)
    );
    this.$store.state.connection.on(
      "SubItemTrashed",
      async (todoItemId, subItem) => {
        this.$store.commit("trashSubItem", {
          todoItemId,
          subItemId: subItem.id,
        });
        this.refreshSubItemLayout(itemId);
      }
    );
  },
  data() {
    return {
      items: [],
      layout: [],
    };
  },
  methods: {
    async getLayout() {
      try {
        const response = await axios({
          method: "GET",
          url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/layout`,
        });

        this.layout = response.data.layout;
      } catch (error) {
        console.log(error);
      }
    },
    async updateSubItemPosition(event) {
      const subItemId = event.item.getAttribute("data-id");
      const position = event.newIndex;

      try {
        await axios({
          method: "PUT",
          url: `api/lists/${this.todoListItem.listId}/todos/${this.todoListItem.id}/layout`,
          headers: {
            "content-type": "application/json",
          },
          data: JSON.stringify({ subItemId, position }),
        });
      } catch (error) {
        console.log(error);
      }
    },
    async refreshSubItemLayout(todoItemId) {
      if (todoItemId === this.todoListItem.id) {
        await this.getLayout();
      }
    },
    setSubItems() {
      return this.$store.getters.getSubItemsByItemId(this.todoListItem.id);
    },
  },
  watch: {
    items: async function () {
      await this.getLayout();
    },
  },
};
</script>