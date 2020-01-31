# Payment Gateway for Checkout.com

The Payment Getaway is an API that allows merchants to make payments and retrieve details of processed ones. The API relies on an Acquiring Bank to process the payment.
The Acquiring Bank behaviour has been mocked using an implementation of IBankService assuming that would have an async method called ProcessPaymentAsync.
The implementation can easily be swapped with a real service which would make http calls to a real Acquiring Bank API.

## Authentication
Merchants who want to Make and Get Payments need to be authenticated. 
Authentication is achieved by using JWT tokens issued by the AuthController.
In order to get a token merchants need to make a GET request to "GetToken" method and retrieve the token from the response body.
The payments/make and payments/get methods will only respond if the Merchant is authenticated by adding the header
 
Authorization: Bearer {_token-retrieved-from-the-reponse-body_}
 
### GetToken
GET http://localhost:5000/auth/gettoken
Action to get an Authorisation token
The "GetToken" method should check the credentials provided by the merchant and SignIn only if they are valid but that's been omitted for simplicity and demo purposes, so the method will always return a valid token

### Make Payment
POST http://localhost:5000/payments
A typical workflow for the "Make payment" method involves
- receiving payment details
- validate payment details
- process payment through an acquiring bank service
- save payment details into a data storage so it can be retrieved later.
 
 body sample 
 
```javascript
{
	"CardNumber" : "4024-0071-7669-1425",
	"ExpiryDate" : "11/20",
	"Amount" : 10,
	"Currency" : 0,
	"Cvv" : "123",
	"Address" : "8 Baker Street, London",
	"NameOnCard" : "D Galifi"
}
```
 
### Get Payment
GET http://localhost:5000/Payments/get/{_payment-id_}
This is used by the merchant to retrieve details including a masked card number and name on card of a previously processed payment


## Encryption
The payment details get encrypted with an asymmetric encryption algorithm before being sent to the Acquiring bank. The Api knows the public key and it uses it to encrypt the payment details before sending it to the Acquirer in order to process the payment.
The Acquiring Bank will decrypt the message using the private key and will process the payment.
For demo purposes the BankServiceMock only accepts payments by credit card numbers starting with a '4'.
 
 
## Data storage
 Data storage is not implemented. The data for payments made are stored in memory and retrieved from there. Again IDataRepository implementation can be replaced with a real one.
 
 
## Credit card number samples
valid

* 4556-3171-3860-0430

* 4556-5867-7547-7720

non-valid

* 5113-2144-4170-9185

* 2221-0020-7985-2054

## Build scripts for docker
The repo contains also 2 powershell scripts for running the app in a docker container
 - docker-build.ps1: builds the co-img docker image
 - docker-run.ps1: runs the co-img in a container called co and exposes port 5000 so that the API is available at http://localhost:5000

