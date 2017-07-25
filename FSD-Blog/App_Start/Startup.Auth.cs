using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using FSD_Blog.Models;
using Owin.Security.Providers.GitHub;
using Owin.Security.Providers.LinkedIn;
using Owin.Security.Providers.StackExchange;
using Microsoft.Owin.Security.Twitter;

namespace FSD_Blog
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers

            //app.UseTwitterAuthentication(new TwitterAuthenticationOptions()
            //{
            //    ConsumerKey = "fIb5ejLkWGLULpEjNPPhGu3fr",
            //    ConsumerSecret = "pcpLl601MkJVntuXUjGW5fR7Jr7BO3e8cbRF4XgS1K7b3MdcYp"
            //});

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions(){
                ClientId = "954845664170-ttrq1qel7pv9a2bp5pth2vd6ggibnhc1.apps.googleusercontent.com",
                ClientSecret = "qMETCT3qY5CE5ckg5vHPV7_A"});

            app.UseGitHubAuthentication(
                "566516530ac787c2a9b2",
                "8f514ecc8faa3ea4fbd4e45412d70b0eae3be9f0");

            app.UseLinkedInAuthentication(
                "78iom765f151im",
                "wp74wbuY3AZcGM8A");

            app.UseFacebookAuthentication(
               appId: "1857356494291158",
               appSecret: "f27c4e2b194c893d05ece394235c54d0");

            //app.UsePinterestAuthentication(
            //    "4912755025683432569",
            //    "ea45564bcf6a801f7fea9fcfec6b51f1a4f9e533bfdc51913f8c370cb059be1d");

            app.UseMicrosoftAccountAuthentication(
                clientId: "cc2211a9-8272-44ab-8c31-f64313489790",
                clientSecret: "2wBZfBPwa5SDj4rLPKZp3WE");

        }
    }
}