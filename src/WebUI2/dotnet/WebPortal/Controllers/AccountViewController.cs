﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 
using System.Net.Http;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WindowsAuth.models;

using WebPortal.Helper;
using WindowsAuth.Services;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WindowsAuth.Controllers
{
    public class AccountViewController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        // private IAzureAdTokenService _tokenCache;
        public AccountViewController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IOptions<AppSettings> appSettings, ILoggerFactory logger)
        {
            _signInManager = signInManager;
            _userManager = userManager; 
            _appSettings = appSettings.Value;
            _logger = logger.CreateLogger("AccountController");
        }

        // GET: /Account/Login
        [HttpGet("{scheme}")]
        public IActionResult Login(string scheme )
        {
            if (HttpContext.User == null || !HttpContext.User.Identity.IsAuthenticated)
            {
                if (Startup.AuthenticationSchemes.TryGetValue(scheme, out string provider ))
                {
                    return RedirectToPage($"./ExternalLogin", new { proivder = provider });
                    /* var redirectUrl = Url.Page("/ExternalLogin", pageHandler: "Callback");
                    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                    return new ChallengeResult(provider, properties); */
                } else if ( Startup.AuthenticationCallback.TryGetValue(scheme, out bool val ))
                {
                    // Sign in call back 
                }
            } 
            return View(); 
        }

        public IActionResult Login()
        {
            return View();
        }

        // Issue a challenge to send the user to AAD to sign in,
        // adding some additional data to the request which will be used in Startup.Auth.cs
        // The Tenant name here serves no functional purpose - it is only used to show how you
        // can collect additional information from the user during sign up.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task SignUp([Bind("ID", "Name", "AdminConsented")] Tenant tenant)
        {
            throw new Exception("Dead branch");
            await HttpContext.Authentication.ChallengeAsync(
                OpenIdConnectDefaults.AuthenticationScheme,
                new AuthenticationProperties(new Dictionary<string, string>
                {
                    { Constants.AdminConsentKey, tenant.AdminConsented.ToString() },
                    { Constants.TenantNameKey, tenant.Name }
                })
                { RedirectUri = "/" });
        }

        // GET: /Account/LogOff
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                OpenIDAuthentication config;
                var email = HttpContext.Session.GetString("Email");
                var scheme = Startup.GetAuthentication(email, out config);
                _logger.LogInformation("Log out account {0} using scheme {1}", email, scheme);
                
                // await HttpContext.Authentication.SignOutAsync(scheme);
                HttpContext.Session.Remove("isAuthorized");
                HttpContext.Session.Remove("isAdmin");
                HttpContext.Session.Remove("Email");
                HttpContext.Session.Remove("TenantID");
                HttpContext.Session.Remove("uid");
                HttpContext.Session.Remove("gid");
                HttpContext.Session.Remove("Password");
                HttpContext.Session.Remove("Restapi");
                HttpContext.Session.Remove("WorkFolderAccessPoint");
                HttpContext.Session.Remove("DataFolderAccessPoint");
                HttpContext.Session.Remove("AuthorizedClusters");
                HttpContext.Session.Remove("CurrentClusters");
                HttpContext.Session.Remove("Username");
                HttpContext.Session.Remove("ClustersList");
                await _signInManager.SignOutAsync();
                // await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public async Task EndSession()
        {
            if (User.Identity.IsAuthenticated)
            {
                IAzureAdTokenService tokenCache = (IAzureAdTokenService)HttpContext.RequestServices.GetService(typeof(IAzureAdTokenService));
                if ( !Object.ReferenceEquals(tokenCache, null))
                    tokenCache.Clear();
            }
            // If AAD sends a single sign-out message to the app, end the user's session, but don't redirect to AAD for sign out.
            await HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
