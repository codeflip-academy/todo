import Vue from 'vue';
import axios from 'axios';

const subItems = {
    state: () => ({
        subItems: {},
    }),
    mutations: {
        setSubItems(state, { todoItemId, subItems }) {
            state.subItems[todoItemId] = subItems;
        },
        addSubItem(state, { subItem }) {
            state.subItems[subItem.listItemId].unshift(subItem);
        },
        updateSubItem(state, { subItem }) {
            const index = state.subItems[subItem.listItemId].findIndex(i => i.id == subItem.id);
            Vue.set(state.subItems[subItem.listItemId], index, subItem);
        },
        trashSubItem(state, { todoItemId, subItemId }) {
            const index = state.subItems[todoItemId].findIndex(i => i.id == subItemId);
            state.subItems[todoItemId].splice(index, 1);
        },
        updateSubItemCompletedState(state, { subItem }) {
            const index = state.subItems[subItem.listItemId].findIndex(i => i.id == subItem.id);
            state.subItems[subItem.listItemId][index].completed = subItem.completed;
        }
    },
    actions: {
        async loadSubItems(context, { listId, todoItemId }) {
            try {
                const response = await axios({
                    method: 'GET',
                    url: `api/lists/${listId}/todos/${todoItemId}/subitems`
                });

                context.commit('setSubItems', { todoItemId: todoItemId, subItems: response.data });
            }
            catch (error) {
                console.log(error);
            }
        },
        async addSubItem(context, { listId, todoItemId, name }) {
            try {
                const response = await axios({
                    method: 'POST',
                    url: `api/lists/${listId}/todos/${todoItemId}/subitems`,
                    headers: { 'content-type': 'application/json' },
                    data: JSON.stringify({ name })
                });

                context.commit('addSubItem', { subItem: response.data });
            }
            catch (error) {
                console.log(error);
            }
        },
        async updateSubItem(context, { listId, todoItemId, subItemId, name }) {
            try {
                await axios({
                    method: 'PUT',
                    url: `api/lists/${listId}/todos/${todoItemId}/subitems/${subItemId}`,
                    headers: { 'content-type': 'application/json' },
                    data: JSON.stringify({ name })
                })
            }
            catch (error) {
                console.log(error);
            }
        },
        async trashSubItem(context, { listId, todoItemId, subItemId }) {
            try {
                await axios({
                    method: 'DELETE',
                    url: `api/lists/${listId}/todos/${todoItemId}/subitems/${subItemId}`,
                });

                context.commit('trashSubItem', { todoItemId, subItemId });
            }
            catch (error) {
                console.log(error);
            }
        },
        async toggleSubItemCompletedState(context, { listId, todoItemId, subItemId, completed }) {
            try {
                await axios({
                    method: 'PUT',
                    url: `api/lists/${listId}/todos/${todoItemId}/subitems/${subItemId}/completed`,
                    headers: {
                        'content-type': 'application/json'
                    },
                    data: completed
                });
            }
            catch (error) {
                console.log(error);
            }
        }
    },
    getters: {
        getSubItemsByItemId: (state) => (itemId) => {
            return state.subItems[itemId];
        },
        getSubItemCompletedState: (state) => (itemId, subItemId) => {
            return state.subItems[itemId]?.find(i => i.id === subItemId).completed;
        },
        subItemCountByItemId: (state) => (itemId) => {
            return state.subItems[itemId].length;
        },
        getSubItemsLoadingState(state) {
            return state.loadingSubItems;
        }
    }
}

export default subItems;