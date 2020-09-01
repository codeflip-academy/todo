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
            <TodoLists></TodoLists>
          </div>
        </div>
      </b-col>
      <b-col
        class="todo-list-bg"
        :style="{ 'background-image': 'url(' + todoListImage + ')' }"
        :class="{ 'list-selected': $route.params.todoListId }"
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
            <b-dropdown-item>
              <div @click="signOut">Sign out</div>
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