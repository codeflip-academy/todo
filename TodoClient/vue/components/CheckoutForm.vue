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
      <!-- <b-row>
        <b-col>
          <b-form-group label="CVV" for="cc-cvv">
            <div class="form-control" id="cc-cvv"></div>
          </b-form-group>
        </b-col>
        <b-col>
          <b-form-group label="Postal Code" for="cc-postal-code">
            <div class="form-control" id="cc-postal-code"></div>
          </b-form-group>
        </b-col>
      </b-row>-->
      <div>
        <b-button type="submit" variant="success" :disabled="!allowFormSubmissions">Submit</b-button>
      </div>
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
      allowFormSubmissions: false,
    };
  },
  async created() {
    await this.generateClientToken();
    await this.createBrainTreeClient();
    await this.createHostedFieldsClient();
    this.allowFormSubmissions = true;
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
          // cvv: {
          //   selector: "#cc-cvv",
          //   placeholder: "123",
          // },
          expirationDate: {
            selector: "#cc-expiration",
            placeholder: "MM / YY",
          },
          // postalCode: {
          //   selector: "#cc-postal-code",
          //   placeholder: "11111",
          // },
        },
      });
    },
    async submitPaymentMethod() {
      await this.hostedFieldsClient.tokenize(async (err, payload) => {
        if (err) {
          console.log(err);
          return;
        }

        await axios({
          method: "POST",
          url: "/api/payments/",
          data: JSON.stringify({ paymentMethodNonce: payload.nonce }),
          headers: {
            "content-type": "application/json",
          },
        });
      });
    },
  },
  computed: {
    user() {
      return this.$store.getters.user;
    },
  },
};
</script>