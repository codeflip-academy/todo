import axios from 'axios';

let cartModel = {  
    FirstName: "Trenton",  
    LastName: "Hicks",  
    Email: "trentonmhicks@gmail.com",  
    Street: "Some St. 1234",  
    Price: 50
}

axios({
    method: 'GET',
    url: '/api/Payments/GenerateToken'
}).then((response) => {
    const clientToken = response.data;
    const button = document.querySelector('#submit-button');

    braintree.dropin.create({
        authorization: clientToken,
        container: '#dropin-container'
    }, (createErr, instance) => {
        button.addEventListener('click', function () {
        instance.requestPaymentMethod(function (err, payload) {
            if (err) {  
                console.log('Error', err);  
                return;
            }

            cartModel.PaymentMethodNonce = payload.nonce;
            checkout();
        });
        });
    });
});

function checkout() {
    axios({  
        method: 'POST',
        url: '/api/Payments/Checkout',  
        data: cartModel
    })
    .then((response) => {
        let paymentResult = response.data;
        console.log(paymentResult);
    });
}