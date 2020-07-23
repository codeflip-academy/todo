<template>

  <div class="todo-lists-wrapper">

    <todo-list-preview
        v-for="todoList in todoLists"
        :key="todoList.id"
        :todoList="todoList"
        :contributors="contributors">
    </todo-list-preview>
        
  </div>
      
</template>

<script>

    import TodoListPreview from './TodoListPreview';

    export default {
        data() {
            return {};
        },
        computed: {
            todoLists() {
                return this.$store.getters.todoLists;
            },
            contributors() {
                return this.$store.getters.contributors;
            },
        },
        components: {
            TodoListPreview
        },
    };

</script>

<style lang="scss" scoped>

</style>