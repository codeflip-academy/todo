<template>
    
<b-form id="add-sub-item-form" @submit.prevent="addSubItem">
    <b-button
        ref="addItemBtn"
        class="mt-3"
        size="sm"
        @click="focusForm"
        v-if="!formActive">
        Add an item
    </b-button>

    <div class="add-sub-item-input-wrapper mt-3" v-if="formActive">
        <b-form-group label="Name">
            <b-form-input v-model="form.name" maxlength="50" minlength="1" ref="subItemName" required></b-form-input>
        </b-form-group>

        <b-button
            size="sm" 
            variant="success"
            type="submit">
            Add
        </b-button>
        <b-button
            size="sm"
            @click="blurForm">
            Cancel
        </b-button>
    </div>
</b-form>

</template>

<script>

    export default {
        name: 'AddSubItemForm',
        props: ['todoListItem'],
        data() {
            return {
                form: {
                    name: ''
                },
                formActive: false
            }
        },
        methods: {
            focusForm() {
                this.formActive = true;

                this.$nextTick(() => {
                    this.$refs.subItemName.focus();
                });
            },
            blurForm() {
                this.formActive = false;

                this.$nextTick(() => {
                    this.$refs.addItemBtn.focus();
                });
            },
            async addSubItem() {
                await this.$store.dispatch('addSubItem', {
                    listId: this.todoListItem.listId,
                    todoItemId: this.todoListItem.id,
                    name: this.form.name
                });

                this.blurForm();
                this.form.name = '';
            }
        },
    }

</script>