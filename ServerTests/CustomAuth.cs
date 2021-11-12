using FubarDev.FtpServer.AccountManagement;
using System.Threading.Tasks;
using System.Security.Claims;
using FtpService;

namespace ServerTests
{
    public class CustomMembershipProvider : IMembershipProvider
    {
        public async Task<MemberValidationResult> 
            ValidateUserAsync(string username, string password)
        {
            var config = await MyConfig.ReadConfig();
            if (username == config.username && password == config.password)
            {
                var identity = new ClaimsIdentity();
                return new MemberValidationResult(
                       MemberValidationStatus.AuthenticatedUser,
                       new ClaimsPrincipal(identity));
            }

            return new MemberValidationResult(MemberValidationStatus.InvalidLogin);
        }
    }
}
