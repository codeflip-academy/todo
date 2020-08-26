<template>
  <div id="add-todo-list-controls">
    <b-button class="add-list-btn" @click="$bvModal.show('modal-add-todo-list')">
      <b-icon-plus></b-icon-plus>
    </b-button>

    <b-modal id="modal-add-todo-list" title="Add list" @shown="focusOnForm">
      <b-form @submit.prevent="addTodoListRequest">
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
  name: "AddTodoListForm",
  data() {
    return {
      form: {
        listTitle: "",
      },
    };
  },
  methods: {
    focusOnForm() {
      this.$refs.listTitle.focus();
    },
    addTodoListRequest() {
      this.$emit("add-todo-list", this.form.listTitle);

      this.form.listTitle = "";
      this.$bvModal.hide("modal-add-todo-list");
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

.btn.add-list-btn {
  position: fixed;
  bottom: 50px;
  left: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  width: 45px;
  height: 45px;
  border: none;
  border-radius: 100px;
  font-size: 30px;
  color: #fff;
  background: $orange;
  box-shadow: 0 10px 30px transparentize($orange, 0.5);

  &:hover {
    background: darken($orange, 5%);
    box-shadow: 0 10px 20px transparentize($orange, 0.5);
  }

  &:active,
  &:focus {
    box-shadow: 0 10px 10px transparentize($orange, 0.8) !important;
    border: none !important;
    background: darken($orange, 5%) !important;
  }
}
</style>