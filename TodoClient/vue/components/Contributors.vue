<template>

    <ul class="contributors">
        <li v-for="contributor in todoListContributors" :key="contributor" class="contributor">
            <img
            :src="accountContributors[contributor].pictureUrl"
            :alt="accountContributors[contributor].fullName"
            v-b-tooltip.hover :title="accountContributors[contributor].fullName">
        </li>
    </ul>
    
</template>

<script>
    export default {
        name: "Contributors",
        props: ['todoListContributors', 'accountContributors'],
    }
</script>

<style lang="scss" scoped>

    .b-tooltip {
        font-weight: bold;
        font-family: 'Nunito', sans-serif;
    }

    .contributors {
        display: flex;
        list-style: none;
        padding: 0;
        margin: 0;

        .contributor {
            position: relative;

            &:not(:first-child) {
                margin-left: -10px;
            }

            img {
                max-width: 36px;
                border-radius: 100px;
            }
        }
    }

</style>