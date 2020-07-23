import axios from 'axios';

let data = {};

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

            data.paymentMethodNonce = payload.nonce;
            addPaymentMethod();
        });
        });
    });
});

function addPaymentMethod() {
    axios({  
        method: 'POST',
        url: '/api/payments/',
        data: JSON.stringify(data),
        headers: {
            'content-type': 'application/json'
        }
    })
    .then((response) => {
        let paymentResult = response.data;
    });
}