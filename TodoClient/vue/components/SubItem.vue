<template>
  <b-list-group-item class="sub-item bg-light" :data-id="subItem.id">
    <div class="sub-item-handle mr-2" v-if="!editingSubItem">
      <b-icon-list></b-icon-list>
    </div>

    <div class="sub-item-checkbox-wrapper" v-if="!editingSubItem">
      <b-form-checkbox v-model="completedState"></b-form-checkbox>
    </div>

    <div class="sub-item-name" @click="focusForm" v-if="!editingSubItem">{{ subItem.name }}</div>

    <div class="sub-item-controls pr-3" v-if="!editingSubItem">
      <b-button size="sm" variant="danger" @click="deleteSubItem">Delete</b-button>
    </div>

    <b-form @submit.prevent="updateSubItem" v-if="editingSubItem" class="edit-sub-item-form">
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

      <b-button size="sm" class="mr-1" variant="success" type="submit">Save</b-button>
      <b-button size="sm" variant="secondary" @click="editingSubItem = false;">Cancel</b-button>
    </b-form>
  </b-list-group-item>
</template>

<script>
export default {
  props: ["listId", "subItem"],
  computed: {
    completedState: {
      get() {
        return this.$store.getters.getSubItemCompletedState(
          this.subItem.listItemId,
          this.subItem.id
        );
      },
      set(value) {
        this.$store.dispatch("toggleSubItemCompletedState", {
          listId: this.listId,
          todoItemId: this.subItem.listItemId,
          subItemId: this.subItem.id,
          completed: value
        });
      }
    }
  },
  data() {
    return {
      editingSubItem: false,
      itemCompletedState: false,
      form: {
        name: this.subItem.name
      }
    };
  },
  methods: {
    focusForm() {
      this.editingSubItem = true;

      this.$nextTick(() => {
        this.$refs.subItemName.focus();
      });
    },
    async updateSubItem() {
      await this.$store.dispatch("updateSubItem", {
        listId: this.listId,
        todoItemId: this.subItem.listItemId,
        subItemId: this.subItem.id,
        name: this.form.name
      });

      this.editingSubItem = false;
    },
    async deleteSubItem() {
      this.$store.commit("trashSubItem", { subItem: this.subItem });

      await this.$store.dispatch("trashSubItem", {
        listId: this.listId,
        todoItemId: this.subItem.listItemId,
        subItemId: this.subItem.id
      });

      await this.$store.dispatch("loadItemsByListId", {
        todoListId: this.listId
      });
    }
  }
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