<template>

  <b-container>
    <b-button class="back-button mb-2" variant="link" to="/">
      <b-icon icon="chevron-left"></b-icon> My Lists
    </b-button>

    <TodoList :todoListId="todoListId" v-if="!loading"></TodoList>
  </b-container>

</template>

<script>

  import TodoList from '../components/TodoList';

  export default {
    name: 'TodoListView',
    props: ['todoListId'],
    components: {
      TodoList
    },
    computed: {
      loading() {
        return this.$store.getters.getLoadingState;
      }
    },
  };

</script>

<style lang="scss" scoped>

.back-button {
  display: flex;
  align-items: center;
  padding: 0px;
  position: relative;
  left: -7px;
  margin-bottom: 10px;
}

</style>