<template>
  <b-dropdown id="invitations-dropdown" right>
    <template v-slot:button-content>
      <div class="notification-icon">
        <b-icon-bell></b-icon-bell>
        <div class="notification-icon-badge" v-if="hasNotifications"></div>
      </div>
      <span class="sr-only">Invitations</span>
    </template>
    <div class="invitation" v-for="list in todoLists" :key="list.id">
      <h3 class="invitation-title">{{ list.listTitle }}</h3>

      <Contributors
        class="mb-2"
        :todoListContributors="list.contributors"
        :accountContributors="contributors"
      ></Contributors>

      <div class="list-controls">
        <b-button variant="success" size="sm" @click="acceptInvitation(list.id)">Accept</b-button>
        <b-button variant="danger" size="sm" @click="declineInvitation(list.id)">Decline</b-button>
      </div>
    </div>

    <div class="invitation text-center" v-if="!hasNotifications">Nothing to see here 😉</div>
  </b-dropdown>
</template>

<script>
import axios from "axios";
import { mapState } from "vuex";

import Contributors from "./Contributors";

export default {
  name: "Invitations",
  components: {
    Contributors,
  },
  computed: {
    todoLists() {
      return this.$store.getters.invitedTodoLists;
    },
    ...mapState({
      contributors: (state) => state.contributors,
    }),
    hasNotifications() {
      return this.todoLists.length > 0;
    },
  },
  methods: {
    async acceptInvitation(listId) {
      await axios({
        method: "POST",
        url: `api/lists/${listId}/accept`,
      });

      this.$store.commit("changeUserRoleByListId", {
        todoListId: listId,
        role: 2,
      });
    },
    async declineInvitation(listId) {
      await axios({
        method: "POST",
        url: `api/lists/${listId}/decline`,
      });

      this.$store.commit("deleteTodoList", listId);
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

#invitations-dropdown {
  background: transparent;

  .dropdown-toggle {
    height: 57px;
    padding: 12px 20px;
    color: $gray;
    border: none !important;
    border-left: 1px solid darken($light-gray, 5%) !important;
    background: transparent;

    &:hover {
      background-color: darken($light-gray, 2%);
      box-shadow: none !important;
    }

    &:active,
    &:focus {
      background-color: darken($light-gray, 4%);
      box-shadow: none !important;
    }

    &::after {
      display: none;
    }
  }

  .dropdown-menu {
    min-width: 250px;
    padding: 0px !important;
    border-radius: 0px;
  }

  .btn {
    border-radius: 0;
  }

  .notification-icon {
    position: relative;

    .notification-icon-badge {
      position: absolute;
      top: 4px;
      right: 1px;
      display: block;
      width: 6px;
      height: 6px;
      background: #b71c1c;
      border-radius: 100px;
    }
  }
}

.invitation {
  padding: 18px;

  .invitation-title {
    font-size: 20px;
    font-weight: bold;
    margin-bottom: 8px;
  }

  &:not(:last-child) {
    border-bottom: 1px solid darken($light-gray, 5%);
  }
}
</style>