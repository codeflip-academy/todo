<template>
  <b-form @submit.prevent="submitPaymentMethod">
    <b-card header="Update payment method">
      <b-row>
        <b-col>
          <b-form-group label="Card Number" for="cc-number">
            <div class="form-control" id="cc-number"></div>
          </b-form-group>
        </b-col>
        <b-col>
          <b-form-group label="Expiration Date" for="cc-expiration">
            <div class="form-control" id="cc-expiration"></div>
          </b-form-group>
        </b-col>
      </b-row>
      <div>
        <b-button type="submit" variant="success" :disabled="!formReady">Submit</b-button>
      </div>
      <b-alert variant="danger" class="mt-3 mb-0" :show="errorMsg !== ''">{{ errorMsg }}</b-alert>
    </b-card>
  </b-form>
</template>

<script>
import axios from "axios";
const braintree = require("braintree-web/client");
const hostedFields = require("braintree-web/hosted-fields");

export default {
  name: "CheckoutForm",
  data() {
    return {
      clientToken: "",
      brainTreeClient: null,
      hostedFieldsClient: null,
      formReady: false,
      errorMsg: "",
    };
  },
  async created() {
    await this.generateClientToken();
    await this.createBrainTreeClient();
    await this.createHostedFieldsClient();
    this.formReady = true;
  },
  methods: {
    async generateClientToken() {
      const response = await axios({
        method: "GET",
        url: "/api/Payments/GenerateToken",
      });

      this.clientToken = response.data;
    },
    async createBrainTreeClient() {
      this.brainTreeClient = await braintree.create({
        authorization: this.clientToken,
      });
    },
    async createHostedFieldsClient() {
      this.hostedFieldsClient = await hostedFields.create({
        client: this.brainTreeClient,
        authorization: this.clientToken,
        fields: {
          number: {
            selector: "#cc-number",
            placeholder: "4111 1111 1111 1111",
          },
          expirationDate: {
            selector: "#cc-expiration",
            placeholder: "MM / YY",
          },
        },
      });
    },
    async submitPaymentMethod() {
      this.formReady = false;

      await this.hostedFieldsClient.tokenize(async (err, payload) => {
        if (err) {
          this.displayErrorMessage(err.message);
          this.formReady = true;
          return;
        } else {
          this.hideErrorMessage();
        }

        await axios({
          method: "POST",
          url: "/api/payments/",
          data: JSON.stringify({ paymentMethodNonce: payload.nonce }),
          headers: {
            "content-type": "application/json",
          },
        });

        this.$emit("form-submitted");
        this.formReady = true;
      });
    },
    displayErrorMessage(msg) {
      this.errorMsg = msg;
    },
    hideErrorMessage() {
      this.errorMsg = "";
    },
  },
  computed: {
    user() {
      return this.$store.getters.user;
    },
  },
};
</script>