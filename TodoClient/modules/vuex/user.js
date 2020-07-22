import axios from 'axios';

const user = {
    state: () => ({
        user: {},
        plan: {}
    }),
    mutations: {
        setUserData(state, data) {
            state.user = data;
        },
        setPlanData(state, data) {
            state.plan = data;
        }
    },
    actions: {
        async changePlan(context, { planName }) {
            try {
                await axios({
                    method: 'PUT',
                    url: '/api/accounts/role',
                    data: JSON.stringify({ plan: planName }),
                    headers: {
                        'content-type': 'application/json'
                    }
                });

                context.dispatch('getPlan');
            }
            catch (error) {
                throw error;
            }
        },
        async getPlan(context) {
            try {
                const response = await axios({
                    method: "GET",
                    url: "api/accounts/plan"
                });

                context.commit('setPlanData', response.data);
            }
            catch (error) {
                throw error;
            }
        }
    },
    getters: {
        user(state) {
            return state.user;
        },
        plan(state) {
            return state.plan;
        }
    }
}

export default user;