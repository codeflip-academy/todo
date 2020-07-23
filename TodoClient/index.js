import Vue from 'vue';
import store from './modules/store';
import Router from './modules/router';
import App from './vue/App.vue';
import './modules/bootstrap';
import 'animate.css/animate.css';

new Vue({
  el: '#app',
  store,
  router: Router,
  render: h => h(App)
});
