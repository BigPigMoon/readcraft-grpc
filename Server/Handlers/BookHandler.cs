using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Server.Services.Book;
using Server.Utils;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Handlers
{
    [Authorize(AuthPolicy.AccessPolicy)]
    internal class BookHandler : BookHand.BookHandBase
    {
        private readonly IBookService _bookService;

        public BookHandler()
        {
            _bookService = new BookService();
        }

        public override async Task<BookResponse> GetBook(BookRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue("authorization");

            if (accessToken == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token not found in header"));

            JwtSecurityToken jwt = JwtUtils.DecodeJwt(accessToken);
            long userId = JwtUtils.GetUid(jwt);

            return await _bookService.GetBook(userId, request.BookId);
        }

        public override async Task<BooksResponse> GetBooks(EmptyRequest request, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue("authorization");

            if (accessToken == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token not found in header"));

            JwtSecurityToken jwt = JwtUtils.DecodeJwt(accessToken);
            long userId = JwtUtils.GetUid(jwt);

            return await _bookService.GetBooks(userId);
        }

        public override async Task<StatusRespone> UploadBook(IAsyncStreamReader<BookFileRequest> requestStream, ServerCallContext context)
        {
            var accessToken = context.RequestHeaders.GetValue("authorization");

            if (accessToken == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Token not found in header"));

            JwtSecurityToken jwt = JwtUtils.DecodeJwt(accessToken);
            long userId = JwtUtils.GetUid(jwt);

            return await _bookService.UploadBook(userId, requestStream);
        }
    }
}
