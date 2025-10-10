using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        public BookRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<Book> GetBooks()
        {
            return this.GetDbSet<Book>();
        }

        public Book GetBookById(int bookId)
        {
            return this.GetDbSet<Book>().FirstOrDefault(b => b.BookID == bookId);
        }

        public void AddBook(Book book)
        {
            this.GetDbSet<Book>().Add(book);
            UnitOfWork.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            this.SetEntityState(book, EntityState.Modified);
            UnitOfWork.SaveChanges();
        }

        public void DeleteBook(Book book)
        {
            this.SetEntityState(book, EntityState.Deleted);
            UnitOfWork.SaveChanges();
        }
    }
}