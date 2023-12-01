using Grpc.Core;

namespace Server.Services.Book
{
    internal class BookService : IBookService
    {
        public Task<BookResponse> GetBook(long userId, long bookId)
        {
            throw new NotImplementedException();
        }

        public Task<BooksResponse> GetBooks(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<StatusRespone> UploadBook(long userId, IAsyncStreamReader<BookFileRequest> requestStream)
        {
            throw new NotImplementedException();
        }
    }
}
