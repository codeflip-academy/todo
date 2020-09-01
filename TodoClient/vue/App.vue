<template>
  <div id="content">
    <b-row no-gutters>
      <b-col md="4" lg="4" xl="2">
        <Sidebar></Sidebar>
      </b-col>
      <b-col
        class="todo-list-bg"
        :style="{ 'background-image': 'url(' + todoListImage + ')' }"
        :class="{ 'list-selected': listSelected }"
      >
        <Header></Header>
        <RouterView></RouterView>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import axios from "axios";
import Header from "./components/Header";
import Sidebar from "./components/Sidebar";
import todoListImage from "../images/todo-list-bg.jpg";

export default {
  name: "App",
  data() {
    return {
      todoListImage: todoListImage,
    };
  },
  components: {
    Header,
    Sidebar,
  },
  async created() {
    await this.checkAuthState();
    await this.getPlan();
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

    // Todo list name changed
    this.$store.state.connection.on(
      "ListNameUpdated",
      (todoListId, listTitle) =>
        this.$store.commit("updateTodoListTitle", {
          todoListId,
          listTitle,
        })
    );

    // Invitation sent
    this.$store.state.connection.on(
      "InvitationSent",
      async () => await this.$store.dispatch("getTodoLists")
    );

    // Invitation accepted
    this.$store.state.connection.on(
      "InvitationAccepted",
      async () => await this.$store.dispatch("getTodoLists")
    );

    // Contributor left
    this.$store.state.connection.on(
      "ContributorLeft",
      async () => await this.$store.dispatch("getTodoLists")
    );
  },
  computed: {
    listSelected() {
      return this.$route.name !== "Home";
    },
  },
  methods: {
    async checkAuthState() {
      try {
        const user = await axios({
          method: "GET",
          url: "api/accounts",
        });
        this.$store.commit("setUserData", user.data);
      } catch (error) {
        console.error(error);
      }
    },
    async getPlan() {
      await this.$store.dispatch("getPlan");
    },
    logout() {
      this.user = {};
      axios({
        method: "GET",
        url: "api/accounts/logout",
      }).then(() => {
        this.$router.push("/login");
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

* {
  box-sizing: border-box;
}

body {
  font-family: "Poppins", sans-serif !important;
  font-weight: 300;
  font-size: 16px;
}

.account-options {
  background: $light-gray;
  border-bottom: 1px solid darken($light-gray, 5%);
  display: flex;
  align-items: center;
  justify-content: flex-end;

  .account-dropdown {
    display: flex;
    align-items: center;
    padding: 12px 25px;
    text-decoration: none;
    background-color: $light-gray;
    border-left: 1px solid darken($light-gray, 5%);
    transition: background-color 0.3s ease;

    &:hover {
      background-color: darken($light-gray, 2%);
    }

    &:active,
    &:focus {
      background-color: darken($light-gray, 4%);
    }

    .profile-picture {
      margin-right: 12px;

      img {
        width: 100%;
        max-width: 32px;
        height: auto;
        border-radius: 4px;
      }
    }

    .profile-name {
      color: $gray;
      margin-right: 12px;
    }

    .dropdown-toggler {
      color: lighten($gray, 20%);
      font-size: 14px;
    }
  }
}

.sidebar {
  min-height: 100vh;
  border-right: 1px solid darken($light-gray, 5%);

  .sidebar-header {
    font-size: 22px;

    .sidebar-brand {
      display: block;
      padding: 20px 30px;
      font-weight: 300;
      text-transform: uppercase;
      color: $orange;
      transition: color 0.3s ease;

      svg {
        margin-right: 7px;
      }

      &:hover {
        text-decoration: none;
        color: darken($orange, 10%);
      }
    }
  }

  .sidebar-lists {
    padding-right: 15px;
  }
}

.list-wrapper {
  height: 100vh;
  border-right: 1px solid darken($light-gray, 5%);

  .list-header {
    padding: 20px 30px;
    border-bottom: solid 4px $blue;
    background: #fff;

    .list-title {
      font-size: 18px;
      color: $gray;
      text-transform: uppercase;
      margin: 0;
    }
  }
}

.modal {
  .modal-title {
    font-size: 18px;
    font-weight: bold;
    text-transform: uppercase;
    color: $gray;
  }

  .modal-content {
    border-radius: 0 !important;
    border: none !important;
  }

  .modal-footer {
    display: none !important;
  }

  .form-group {
    color: $gray;

    .form-control {
      border-radius: 0;
    }
  }

  .btn {
    border-radius: 0;
  }
}
</style>