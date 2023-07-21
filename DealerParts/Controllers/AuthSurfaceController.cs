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

        public AuthSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, IMemberSignInManager memberSignInManager, IMemberManager memberManager) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this.memberSignInManager = memberSignInManager;
            this.memberManager = memberManager;
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

            var crmUser = await memberManager.FindByNameAsync("CrmUser");

            var role = "";
            var crmUserName = "";

            switch (model.Password)
            {
                case "admin":
                    role = "Dealership Administrators";
                    crmUserName = "Got Rootson";
                    break;
                case "customer":
                    role = "Retail Customers";
                    crmUserName = "Sir Buy Alot";
                    break;
                default:
                    role = "Parts Department Staff";
                    crmUserName = "Mr Always Employee of the month";
                    break;
            }

            var customClaims = new List<Claim>
            {
                new(ClaimTypes.Role, role),
                new("CrmUserName",crmUserName)
            };

            await (memberSignInManager as MemberSignInManager)!.SignInWithClaimsAsync(crmUser!, true, customClaims);

            return Redirect("/");
        }
    }
}
