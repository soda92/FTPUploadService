using FubarDev.FtpServer.AccountManagement;
using System.Threading.Tasks;
using System.Security.Claims;
using FtpService;

namespace ServerTests
{
    public class CustomMembershipProvider : IMembershipProvider
    {
        public Task<MemberValidationResult> 
            ValidateUserAsync(string username, string password)
        {
            var config = MyConfig.GetExampleConfig();
            if (username == config.Username && password == config.Password)
            {
                var identity = new ClaimsIdentity();
                return Task.FromResult(new MemberValidationResult(
                       MemberValidationStatus.AuthenticatedUser,
                       new ClaimsPrincipal(identity)));
            }

            return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
        }
    }
}
