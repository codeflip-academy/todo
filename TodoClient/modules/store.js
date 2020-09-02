import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';

import * as signalR from "@microsoft/signalr";

Vue.use(Vuex);

const store = new Vuex.Store({
  state: {
    connection: new signalR.HubConnectionBuilder().withUrl("/notifications").build(),
    user: {},
    plan: {},
    todoLists: [],
    contributors: [],
    loadingTodoLists: true,
    loadingPlan: true,
  },
  mutations: {
    setUserData(state, data) {
      state.user = data;
    },
    setPlanData(state, data) {
      state.plan = data;
    },
    setContributors(state, contributors) {
      state.contributors = contributors;
    },
    setTodoListsLoadingState(state, loadingState) {
      state.loadingTodoLists = loadingState;
    },
    setPlanLoadingState(state, loadingState) {
      state.loadingPlan = loadingState;
    },
    setTodoLists(state, lists) {
      state.todoLists = lists;
    },
    deleteTodoList(state, todoListId) {
      state.todoLists.splice(
        state.todoLists.findIndex((t) => t.id === todoListId),
        1
      );
    },
    updateTodoListTitle(state, { todoListId, listTitle }) {
      state.todoLists[state.todoLists.findIndex((t) => t.id === todoListId)].listTitle = listTitle;
    },
    setTodoListCompletedState(state, { todoListId, completed }) {
      state.todoLists[state.todoLists.findIndex((t) => t.id === todoListId)].completed = completed;
    },
    changeUserRoleByListId(state, { todoListId, role }) {
      state.todoLists[state.todoLists.findIndex((t) => t.id === todoListId)].role = role;
    },
    updateTodoListUncompletedItems(state, { listId, uncompletedItems }) {
      state.todoLists[state.todoLists.findIndex((t) => t.id === listId)].incompleteCount = uncompletedItems;
    }
  },
  actions: {
    async changePlan(context, { planName }) {
      try {
        await axios({
          method: 'PUT',
          url: '/api/accounts/role',
          data: JSON.stringify({ plan: planName }),
          headers: {
            'content-type': 'application/json'
          }
        });

        await context.dispatch('getPlan');
      }
      catch (error) {
        throw error;
      }
    },
    async getPlan(context) {
      context.commit('setPlanLoadingState', true);

      try {
        const response = await axios({
          method: "GET",
          url: "api/accounts/plan"
        });

        context.commit('setPlanData', response.data);
      }
      catch (error) {
        throw error;
      }

      context.commit('setPlanLoadingState', false);
    },
    async getTodoLists(context) {
      context.commit('setTodoListsLoadingState', true);

      const response = await axios({
        method: "GET",
        url: "api/lists",
      });

      context.commit('setTodoLists', response.data.todoLists);
      context.commit('setContributors', response.data.contributors);

      context.commit('setTodoListsLoadingState', false);
    },
    async addTodoList(context, { listTitle }) {
      await axios({
        method: "POST",
        url: "api/lists",
        data: JSON.stringify({ listTitle: listTitle, email: context.state.user.email }),
        headers: {
          "content-type": "application/json",
        },
      });

      context.dispatch('getTodoLists');
    },
    async deleteTodoList(context, { todoListId }) {
      context.commit('deleteTodoList', todoListId)

      await axios({
        method: "DELETE",
        url: `api/lists/${todoListId}`,
      });
    },
    async updateTodoListTitle(context, { todoListId, listTitle }) {
      context.commit('updateTodoListTitle', { todoListId, listTitle });

      await axios({
        method: 'PUT',
        url: `api/lists/${todoListId}`,
        data: JSON.stringify({ listTitle }),
        headers: {
          'content-type': 'application/json',
        }
      });
    }
  },
  getters: {
    user(state) {
      return state.user;
    },
    plan(state) {
      return state.plan;
    },
    planName(state) {
      return state.plan.name;
    },
    getTodoListById: (state) => (todoListId) => {
      return state.todoLists.find(t => t.id === todoListId);
    },
    invitedTodoLists(state) {
      return state.todoLists.filter(t => t.role === 0);
    }
  }
});

export default store;
