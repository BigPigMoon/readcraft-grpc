using Grpc.Core;

namespace Server.Services.Book
{
    internal interface IBookService
    {
        Task<BookResponse> GetBook(long userId, long bookId);

        Task<BooksResponse> GetBooks(long userId);

        Task<StatusRespone> UploadBook(long userId, IAsyncStreamReader<BookFileRequest> requestStream);
    }
}
