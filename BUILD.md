Build the Policy and Claims-Based Identity sample web application
===

1.  Open Visual Studio and create a new Web (ASP.NET Core Web) Application
	* Name Solution file: <b>Solution</b>
	* Name Project file:  <b>&lt;Solution&gt;</b>.Web.UI
	* Model View Controller
	* Configure for HTTPS
	* Configure Razor runtime compilation
	* No Authentication
2.	Configure Web UI project
	* Configure to debug the web app using Kestrel
	* Change Debug Options: Project->Debug->Profile: <b>&lt;Solution&gt;</b>.Web.UI
	* Web Server Settings: ``https://localhost:5005;http://localhost:5000``
	* Make sure you have <b>&lt;Solution&gt;</b>.Web.UI selected in the Debug launcher
3. Add key packages 
	* Logging (Serilog.ASPNetCore)
	* Razor Runtime Compilation (Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation)
	* Options (Microsoft.Extensions.Options)
4. Apply Bootstrap 4 settings
5. Configure Startup.cs
	* Add AddRazorRuntimeCompilation();
	* Add Cookie Authentication
	* Add Custom Policy / Claim Handlers
    * Add appsettings.json
6. Add Repository Classes
	* Data
	* POCO
	* Repository
	* Update namespaces and project dependancies
	* Add key packages: Logging, 
7. Create the Account Controller and Actions
	* Login
    * Register
	* Logout
    * Claims
8. Create the Account Views
	* LoginRegister
    * Claims
9. Create the Home Views
    * Logout
    * InvalidCredentials
    * LoginSuccess
    * About 
10. Identity Folder in Web.UI project
    * Custom Authorization Policy class
    * PasswordHash
	* HttpUser extension method
	* Add Cryptography package (Microsoft.AspNetCore.Cryptography.KeyDerivation)
11. Create the Secret Controller, Actions, and Views
    * Admin Secret
    * Basic Secret
    * Claims Secret
    * Combo Secret
    * GrownUp Secret
    * Manager Secret
    * No Role Secret
    * No Secret
12. Optionally, configure you computer for self-signed certificates on localhost
    * ``dotnet dev-certs https --trust``
    * This adds a self-signed certificate for <b>localhost</b> to the <b>Personal</b> certificate store on your computer
    * Note: Firefox does not use the Windows Certificate store by default
    * To configure it to do so, Open a new tab in Firefox
    * Type ``about:config``
    * Click <b>Accept the Risk and Continue</b>
    * Search for ``enterp``
    * On the <b>security.enterprise_root.enabled</b> line, click the <b>Toggle</b> link to enable the option
    * Firefox will now use the Windows Certificate store, where the localhost self-signed certificate lives. 

Links
---
 * [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-3.1)
 * [ASP.NET Core Identity without Entity Framework](https://markjohnson.io/articles/asp-net-core-identity-without-entity-framework/)
 * [Custom storage providers for ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-3.1)
 * [Learning ASP.NET Core Identity](https://docs.microsoft.com/en-us/learn/modules/secure-aspnet-core-identity/?view=aspnetcore-3.1)
 * [ASP.NET Core Identity video series](https://www.youtube.com/watch?v=Fhfvbl_KbWo)
 * [ASP.NET Core Identity - SignIn Manager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.signinmanager-1)
 * [ASP.NET Core Identity - User Manager](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.usermanager-1)_