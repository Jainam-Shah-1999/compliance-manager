using KpaFinAdvisors.Common.Models;
using KpaFinAdvisors.Common.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KpaFinAdvisors.Common.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;

        private readonly KpaFinAdvisorsDbContext _context;

        private readonly IDistributedCache _cache;

        public AuthenticationService(ILogger<AuthenticationService> logger, KpaFinAdvisorsDbContext context, IDistributedCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }
        public (User, string) Login(string userName, string password, ClaimsPrincipal loggedInUser)
        {
            try
            {
                var user = _context.Users.Where(user => user.Username == userName && user.Password == password).ToList().Single();
                if (user.Inactive)
                {
                    throw new UnauthorizedAccessException("The account is disabled, contact administrator.");
                }
                var token = GenerateAuthToken(user);
                return (user, token);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                throw new InvalidDataException("The username or password is incorrect, try again.");
            }
        }

        public async void Logout(string authToken)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                // Use a proper key or identifier for your tokens
                await _cache.SetStringAsync(authToken, "invalid", new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) // Token lifetime or your preference
                });
            }
        }

        private static string GenerateAuthToken(User user)
        {
            //var claims = new[]
            //{
            //new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
            //new Claim(ClaimTypes.Name, user.CompanyName),
            //new Claim(ClaimTypes.GivenName, user.RepresentativeName),
            //new Claim(ClaimTypes.Email, user.Email),
            //new Claim(ClaimTypes.MobilePhone, user.ContactNumber.ToString()),
            //new Claim(ClaimTypes.PrimarySid, user.Username),
            //new Claim(ClaimTypes.Sid, user.Password),
            //new Claim(ClaimTypes.Role, user.UserType.ToString()),
            //};

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourVeryLongSecureSecretKeyHere1234567890"));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(
            //    issuer: "JainamShah",
            //    audience: "KPAFinAdvisors",
            //    claims: claims,
            //    expires: DateTime.Now.AddHours(1),
            //    signingCredentials: creds);

            //return new JwtSecurityTokenHandler().WriteToken(token);


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("YourVeryLongSecureSecretKeyHere1234567890");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.SerialNumber, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.CompanyName),
                new Claim(ClaimTypes.GivenName, user.RepresentativeName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.ContactNumber.ToString()),
                new Claim(ClaimTypes.PrimarySid, user.Username),
                new Claim(ClaimTypes.Sid, user.Password),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expiration time
                Issuer = "JainamShah",
                Audience = "KPAFinAdvisors",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
