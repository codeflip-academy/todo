import Vue from 'vue';
import VueRouter from 'vue-router';
import axios from 'axios';

Vue.use(VueRouter);

// Views
import Home from '.././vue/views/Home';
import TodoListView from '.././vue/views/TodoListView';
import Login from '.././vue/views/Login';
import Settings from '../vue/views/Settings.vue';

const router = new VueRouter({
  routes: [
    { path: '/login', component: Login, name: 'Login' },
    { path: '/', component: Home, name: 'Home' },
    { path: '/lists', component: Home, name: 'My Lists' },
    { path: '/lists/:todoListId', component: TodoListView, props: true },
    { path: '/settings', component: Settings }
  ],
});

export default router;