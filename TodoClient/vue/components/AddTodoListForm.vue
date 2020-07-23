<template>

  <div id="add-todo-list-controls">
    <b-button @click="$bvModal.show('modal-add-todo-list')">Add list</b-button>

    <b-modal id="modal-add-todo-list" title="Add list" @shown="focusOnForm">
        <b-form @submit.prevent="addTodoList">
            <b-form-group label="List Title">
                <b-form-input ref="listTitle" v-model="form.listTitle"></b-form-input>
            </b-form-group>

            <b-button type="submit" variant="success" class="ml-auto d-block">Add</b-button>
        </b-form>
    </b-modal>

  </div>
      
</template>

<script>

    export default {
        name: 'AddTodoListForm',
        data() {
            return {
                form: {
                    listTitle: ''
                }
            }
        },
        computed: {
            user() {
                return this.$store.getters.user;
            }
        },
        methods: {
            focusOnForm() {
                this.$refs.listTitle.focus()
            },
            addTodoList() {
                this.$store.dispatch('addTodoList', {
                    listTitle: this.form.listTitle,
                    email: this.user.email,
                })
                .then(() => {
                    this.form.listTitle = '';
                });
                
                this.$bvModal.hide('modal-add-todo-list');
            }
        },
    };

</script>