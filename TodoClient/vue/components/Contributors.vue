<template>
  <ul class="contributors">
    <li v-for="contributor in todoListContributors" :key="contributor" class="contributor">
      <img
        :src="accountContributors[contributor].pictureUrl"
        :alt="accountContributors[contributor].fullName"
        v-b-tooltip.hover
        :title="accountContributors[contributor].fullName"
      />
    </li>
  </ul>
</template>

<script>
export default {
  name: "Contributors",
  props: ["todoListContributors", "accountContributors"],
};
</script>

<style lang="scss" scoped>
.b-tooltip {
  font-family: "Poppins", sans-serif;
  font-weight: bold;
  font-size: 13px;
}

.contributors {
  display: flex;
  justify-content: flex-end;
  list-style: none;
  padding: 0;
  margin: 0;

  .contributor {
    position: relative;

    &:not(:first-child) {
      margin-left: -10px;
    }

    img {
      max-width: 30px;
      border-radius: 100px;
    }
  }
}
</style>