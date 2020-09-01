<template>
  <div id="content">
    <b-row no-gutters>
      <b-col md="4" lg="4" xl="2">
        <div class="sidebar">
          <div class="sidebar-header mb-3">
            <b-link :to="{ name: 'Home' }" class="sidebar-brand">
              <b-icon-check-all></b-icon-check-all>Todo
            </b-link>
          </div>
          <div class="sidebar-lists">
            <TodoLists></TodoLists>
          </div>
          <div class="item-categories">
            <!-- <b-link to="/all" class="item-category">
              <div class="item-category-icon">
                <b-icon-inbox></b-icon-inbox>
              </div>
              <div class="item-category-name">All</div>
            </b-link>
            <b-link to="/today" class="item-category">
              <div class="item-category-icon">
                <b-icon-calendar3></b-icon-calendar3>
              </div>
              <div class="item-category-name">Today</div>
            </b-link>
            <b-link to="/important" class="item-category">
              <div class="item-category-icon">
                <b-icon-star></b-icon-star>
              </div>
              <div class="item-category-name">Important</div>
            </b-link>
            <b-link to="/scheduled" class="item-category">
              <div class="item-category-icon">
                <b-icon-alarm></b-icon-alarm>
              </div>
              <div class="item-category-name">Scheduled</div>
            </b-link>-->
          </div>
        </div>
      </b-col>
      <b-col
        class="todo-list-bg"
        :style="{ 'background-image': 'url(' + todoListImage + ')' }"
        :class="{ 'list-selected': listSelected }"
      >
        <div class="account-options">
          <Invitations></Invitations>
          <b-dropdown class="account-dropdown" right>
            <template v-slot:button-content>
              <div class="profile-picture">
                <img :src="user.pictureUrl" alt />
              </div>
              <div class="profile-name">{{ user.fullName }}</div>
              <div class="dropdown-toggler">
                <b-icon-chevron-down></b-icon-chevron-down>
              </div>
            </template>
            <b-dropdown-item to="/settings">
              <b-icon-gear class="mr-2"></b-icon-gear>Settings
            </b-dropdown-item>
            <b-dropdown-item>
              <div @click="signOut">
                <b-icon-box-arrow-left class="mr-2"></b-icon-box-arrow-left>Sign out
              </div>
            </b-dropdown-item>
          </b-dropdown>
        </div>
        <RouterView :key="$route.fullPath" v-if="!loadingTodoLists"></RouterView>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import todoListImage from "../../images/todo-list-bg.jpg";
import Draggable from "vuedraggable";
import TodoLists from "../components/TodoLists";
import TodoList from "../components/TodoList";
import Invitations from "../components/Invitations";

export default {
  name: "Home",
  data() {
    return {
      todoListImage: todoListImage,
    };
  },
  computed: {
    user() {
      return this.$store.getters.user;
    },
    ...mapState({
      loadingTodoLists: (state) => state.loadingTodoLists,
    }),
    listSelected() {
      return this.$route.name !== "Home";
    },
  },
  components: {
    TodoLists,
    TodoList,
    Draggable,
    Invitations,
  },
  methods: {
    async signOut() {
      await axios({
        method: "GET",
        url: "api/accounts/logout",
      });

      this.$router.push("/login");
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

.item-categories {
  margin-left: 30px;
  margin-top: 40px;
}

.item-category {
  display: flex;
  align-items: center;
  color: $gray;

  .item-category-icon {
    margin-right: 15px;
  }

  &:not(:last-child) {
    margin-bottom: 20px;
  }
}

.todo-list-bg {
  background-position: center;
  background-size: 70%;
  background-repeat: no-repeat;

  &.list-selected {
    background-image: none !important;
  }
}

.account-dropdown {
  .btn {
    display: flex;
    align-items: center;
    padding: 0px;
    background: transparent;
    border: none;

    &::after {
      display: none;
    }

    &:hover,
    &:focus,
    &:active {
      background: transparent !important;
      box-shadow: none;
    }
  }

  .dropdown-menu {
    border-radius: 0;
    margin-top: 12px;
  }
}

.show > .btn-secondary.dropdown-toggle {
  background-color: transparent !important;
}

.secondary:not(:disabled):not(.disabled):active:focus,
.show > .btn-secondary.dropdown-toggle:focus {
  box-shadow: none !important;
}
</style>