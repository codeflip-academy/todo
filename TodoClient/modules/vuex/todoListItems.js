import Vue from 'vue';
import axios from 'axios';

const todoLists = {
    state: () => ({
        items: {},
    }),
    mutations: {
        setItems(state, { listId, items }) {
            Vue.set(state.items, listId, items);
        },
        addItem(state, { listId, item }) {
            state.items[listId].unshift(item);
        },
        deleteItem(state, { listId, itemId }) {
            const index = state.items[listId].findIndex(i => i.itemId === itemId);
            state.items[listId].splice(index, 1);
        },
        updateItemCompletedState(state, { item }) {
            let index = state.items[item.listId].findIndex(i => i.id === item.id);
            state.items[item.listId][index].completed = item.completed;
        },
        updateItem(state, { item }) {
            let index = state.items[item.listId].findIndex(i => i.id === item.id);
            Vue.set(state.items[item.listId], index, item);
        }
    },
    actions: {
        async loadItemsByListId(context, payload) {
            const response = await axios({
                method: 'GET',
                url: `api/lists/${payload.todoListId}/todos`
            });

            context.commit('setItems', { listId: payload.todoListId, items: response.data });

            response.data.forEach(async item => {
                await context.dispatch("loadSubItems", {
                    listId: item.listId,
                    todoItemId: item.id
                });
            });
        },
        async addItem(context, item) {
            const response = await axios({
                method: 'POST',
                url: `api/lists/${item.listId}/todos`,
                data: JSON.stringify(item),
                headers: {
                    'content-type': 'application/json'
                }
            });

            const itemAdded = response.data;

            context.commit('addItem', {
                listId: itemAdded.listId,
                item: itemAdded
            });

            // Set initial state for sub items in new item
            context.commit('setSubItems', { todoItemId: itemAdded.id, subItems: [] });
        },
        toggleItemCompletedState(context, { item }) {
            axios({
                method: 'PUT',
                url: `api/lists/${item.listId}/todos/${item.id}/completed`,
                data: JSON.stringify({ completed: item.completed }),
                headers: {
                    'content-type': 'application/json'
                }
            });

            context.commit('updateItemCompletedState', { item })
        },
        async updateItem(context, { item }) {
            context.commit('updateItem', { item });

            await axios({
                method: 'PUT',
                url: `api/lists/${item.listId}/todos/${item.id}`,
                data: JSON.stringify({
                    name: item.name,
                    notes: item.notes,
                    dueDate: item.dueDate
                }),
                headers: {
                    'content-type': 'application/json'
                }
            });
        },
        async deleteItem(context, { item }) {
            await axios({
                method: 'DELETE',
                url: `api/lists/${item.listId}/todos/${item.id}`
            });

            context.commit('deleteItem', { listId: item.listId, itemId: item.id });
        },
    },
    getters: {
        getItemsByListId: (state) => (listId) => {
            return state.items[listId];
        },
        getItemName: (state) => (listId, itemId) => {
            return state.items[listId].find(i => i.id === itemId).name;
        },
        getItemCompletedState: (state) => (listId, itemId) => {
            return state.items[listId].find(i => i.id === itemId).completed;
        }
    }
}

export default todoLists;