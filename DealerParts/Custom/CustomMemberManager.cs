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
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomMemberManager(IIpResolver ipResolver, IMemberUserStore store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<MemberIdentityUser> passwordHasher, IEnumerable<IUserValidator<MemberIdentityUser>> userValidators, IEnumerable<IPasswordValidator<MemberIdentityUser>> passwordValidators, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<MemberIdentityUser>> logger, IOptionsSnapshot<MemberPasswordConfigurationSettings> passwordConfiguration, IPublicAccessService publicAccessService, IHttpContextAccessor httpContextAccessor) : base(ipResolver, store, optionsAccessor, passwordHasher, userValidators, passwordValidators, errors, services, logger, passwordConfiguration, publicAccessService, httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public override async Task<MemberIdentityUser?> GetUserAsync(ClaimsPrincipal principal)
        {
            //If we mutate the fetched MemberIdentityUser and call .AddRoles(), it will mutate the signed in user and add the roles
            //"permanently". Instead we create a new MemberIdentityUser and copy the props that is needed.
            var baseResult = await base.GetUserAsync(principal);
            var newMember = new MemberIdentityUser();
            newMember.Id = baseResult?.Id!;
            newMember.UserName = baseResult?.UserName;
            newMember.IsApproved = true;

            foreach (var claim in principal.Claims.Where(x => x.Type == ClaimTypes.Role))
            {
                newMember.AddRole(claim.Value);
            }
  
            return newMember;
        }
    }
}
