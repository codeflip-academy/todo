<template>
  <b-list-group-item class="sub-item bg-light" :data-id="subItem.id">
    <div class="sub-item-handle mr-2" v-if="!editingSubItem">
      <b-icon-list></b-icon-list>
    </div>

    <div class="sub-item-checkbox-wrapper" v-if="!editingSubItem">
      <b-form-checkbox v-model="subItem.completed"></b-form-checkbox>
    </div>

    <div class="sub-item-name" @click="focusForm" v-if="!editingSubItem">{{ subItem.name }}</div>

    <div class="sub-item-controls pr-3" v-if="!editingSubItem">
      <b-button size="sm" variant="danger" @click="null">Delete</b-button>
    </div>

    <b-form @submit.prevent="null" v-if="editingSubItem" class="edit-sub-item-form">
      <b-form-group>
        <b-form-input
          ref="subItemName"
          v-model="form.name"
          maxlength="50"
          minlength="1"
          class="mr-2"
          required
        ></b-form-input>
      </b-form-group>

      <div class="d-flex justify-content-end">
        <b-button size="sm" variant="success" type="submit" class="flex-grow-0 mr-1">Save</b-button>
        <b-button
          size="sm"
          variant="secondary"
          class="flex-grow-0"
          @click="editingSubItem = false;"
        >Cancel</b-button>
      </div>
    </b-form>
  </b-list-group-item>
</template>

<script>
export default {
  props: ["listId", "subItem"],
  computed: {},
  data() {
    return {
      editingSubItem: false,
      form: {
        name: this.subItem.name,
      },
    };
  },
  methods: {
    focusForm() {
      this.editingSubItem = true;

      this.$nextTick(() => {
        this.$refs.subItemName.focus();
      });
    },
  },
};
</script>

<style lang="scss">
.sub-item {
  transition: background-color 0.3s ease;

  &.list-group-item {
    display: flex;
    align-items: center;
    padding: 0;
  }

  &:hover {
    cursor: pointer;
    background-color: darken(#f8f9fa, 3) !important;
  }

  &:active {
    background-color: darken(#f8f9fa, 5) !important;
  }

  .sub-item-handle {
    &:hover {
      cursor: move;
    }

    padding: 12px 0px 12px 20px;
  }

  .sub-item-name {
    font-family: "Nunito", sans-serif;
    font-weight: bold;
    flex: 1 0 auto;
    padding: 12px 0;
  }
}

.edit-sub-item-form {
  padding: 12px;
  width: 100%;
}
</style>