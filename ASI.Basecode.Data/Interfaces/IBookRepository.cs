using ASI.Basecode.Data.Models;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IBookRepository
    {
        IQueryable<Book> GetBooks();
        Book GetBookById(int bookId);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(Book book);
    }
}