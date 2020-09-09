<template>
  <div class="sub-item" :class="{ 'completed': subItemCompletedState }" :data-id="subItem.id">
    <label
      :for="subItem.id + '-checkbox'"
      v-if="!editingSubItem"
      class="sub-item-checkbox"
      :class="{ 'selected': subItemCompletedState }"
    >
      <b-icon-check></b-icon-check>
      <b-form-checkbox
        :id="subItem.id + '-checkbox'"
        v-model="subItemCompletedState"
        class="sr-only"
      ></b-form-checkbox>
    </label>

    <div class="sub-item-name" @click="focusForm" v-if="!editingSubItem">{{ subItem.name }}</div>

    <div class="sub-item-controls ml-auto pr-3" v-if="!editingSubItem">
      <b-button size="sm" variant="danger" @click="sendDeleteSubItemEvent">Delete</b-button>
    </div>

    <b-form
      @submit.prevent="sendUpdateSubItemNameEvent"
      v-if="editingSubItem"
      class="edit-sub-item-form"
    >
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
  </div>
</template>

<script>
export default {
  props: ["listId", "subItem"],
  data() {
    return {
      editingSubItem: false,
      form: {
        name: this.subItem.name,
      },
    };
  },
  computed: {
    subItemCompletedState: {
      get() {
        return this.subItem.completed;
      },
      set(val) {
        this.sendCheckboxClickedEvent(val);
      },
    },
  },
  methods: {
    focusForm() {
      this.editingSubItem = true;

      this.$nextTick(() => {
        this.$refs.subItemName.focus();
      });
    },
    sendDeleteSubItemEvent() {
      this.$emit("delete-sub-item", this.subItem.id);
    },
    sendCheckboxClickedEvent(completed) {
      this.$emit("checkbox-clicked", {
        subItemId: this.subItem.id,
        completed: completed,
      });
    },
    sendUpdateSubItemNameEvent() {
      this.$emit("update-sub-item-name", {
        subItemId: this.subItem.id,
        subItemName: this.form.name,
      });

      this.editingSubItem = false;
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

.sub-item {
  display: flex;
  align-items: center;
  color: lighten($gray, 20);

  &.completed {
    .sub-item-name {
      text-decoration: line-through;
    }
  }

  &:not(:last-child) {
    margin-bottom: 5px;
  }

  .sub-item-checkbox {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 16px;
    height: 16px;
    border-radius: 16px;
    background-color: transparent;
    border: solid 1px lighten($gray, 20);
    margin-right: 10px;
    margin-bottom: 0;
    transition: background-color 0.3s ease;

    svg {
      opacity: 0;

      &:hover {
        opacity: 1;
      }
    }

    &.selected {
      background-color: lighten($gray, 20);

      svg {
        opacity: 1;
        color: white;
      }
    }
  }
}
</style>