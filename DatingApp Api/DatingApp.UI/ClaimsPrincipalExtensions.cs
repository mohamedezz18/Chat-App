using System;
using System.Security.Claims;

namespace DatingApp.UI
{
    public static class ClaimsPrincipalExtensions
    {


        public static Guid GetMemberId(this ClaimsPrincipal user)
        {
            if (Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid id))
            {
                return id;
            }

            return Guid.Empty;

        }



    }
}
