# .NET Identity

## 1.Under The Hood Project: Understand simple authentication & authorization between a client and a server. 

This mode of authentication cannot be used outside its project domain.

- Security Context - contains all of the information about the user. That includes the user first name, last name, email address, country, etc.

- Claims Principle - This information is encapsulated within one single object called claims principle (carries all of the user information). The principle contains one of many identities of the user, called claims

- Claims - key value pairs that carry a user's identity information

- Cookie - a piece of information that is stored in the header of the Http request and Http response. This information is carried back and forth between the browser and the web server. The security context is usually in the cookie, and it is serialized and encrypted

## 2.Securing Web APIs using tokens:

We're not going to trigger the API directly from the browser, rather from the backend of the "Under The Hood" project. For that we'll need to use the http client factory to trigger the web API endpoint. For that, we wil install Microsoft.AspNetCore.Http.Extensions package from npm.

- Token - a string that is stored in the header of our Http request or Http response so it can freely go across project domains. It has 3 different parts separated by dots
	- hashing algorithm
	- claims that contain the user information
	- hashed result of the claims

## 3. ASP.NET Core Identity

The three essential parts of identity

- UI (Identity.UI nuget package, that contains all of the scaffolded user interfaces used to cover all scenarios)
- Authentication and authorization functionalities, which include verifying credentials and generating the security context aka authentication ticket (Identity nuget package)
- Data store, where the use information, claims and roles are stored (Identity.EntityFrameworkCore)
  * Additional frameworks are: EntityFrameworkCore.SqlServer for interacting with the SQL database, and EntityFrameworkCore.Tools and EntityFrameworkCore.Design for running migrations 

Main classes to work with
- SigningManager: helps with verifying credentials and create the security context, and authentication
- UserManager: helps to retrieve all of the user information stored in the database

Roles vs. Claims
- If your requirements needs more complicated logic in order to give permission to users to access resources, then you should use claims. Otherwise, you can use roles.
- Roles are simpler, but it cannot handle complicated scenarios.
- Claims are more flexible, but needs a little more work to do.
