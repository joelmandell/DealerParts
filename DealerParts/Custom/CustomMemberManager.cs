using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Net;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Security;

namespace DealerParts.Custom
{
    public class CustomMemberManager : MemberManager
    {
        public CustomMemberManager(IIpResolver ipResolver, IMemberUserStore store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MemberIdentityUser> passwordHasher, IEnumerable<IUserValidator<MemberIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MemberIdentityUser>> passwordValidators, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MemberIdentityUser>> logger, IOptionsSnapshot<MemberPasswordConfigurationSettings> passwordConfiguration, IPublicAccessService publicAccessService, IHttpContextAccessor httpContextAccessor) : base(ipResolver, store, optionsAccessor, passwordHasher, userValidators, passwordValidators, errors, services, logger, passwordConfiguration, publicAccessService, httpContextAccessor)
        {
        }

        public override async Task<MemberIdentityUser?> GetUserAsync(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            var customUser = new MemberIdentityUser();
            customUser.UserName = ClaimConstants.CrmUser;
            customUser.IsApproved = true;

            foreach(var claim in principal.Claims.Where(x => x.Type == ClaimTypes.Role)) 
            {
                customUser.AddRole(claim.Value);
            }

            return await Task.FromResult(customUser);
        }
    }
}
