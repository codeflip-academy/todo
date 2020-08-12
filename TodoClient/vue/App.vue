<template>
  <div id="content" class="mt-4">
    <Header></Header>
    <RouterView></RouterView>
  </div>
</template>

<script>
import Header from "./components/Header";

export default {
  name: "App",
  components: {
    Header,
  },
  async beforeCreate() {
    try {
      await this.$store.dispatch("loadTodoLists");
    } catch (error) {}
  },
  mounted() {
    this.$store.state.connection
      .start()
      .catch((err) => console.error(err.toString()));
    this.$store.state.connection.on("InvitationSent", (list) =>
      this.$store.commit("addTodoList", { list })
    );
    this.$store.state.connection.on("InvitationAccepted", (list) =>
      this.$store.dispatch("refreshContributors", { list })
    );
    this.$store.state.connection.on("ContributorLeft", (list) =>
      this.$store.dispatch("refreshContributors", { list })
    );
    this.$store.state.connection.on("ListNameUpdated", (listId, listTitle) =>
      this.$store.commit("updateListTitle", { listId, listTitle })
    );
    this.$store.state.connection.on(
      "ListCompletedStateChanged",
      (listId, listCompletedState) =>
        this.$store.commit("setTodoListCompletedState", {
          listId,
          listCompletedState,
        })
    );
    this.$store.state.connection.on("ItemCreated", (listId, item) => {
      this.$store.commit("addItem", { listId, item });
      this.$store.commit("setSubItems", { todoItemId: item.id, subItems: [] });
    });
    this.$store.state.connection.on("ItemCompleted", (item) =>
      this.$store.commit("updateItemCompletedState", { item })
    );
    this.$store.state.connection.on("ItemUpdated", (item) =>
      this.$store.commit("updateItem", { item })
    );
    this.$store.state.connection.on("SubItemCreated", (subItem) =>
      this.$store.commit("addSubItem", { subItem })
    );
    this.$store.state.connection.on("SubItemCompletedStateChanged", (subItem) =>
      this.$store.commit("updateSubItemCompletedState", {
        todoItemId: subItem.listItemId,
        subItemId: subItem.id,
        completed: subItem.completed,
      })
    );
    this.$store.state.connection.on("SubItemUpdated", (subItem) =>
      this.$store.commit("updateSubItem", { subItem })
    );
  },
};
</script>

<style lang="scss">
* {
  box-sizing: border-box;
}

#content {
  padding: 75px 20px;
  height: 100vh;

  h1,
  h2,
  h3,
  h4 {
    font-family: "Nunito", sans-serif;
    font-weight: bold;
  }
}

h1 {
  font-size: 40px;
}

.modal-footer {
  display: none !important;
}

#confetti-canvas {
  z-index: 100;
}
</style>