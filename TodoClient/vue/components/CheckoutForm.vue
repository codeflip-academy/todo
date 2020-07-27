<template>
  <b-form>
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
      <b-row>
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
      </b-row>
      <div>
        <b-button type="submit" variant="success">Submit</b-button>
        <b-button variant="secondary" @click="$emit('cancelUpdate')">Cancel</b-button>
      </div>
    </b-card>
  </b-form>
</template>

<script>
import axios from "axios";
const client = require("braintree-web/client");
const hostedFields = require("braintree-web/hosted-fields");

export default {
  name: "CheckoutForm",
  data() {
    return {
      clientToken: "",
      clientInstance: null,
    };
  },
  async created() {
    this.clientToken = await this.getClientToken();
    this.clientInstance = client.create(
      {
        authorization: this.clientToken,
      },
      this.createCheckoutForm()
    );
  },
  methods: {
    async getClientToken() {
      const response = await axios({
        method: "GET",
        url: "/api/Payments/GenerateToken",
      });

      return response.data;
    },
    createCheckoutForm() {
      hostedFields.create({
        client: this.clientInstance,
        authorization: this.clientToken,
        fields: {
          number: {
            selector: "#cc-number",
            placeholder: "4111 1111 1111 1111",
          },
          cvv: {
            selector: "#cc-cvv",
            placeholder: "123",
          },
          expirationDate: {
            selector: "#cc-expiration",
            placeholder: "MM / YY",
          },
          postalCode: {
            selector: "#cc-postal-code",
            placeholder: "11111",
          },
        },
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