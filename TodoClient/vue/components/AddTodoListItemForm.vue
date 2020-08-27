<template>
  <div id="add-todo-list-item">
    <div class="item item-add" @click="$bvModal.show('modal-add-todo-list-item')">
      <b-button class="btn-add">
        <b-icon-plus></b-icon-plus>
      </b-button>Add item
    </div>

    <b-modal id="modal-add-todo-list-item" title="Add item" @shown="focusOnForm">
      <b-form @submit.prevent="addItemRequest">
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
  name: "AddTodoListItemForm",
  props: ["todoListId"],
  data() {
    return {
      form: {
        listId: this.todoListId,
        name: null,
        notes: null,
        dueDate: null,
      },
    };
  },
  computed: {
    plan() {
      return this.$store.getters.plan;
    },
  },
  methods: {
    focusOnForm() {
      this.$refs.title.focus();
    },
    async addItemRequest() {
      let item = { ...this.form };

      this.$emit("add-item", item);

      this.form.name = null;
      this.form.notes = null;
      this.form.dueDate = null;
      this.$bvModal.hide("modal-add-todo-list-item");
    },
  },
};
</script>

<style lang="scss" scoped>
$light-gray: #f5f6f7;
$gray: #455a64;
$blue: #1e88e5;
$orange: #ff7043;
$green: #4caf50;
$red: #b71c1c;

.btn-add.btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 100px;
  background-color: $green;
  border: none;
  margin-bottom: 0;
  margin-right: 16px;
  font-size: 36px;
  color: #fff;
  box-shadow: 0 10px 30px transparentize($green, 0.7);
  transition: all 0.2s ease;

  &:hover {
    background-color: darken($green, 5%);
    box-shadow: 0 10px 30px transparentize($green, 0.7);
  }

  &:active,
  &focus {
    box-shadow: 0 10px 30px transparentize($green, 0.7) !important;
    background-color: darken($green, 10%) !important;
  }
}
</style>