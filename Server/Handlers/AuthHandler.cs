using Grpc.Core;
using Server.Services;
using Server.Services.Auth;

namespace Server.Handlers;

public class AuthHandler : AuthHand.AuthHandBase
{
    private readonly IAuthService _authService;

    public AuthHandler()
    {
        _authService = new AuthService();
    }

    public override Task<TokensResponse> Login(LoginRequest request, ServerCallContext context)
    {

        throw new NotImplementedException();
    }

    public override async Task<TokensResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        string email = request.Email.Trim();
        string username = request.Username.Trim();
        string password = request.Password.Trim();

        if (email == "")
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

        if (password == "")
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

        if (username == "")
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Username is required"));

        return await _authService.Register(email, username, password);
    }
}