// <copyright file="CustomMembershipProvider.cs" company="Fubar Development Junker">
// Copyright (c) Fubar Development Junker. All rights reserved.
// </copyright>

using FubarDev.FtpServer.AccountManagement;
using System.Threading.Tasks;
using System.Security.Claims;
using MyConfigNamespace;

namespace FtpServer
{
    public class CustomMembershipProvider : IMembershipProvider
    {
        /// <inheritdoc />
        public async Task<MemberValidationResult> ValidateUserAsync(string username, string password)
        {
            var config = await MyConfig.ReadConfig();
            if (username == config.username && password == config.password)
            {
                var identity = new ClaimsIdentity();
                // return Task.FromResult(
                return new MemberValidationResult(
                       MemberValidationStatus.AuthenticatedUser,
                       new ClaimsPrincipal(identity));
            }

            // return Task.FromResult(new MemberValidationResult(MemberValidationStatus.InvalidLogin));
            return new MemberValidationResult(MemberValidationStatus.InvalidLogin);
        }
    }
}