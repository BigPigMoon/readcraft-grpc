using Grpc.Core;

namespace Server.Services;

public class AuthService : Auth.AuthBase
{
    public override Task<TokensResponse> Login(LoginRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }

    public override Task<TokensResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}