using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PW.Application.Common.Constants;
using PW.Identity.Entities;

namespace PW.Identity.Factories;

public class CustomUserClaimsPrincipalFactory
: UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
{
   public CustomUserClaimsPrincipalFactory(
       UserManager<ApplicationUser> userManager,
       RoleManager<ApplicationRole> roleManager,
       IOptions<IdentityOptions> optionsAccessor)
       : base(userManager, roleManager, optionsAccessor)
   {
   }

   protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
   {
      var identity = await base.GenerateClaimsAsync(user);

      identity.AddClaim(new Claim(CustomClaims.FullName, $"{user.FirstName} {user.LastName}"));

      return identity;
   }
}
