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
            return this.GetDbSet<Book>()
                .Include(b => b.Reviews)
                .Include(b => b.Borrowings);
        }

        public Book GetBookById(int bookId)
        {
            return this.GetDbSet<Book>()
                .Include(b => b.Reviews)
                .Include(b => b.Borrowings)
                .FirstOrDefault(b => b.BookID == bookId);
        }

        public Book GetBookByCode(string bookCode)
        {
            return this.GetDbSet<Book>()
                .Include(b => b.Reviews)
                .Include(b => b.Borrowings)
                .FirstOrDefault(b => b.BookCode == bookCode);
        }

        public IQueryable<Book> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetBooks();
            }

            searchTerm = searchTerm.ToLower();
            return this.GetDbSet<Book>()
                .Include(b => b.Reviews)
                .Include(b => b.Borrowings)
                .Where(b => b.Title.ToLower().Contains(searchTerm) ||
                           b.BookCode.ToLower().Contains(searchTerm) ||
                           b.Author.ToLower().Contains(searchTerm) ||
                           b.Genre.ToLower().Contains(searchTerm) ||
                           (b.Description != null && b.Description.ToLower().Contains(searchTerm)));
        }

        public IQueryable<Book> FilterBooks(string genre = null, string author = null, string publisher = null)
        {
            var query = this.GetDbSet<Book>()
                .Include(b => b.Reviews)
                .Include(b => b.Borrowings)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.ToLower() == genre.ToLower());
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.ToLower().Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                query = query.Where(b => b.Publisher.ToLower().Contains(publisher.ToLower()));
            }

            return query;
        }

        // CRITICAL FEATURE #3: Advanced Filter with Date Published and Sorting
        public IQueryable<Book> FilterBooksAdvanced(string genre = null, string author = null, string publisher = null,
                                                     int? yearPublished = null, string sortBy = null)
        {
            var query = this.GetDbSet<Book>()
                .Include(b => b.Reviews)
                .Include(b => b.Borrowings)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.ToLower() == genre.ToLower());
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.ToLower().Contains(author.ToLower()));
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                query = query.Where(b => b.Publisher.ToLower().Contains(publisher.ToLower()));
            }

            // CRITICAL FEATURE #3: Filter by year published
            if (yearPublished.HasValue)
            {
                query = query.Where(b => b.DatePublished.HasValue && b.DatePublished.Value.Year == yearPublished.Value);
            }

            // CRITICAL FEATURE #3: Sort by popularity or other criteria
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "popularity":
                    case "mostborrowed":
                        query = query.OrderByDescending(b => b.BorrowCount);
                        break;
                    case "mostviewed":
                        query = query.OrderByDescending(b => b.ViewCount);
                        break;
                    case "rating":
                    case "toprated":
                        query = query.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0);
                        break;
                    case "newest":
                        query = query.OrderByDescending(b => b.DatePublished);
                        break;
                    case "oldest":
                        query = query.OrderBy(b => b.DatePublished);
                        break;
                    case "title":
                        query = query.OrderBy(b => b.Title);
                        break;
                    default:
                        // Default sorting by title
                        query = query.OrderBy(b => b.Title);
                        break;
                }
            }
            else
            {
                // Default sorting
                query = query.OrderBy(b => b.Title);
            }

            return query;
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

        public void IncrementViewCount(int bookId)
        {
            var book = GetBookById(bookId);
            if (book != null)
            {
                book.ViewCount++;
                this.SetEntityState(book, EntityState.Modified);
                UnitOfWork.SaveChanges();
            }
        }

        public void IncrementBorrowCount(int bookId)
        {
            var book = GetBookById(bookId);
            if (book != null)
            {
                book.BorrowCount++;
                this.SetEntityState(book, EntityState.Modified);
                UnitOfWork.SaveChanges();
            }
        }
    }
}