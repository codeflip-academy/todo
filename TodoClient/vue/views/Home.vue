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
      <b-col>
        <div class="account-options">
          <Invitations></Invitations>
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
        <RouterView :key="$route.fullPath" v-if="!loadingTodoLists"></RouterView>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import Draggable from "vuedraggable";
import TodoLists from "../components/TodoLists";
import TodoList from "../components/TodoList";
import Invitations from "../components/Invitations";

export default {
  name: "Home",
  computed: {
    user() {
      return this.$store.getters.user;
    },
    firstName() {
      const fullName = this.user.fullName.split(" ");
      return fullName[0];
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
};
</script>