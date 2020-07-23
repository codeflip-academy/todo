<template>
    
    <b-card title="Invite">
        <b-form @submit.prevent="invite" class="invitation-form">

            <b-form-group class="email-group text-secondary" label="Email">
                <b-form-input v-model="form.email" type="email" required></b-form-input>
            </b-form-group>

            <b-button type="submit">Send</b-button>

            <b-alert
                variant="success"
                class="mb-0 mt-3"
                :show="dismissCountDown"
                dismissable
                fade
                @dismissed="dismissCountDown=0"
                @dismiss-count-down="countDownChanged">
                Invitation sent!
            </b-alert>

            
        </b-form>
    </b-card>

</template>

<script>

    export default {
        props: ['listId'],
        data() {
            return {
                form: {
                    email: ''
                },
                invitationSent: false,
                dismissSecs: 5,
                dismissCountDown: 0
            }
        },
        methods: {
            invite() {
                this.$store.dispatch('inviteContributorToList', { listId: this.listId, email: this.form.email });
                this.form.email = '';
                this.showAlert();
            },
            countDownChanged(dismissCountDown) {
                this.dismissCountDown = dismissCountDown
            },
            showAlert() {
                this.dismissCountDown = this.dismissSecs
            }
        },
    }

</script>

<style lang="scss" scoped>

    h3 {
        font-size: 24px;
    }

    .invitation-form {
        .email-group {
            font-family: 'Nunito', sans-serif;
            font-weight: bold;
        }
    }

</style>