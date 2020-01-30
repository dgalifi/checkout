# Payment Gateway for

The Payment Getaway is an API that allows merchant to make payments and retrieve details of a past one. The API relies on an Acquiring Bank to process the payment.
The Acquiring Bank behaviour has been mocked using an implementation of IBankService assuming that would have an async method called ProcessPaymentAsync.
The implementation can easily be swapped with a real service which would make http calls to a real Acquiring Bank API.

 ## Authentication
 Merchants who want to Make and Get Payments needs to be authenticated. 
 Authentication is achieved by using JWT tokens issued by the AuthController.
 In order to get a token the merchants need to make a GET request to http://localhost:5000/auth/GetToken and retrieve the token in the response body.
 The "GetToken" should check the credentials provided by the merchant and SignIn only if they are valid but that's been omitted for simplicity and demo purposes.
 The payments/make and payments/get method will only respond with a 200 if the Merchant is authenticated by adding the header
 Authorization: Bearer ${_token-retrieved-from-the-reponse-body_}
 
### GetToken
http://localhost:5000/auth/gettoken

### Make Payment
http://localhost:5000/payments
A typical workflow for the "Make payment" method involves
 - receiving payment details
 - validate payment details
 - process payment through an acquiring bank service
 - save payment details into a data storage so it can be retrieved later
 
 ## Encryption
 The payment details gets encrypted with an asymmetric encryption algorithm. The Api knows the public key and it uses it to encrypt the payment details before sending it to the AcquirerBank in order to process the payment.
 The Acquiring Bank will decrypt the message using the private key and process the payment.
 For demo purposes the BankService will only accept payment for credit card numbers starting with a '4'
 

 
 ## Data storage
 Data storage is not implemented. The data for payments made are stored in memory and retrieved from there. Again IDataRepository implementation can be replaced with a real one.
 
 ### Credit card
4556-3171-3860-0430
4556-5867-7547-7720

5113-2144-4170-9185
2221-0020-7985-2054

## Build scripts for docker
The repo contains also 2 powershell scripts for running the app in a docker container
 - docker-build.ps1: builds the co-img docker image
 - docker-run.ps1: runs the co-img in a container called co and exposes port 5000 so that the API is available at http://localhost:5000

