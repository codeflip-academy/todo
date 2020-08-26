<template>
  <b-form id="add-sub-item-form" @submit.prevent="addSubItem">
    <b-button
      ref="addItemBtn"
      class="mb-3 p-0 add-sub-item-btn"
      size="sm"
      variant="link"
      @click="focusForm"
      v-if="!formActive"
    >
      <b-icon-plus class="add-item-btn-icon"></b-icon-plus>Add item
    </b-button>

    <div class="add-sub-item-input-wrapper mt-3" v-if="formActive">
      <b-form-group label="Name">
        <b-form-input v-model="form.name" maxlength="50" minlength="1" ref="subItemName" required></b-form-input>
      </b-form-group>

      <b-button size="sm" variant="success" type="submit">Add</b-button>
      <b-button size="sm" @click="blurForm">Cancel</b-button>
    </div>
  </b-form>
</template>

<script>
export default {
  name: "AddSubItemForm",
  props: ["todoListItem"],
  data() {
    return {
      form: {
        name: "",
      },
      formActive: false,
    };
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
      const subItemName = this.form.name;

      this.$emit("add-sub-item", subItemName);

      this.blurForm();
      this.form.name = "";
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

.add-sub-item-btn {
  .add-item-btn-icon {
    color: $gray;
    margin-right: 5px;
  }

  &:hover {
    text-decoration: none;
  }
}
</style>