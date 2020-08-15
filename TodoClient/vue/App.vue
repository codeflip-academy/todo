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
  async created() {
    await this.$store.dispatch("getTodoLists");
  },
  mounted() {
    // Establish connection with SignalR
    this.$store.state.connection
      .start()
      .catch((err) => console.error(err.toString()));

    // Todo list completed state changed
    this.$store.state.connection.on(
      "ListCompletedStateChanged",
      (todoListId, completed) =>
        this.$store.commit("setTodoListCompletedState", {
          todoListId,
          completed,
        })
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