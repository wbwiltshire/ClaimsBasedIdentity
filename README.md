Policy and Claims-Based Identity
===

A sample ASP.Net Core web application which implements a light-weight Policy and Claims-based Identity framework using the following:
* ASP.NET Core 3.1
* ASP.NET MVC 6
* Repository pattern using SQL Server  
* Bootstrap 4
* Handlebars.js 
* TurboTables

User POCO is based on ClaimsPrincipal class ()

Hash Ids
---

Use the HashIDs.Net library for secure, public facing User Ids.  Strategy is convert between the two (ApplicationUser Id and HashId) in cases when a user may see it (e.g. URLs)

Links
---
  * [ASP.NET Core Identity video series](https://www.youtube.com/watch?v=Fhfvbl_KbWo)
  * [Policy-Based And Role-Based Authorization In ASP.NET Core 3.0 Using Custom Handler](https://www.c-sharpcorner.com/article/policy-based-role-based-authorization-in-asp-net-core/)
  * [ASP.Net Core Custom Authorization Policy Providers](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/iauthorizationpolicyprovider)
  * [Use cookie authentication without ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view)
  * [Custom Authorization Policies and Requirements in ASP.Net Core](https://andrewlock.net/custom-authorisation-policies-and-requirements-in-asp-net-core/)
  * [ASP.NET Core Authorization Lab](https://github.com/blowdart/AspNetAuthorizationWorkshop)
  * [Retrieve the current user in an ASP.NET Core app](https://docs.microsoft.com/en-us/aspnet/core/migration/claimsprincipal-current)
  * [HashIds.Net](https://github.com/ullmark/hashids.net)
  * [Bootstrap 4](https://getbootstrap.com/)
  * [Handlebars](https://handlebarsjs.com/)
  * [Repository pattern using SQL Server](https://github.com/wbwiltshire/SQLRepositoryAsync)
  * [TurboTables](https://github.com/wbwiltshire/TurboTables)