<template>
  <div id="content">
    <b-row no-gutters>
      <b-col md="4" lg="4" xl="2">
        <div class="sidebar">
          <div class="sidebar-header mb-3">
            <a href="#" class="sidebar-brand">
              <b-icon-check-all></b-icon-check-all>Todo
            </a>
          </div>
          <div class="sidebar-lists">
            <b-button class="sidebar-list selected">
              Design
              <div class="sidebar-list-completed-state">8</div>
            </b-button>
            <b-button class="sidebar-list">
              Marketing
              <div class="sidebar-list-completed-state">6</div>
            </b-button>
            <b-button class="sidebar-list">
              Development
              <div class="sidebar-list-completed-state">37</div>
            </b-button>
          </div>
        </div>
      </b-col>
      <b-col>
        <div class="account-options">
          <a href="#" class="account-dropdown">
            <div class="profile-picture">
              <img :src="user.pictureUrl" alt />
            </div>
            <div class="profile-name">{{ user.fullName }}</div>
            <div class="dropdown-toggler">
              <b-icon-chevron-down></b-icon-chevron-down>
            </div>
          </a>
        </div>
        <b-row no-gutters>
          <b-col md="5">
            <div class="list-wrapper">
              <header class="list-header">
                <h2 class="list-title">Design</h2>
                <div class="list-controls"></div>
              </header>
            </div>
          </b-col>
          <b-col md="7"></b-col>
        </b-row>
      </b-col>
    </b-row>
    <b-button class="add-list-btn">
      <b-icon-plus></b-icon-plus>
    </b-button>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "App",
  computed: {
    user() {
      return this.$store.getters.user;
    },
    firstName() {
      const fullName = this.user.fullName.split(" ");
      return fullName[0];
    },
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
  height: 100vh;
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

  .sidebar-list {
    display: flex;
    align-items: center;
    color: $gray;
    text-align: left;
    width: 100%;
    border: none;
    border-radius: 100px;
    border-top-left-radius: 0;
    border-bottom-left-radius: 0;
    background: #fff;
    padding: 7px 10px 7px 30px;

    &:hover,
    &:active {
      color: $gray;
      background: transparentize($blue, 0.8);
    }

    &:not(:last-child) {
      margin-bottom: 15px;
    }

    &.selected {
      background: transparentize($blue, 0.8);
    }

    &.completed {
      background: transparentize($green, 0.8);

      .sidebar-list-completed-state {
        background: $green;
      }
    }

    svg {
      margin-right: 14px;
    }

    .sidebar-list-completed-state {
      display: flex;
      align-items: center;
      justify-content: center;
      height: 22px;
      width: 22px;
      border-radius: 100px;
      font-size: 10px;
      color: #fff;
      background: $blue;
      margin-left: auto;
    }
  }
}

.list-wrapper {
  height: 100vh;
  border-right: 1px solid darken($light-gray, 5%);

  .list-header {
    padding: 20px 30px;
    border-bottom: solid 4px $blue;

    .list-title {
      font-size: 18px;
      color: $gray;
      text-transform: uppercase;
      margin: 0;
    }
  }
}

.btn.add-list-btn {
  position: fixed;
  bottom: 50px;
  left: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  width: 45px;
  height: 45px;
  border: none;
  border-radius: 100px;
  font-size: 22px;
  color: #fff;
  background: $orange;
  box-shadow: 0 10px 30px transparentize($orange, 0.5);

  &:hover {
    background: darken($orange, 5%);
    box-shadow: 0 10px 20px transparentize($orange, 0.5);
  }

  &:active,
  &:focus {
    box-shadow: none !important;
    border: none !important;
    background: darken($orange, 5%) !important;
  }
}
</style>