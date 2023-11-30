using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Storage;

namespace Server.Handlers
{
    internal class UserHandler : UserHand.UserHandBase
    {
        [Authorize(AuthPolicy.AccessPolicy)]
        public override Task<UsersResponse> Users(EmptyRequest request, ServerCallContext context)
        {
            Context ctx = Context.GetInstance();

            List<UserResponse> usersResponse = [.. ctx.db.Users.Select(user => new UserResponse { Id = user.Id, Username = user.Username })];

            return Task.FromResult(new UsersResponse() { Users = { usersResponse } });
        }
    }
}
