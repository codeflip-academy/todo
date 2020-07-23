<template>
    
    <b-modal :id="`modal-${todoListItem.id}`" :title="todoListItem.name" @shown="refresh">

        <!-- Name -->
        <b-form @submit.prevent="updateItem">
            <b-form-group label="Name">
                <b-form-input
                    ref="name"
                    maxlength="50"
                    v-model="form.name"
                    @focus="editingName = true"
                    required>
                </b-form-input>
            </b-form-group>

            <b-button
                type="submit"
                variant="success"
                size="sm"
                v-if="editingName"
                class="mb-2">
                Save
            </b-button>
            <b-button
                variant="secondary"
                size="sm"
                v-if="editingName"
                class="mb-2"
                @click="cancelChangesToName">
                Cancel
            </b-button>
        </b-form>

        <!-- Notes -->
        <b-form @submit.prevent="updateItem">
            <b-form-group label="Notes">
                <b-form-textarea
                    rows="3"
                    maxlength="200"
                    v-model="form.notes"
                    @focus="editingNotes = true">
                </b-form-textarea>
            </b-form-group>
            <b-button
                type="submit"
                variant="success"
                size="sm"
                v-if="editingNotes"
                class="mb-2">
                Save
            </b-button>
            <b-button
                variant="secondary"
                size="sm"
                v-if="editingNotes"
                class="mb-2"
                @click="cancelChangesToNotes">
                Cancel
            </b-button>
        </b-form>

        <!-- Due Date -->
        <b-form @submit.prevent="" v-if="plan.canAddDueDates">
            <b-form-group>
                <label for="due-date">Due Date: <span v-if="form.dueDate">{{ form.dueDate | formatDate }}</span></label>
                <b-form-datepicker
                    id="due-date"
                    v-model="form.dueDate">
                </b-form-datepicker>
            </b-form-group>
        </b-form>

        <!-- Sub Items -->
        <b-form-group label="Sub-items" class="mb-2"></b-form-group>
        <SubItems :todoListItem="todoListItem"></SubItems>
        <AddSubItemForm :todoListItem="todoListItem"></AddSubItemForm>

    </b-modal>

</template>

<script>

    import moment from 'moment';
    import SubItems from './SubItems';
    import AddSubItemForm from './AddSubItemForm';

    export default {
        name: 'EditTodoItemForm',
        props: ['todoListItem'],
        data() {
            return {
                editingName: false,
                editingNotes: false,
                form: {
                    name: this.todoListItem.name,
                    notes: this.todoListItem.notes,
                    dueDate: this.todoListItem.dueDate
                }
            }
        },
        components: {
            SubItems,
            AddSubItemForm
        },
        computed: {
            dueDate() {
                return this.form.dueDate;
            },
            plan() {
                return this.$store.getters.plan;
            }
        },
        watch: {
            dueDate: function() {
                this.updateItem();
            }
        },
        methods: {
            refresh() {
                this.form.name = this.todoListItem.name;
                this.form.notes = this.todoListItem.notes;
                this.form.dueDate = this.todoListItem.dueDate;
            },
            cancelChangesToName() {
                this.form.name = this.todoListItem.name;
                this.editingName = false;
            },
            cancelChangesToNotes() {
                this.form.notes = this.todoListItem.notes;
                this.editingNotes = false;
            },
            updateItem() {
                this.editingName = false;
                this.editingNotes = false;

                let item = {
                    id: this.todoListItem.id,
                    listId: this.todoListItem.listId,
                    name: this.form.name,
                    notes: this.form.notes,
                    dueDate: this.form.dueDate
                };

                this.$store.dispatch('updateItem', { item });
            },
        },
        filters: {
            formatDate: function(value) {
                return moment(value).format('MM/D/YYYY');
            },
        },
    }

</script>

<style lang="scss" scoped>

    

</style>