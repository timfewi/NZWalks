using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace NZWalks.API.Repositories
{
    public interface ITokenRepository
    {
       string CreateJWTToken(IdentityUser user, IList<string> roles);
    }
}
