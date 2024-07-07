using KpaFinAdvisors.Common.Models;
using System.Security.Claims;

namespace KpaFinAdvisors.Common.Services.Interfaces
{
    public interface IAuthenticationService
    {
        public (User, string) Login(string userName, string password, ClaimsPrincipal user);

        public void Logout(string authToken);
    }
}
