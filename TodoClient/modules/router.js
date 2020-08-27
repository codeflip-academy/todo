import Vue from 'vue';
import VueRouter from 'vue-router';
import axios from 'axios';
import store from './store';

Vue.use(VueRouter);

// Views
import Home from '.././vue/views/Home';
import TodoList from '.././vue/components/TodoList';
import TodoItemDetails from '.././vue/components/TodoItemDetails';
import Login from '.././vue/views/Login';
import Settings from '../vue/views/Settings';

const router = new VueRouter({
  routes: [
    { path: '/login', component: Login, name: 'Login' },
    {
      path: '/',
      component: Home,
      name: 'Home',
      children: [
        {
          path: '/lists/:todoListId',
          component: TodoList,
          props: true,
          children: [{ path: 'items/:itemId', component: TodoItemDetails }]
        },
      ]
    },
    { path: '/settings', component: Settings },
  ],
});

async function isAuthenticated() {
  let authenticated = false;

  try {
    await axios({
      method: "GET",
      url: "api/accounts/login"
    });
    authenticated = true;
  }
  finally {
    return authenticated;
  }
}

router.beforeEach(async (to, from, next) => {
  let authenticated = await isAuthenticated();

  if (!authenticated && to.name !== 'Login') {
    next({ name: 'Login' });
  }
  else {
    next();
  }
});

export default router;