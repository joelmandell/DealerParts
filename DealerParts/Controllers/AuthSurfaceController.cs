using DealerParts.Custom;
using DealerParts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Models;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace DealerParts.Controllers
{
    public class AuthSurfaceController : SurfaceController
    {
        private readonly IMemberSignInManager memberSignInManager;
        private readonly IMemberManager memberManager;
        private readonly CrmService crmService;

        public AuthSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IMemberSignInManager memberSignInManager, IMemberManager memberManager, CrmService crmService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this.memberSignInManager = memberSignInManager;
            this.memberManager = memberManager;
            this.crmService = crmService;
        }

        [HttpGet]
        [Route("/SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await memberSignInManager.SignOutAsync();
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please provide username and password");
                return CurrentUmbracoPage();
            }

            var crmAuthorized = await crmService.SignIn(model?.Username!, model?.Password!);

            if (!crmAuthorized.Success)
            {
                ModelState.AddModelError(string.Empty, "Wrong credentials");
                return CurrentUmbracoPage();
            }

            var crmUser = await memberManager.FindByNameAsync(ClaimConstants.CrmUser);

            var customRoles = new List<Claim>
            {
                new(ClaimTypes.Role, crmAuthorized?.Role ?? ""),
                new(ClaimConstants.CrmClaimUserName, crmAuthorized?.UserName ?? ""),
            };

            foreach(var claim in customRoles)
            {
                crmUser.Claims.Add(new() { ClaimType = claim.Type, ClaimValue = claim.Value, UserId = crmUser?.Id });
            }

            await memberSignInManager.SignInAsync(crmUser!, true);

            return Redirect("/");
        }
    }
}
