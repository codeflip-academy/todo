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
            <RouterView :key="$route.fullPath"></RouterView>
          </b-col>
          <b-col md="7">
            <div class="item-content">
              <h2 class="item-name">Lorem ipsum dolor sit.</h2>
              <div class="item-meta">
                <div class="item-due-date">
                  <b-icon-calendar></b-icon-calendar>Today
                </div>
              </div>
              <div
                class="item-notes"
              >Maecenas faucibus mollis interdum. Etiam porta sem malesuada magna mollis euismod. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec id elit non mi porta gravida at eget metus.</div>
              <div class="sub-items">
                <div class="sub-item">Item 1</div>
                <div class="sub-item">Item 2</div>
                <div class="sub-item">Item 3</div>
                <div class="sub-item">Item 4</div>
              </div>
            </div>
          </b-col>
        </b-row>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import axios from "axios";

import Draggable from "vuedraggable";
import TodoLists from "../components/TodoLists";
import TodoList from "../components/TodoList";

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
  },
  components: {
    TodoLists,
    TodoList,
    Draggable,
  },
};
</script>