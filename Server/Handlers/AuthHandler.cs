using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Server.Services.Auth;
using Server.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Handlers;

public class AuthHandler : AuthHand.AuthHandBase
{
    private readonly IAuthService _authService;

    public AuthHandler()
    {
        _authService = new AuthService();
    }

    public override async Task<TokensResponse> Login(LoginRequest request, ServerCallContext context)
    {
        string email = request.Email.Trim();
        string password = request.Password.Trim();

        if (email == "")
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

        if (password == "")
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

        return await _authService.Login(email, password);
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

    public override async Task<TokensResponse> Refresh(EmptyRequest request, ServerCallContext context)
    {
        var refreshtToken = context.RequestHeaders.GetValue("authorization");

        if (refreshtToken == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Refresh token not found in header"));

        return await _authService.Refresh(refreshtToken.Split(' ')[1]);
    }

    [Authorize(AuthPolicy.AccessPolicy)]
    public override async Task<EmptyRequest> Logout(EmptyRequest request, ServerCallContext context)
    {
        var accessToken = context.RequestHeaders.GetValue("authorization");

        if (accessToken == null)
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Token not found in header"));

        JwtSecurityToken jwt = JwtUtils.DecodeJwt(accessToken);
        int userId = JwtUtils.GetUid(jwt);

        await _authService.Logout(userId);

        return new EmptyRequest();
    }
}