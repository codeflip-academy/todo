<template>
  <div class="coupon-form-wrapper">
    <div>
      <small>
        <a
          href="#"
          class="text-muted"
          @click.prevent
          v-b-toggle.coupon-form-collapse
        >Have a coupon code?</a>
      </small>
    </div>
    <b-collapse id="coupon-form-collapse">
      <b-form id="coupon-form" class="mt-4" @submit.prevent="applyCoupon">
        <b-row class="align-items-end">
          <b-col>
            <b-form-group label="Coupon code" for="coupon-code" class="mb-0">
              <b-form-input
                id="coupon-code"
                type="text"
                pattern=".{6,}"
                maxlength="6"
                required="required"
                autofocus="autofocus"
                title="6 characters required"
                v-model="couponCode"
              ></b-form-input>
            </b-form-group>
          </b-col>
          <b-col class="text-right">
            <b-button variant="success" size="sm" type="submit">Apply coupon</b-button>
          </b-col>
        </b-row>
        <div class="form-submission-response">
          <b-alert
            :show="couponRedeemedMessage !== ''"
            :class="{ 'alert-success': couponRedeemed, 'alert-danger': !couponRedeemed }"
            class="mt-3"
          >{{ couponRedeemedMessage }}</b-alert>
        </div>
      </b-form>
    </b-collapse>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "CouponForm",
  data() {
    return {
      couponCode: "",
      couponRedeemed: null,
      couponRedeemedMessage: "",
    };
  },
  methods: {
    async applyCoupon() {
      try {
        const response = await axios({
          method: "POST",
          url: "/api/payments/coupons/redeem",
          data: JSON.stringify({ couponCode: this.couponCode }),
          headers: {
            "content-type": "application/json",
          },
        });

        this.showCodeRedemptionMessage(response.data, true);
      } catch (error) {
        console.log("error");
        this.showCodeRedemptionMessage("Unable to redeem code", false);
      }
    },
    showCodeRedemptionMessage(msg, status) {
      this.couponRedeemed = status;
      this.couponRedeemedMessage = msg;
      this.couponCode = "";
    },
  },
};
</script>