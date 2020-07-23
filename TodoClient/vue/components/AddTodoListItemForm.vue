<template>

    <div id="add-todo-list-item">
        <b-button @click="$bvModal.show('modal-add-todo-list-item')">Add item</b-button>

        <b-modal id="modal-add-todo-list-item" title="Add item" @shown="focusOnForm">
            <b-form @submit.prevent="addItem">
                
                <!-- Name -->
                <b-form-group label="Name">
                    <b-form-input ref="title" v-model="form.name" maxlength="50" required></b-form-input>
                </b-form-group>

                <!-- Notes -->
                <b-form-group label="Notes">
                    <b-form-textarea rows="3" v-model="form.notes" maxlength="200"></b-form-textarea>
                </b-form-group>

                <!-- Due Date -->
                <b-form-group label="Due Date" v-if="plan.canAddDueDates">
                   <b-form-datepicker v-model="form.dueDate"></b-form-datepicker>
                </b-form-group>

                <b-button type="submit" variant="success" class="ml-auto d-block">Add</b-button>
            </b-form>
        </b-modal>
    </div>
    
</template>

<script>

    export default {
        name: 'AddTodoListItemForm',
        props: ['todoListId'],
        data() {
            return {
                form: {
                    listId: this.todoListId,
                    name: null,
                    notes: null,
                    dueDate: null,
                }
            }
        },
        computed: {
            plan() {
                return this.$store.getters.plan;
            }
        },
        methods: {
            focusOnForm() {
                this.$refs.title.focus();
            },
            addItem() {
                this.$store.dispatch('addItem', this.form).then(() => {
                    this.form.name = null;
                    this.form.notes = null;
                    this.form.dueDate = null;
                    this.$bvModal.hide('modal-add-todo-list-item');
                });
            }
        },
    }

</script>