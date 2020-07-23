import Vue from 'vue';
import Vuex from 'vuex';

import * as signalR from "@microsoft/signalr";

import user from './vuex/user';
import todoLists from './vuex/todoLists';
import todoListItems from './vuex/todoListItems';
import subItems from './vuex/subItems';

Vue.use(Vuex);

const store = new Vuex.Store({
  modules: {
    user,
    todoLists,
    todoListItems,
    subItems
  },
  state: {
    connection: new signalR.HubConnectionBuilder().withUrl("/notifications").build(),
  },
});

export default store;
