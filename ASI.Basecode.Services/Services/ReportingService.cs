using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class ReportingService : IReportingService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBorrowingRepository _borrowingRepository;

        public ReportingService(
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IBorrowingRepository borrowingRepository)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _borrowingRepository = borrowingRepository;
        }

        // ADVANCED FEATURE #2: Borrowing Report
        public BorrowingReportModel GetBorrowingReport(string status = null)
        {
            var borrowings = _borrowingRepository.GetBorrowings().ToList();

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
            {
                borrowings = borrowings.Where(b => b.Status.ToLower() == status.ToLower()).ToList();
            }

            var report = new BorrowingReportModel
            {
                TotalBorrowings = borrowings.Count,
                ActiveBorrowings = borrowings.Count(b => b.Status == "Active"),
                ReturnedBorrowings = borrowings.Count(b => b.Status == "Returned"),
                OverdueBorrowings = borrowings.Count(b => b.Status == "Overdue"),
                Borrowings = borrowings.Select(b => new BorrowingModel
                {
                    BorrowingID = b.BorrowingID,
                    UserId = b.UserId,
                    BookID = b.BookID,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    Status = b.Status,
                    UserName = b.User?.Name,
                    UserEmail = b.User?.Email,
                    BookTitle = b.Book?.Title,
                    BookCode = b.Book?.BookCode
                }).ToList()
            };

            return report;
        }

        // ADVANCED FEATURE #2: Inventory Report
        public InventoryReportModel GetInventoryReport(string genre = null, string author = null, string publisher = null)
        {
            var books = _bookRepository.GetBooks().ToList();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(b => b.Genre.ToLower() == genre.ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.Author.ToLower().Contains(author.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(publisher))
            {
                books = books.Where(b => b.Publisher.ToLower().Contains(publisher.ToLower())).ToList();
            }

            // Calculate breakdown statistics
            var allBooks = _bookRepository.GetBooks().ToList();
            var booksByGenre = allBooks.GroupBy(b => b.Genre)
                                       .ToDictionary(g => g.Key, g => g.Count());
            var booksByAuthor = allBooks.GroupBy(b => b.Author)
                                        .ToDictionary(g => g.Key, g => g.Count());
            var booksByPublisher = allBooks.GroupBy(b => b.Publisher)
                                           .ToDictionary(g => g.Key, g => g.Count());

            var report = new InventoryReportModel
            {
                TotalBooks = books.Count,
                AvailableBooks = books.Count(b => b.Status == "Available"),
                BorrowedBooks = books.Count(b => b.Status == "Borrowed"),
                Ebooks = books.Count(b => b.IsEbook),
                BooksByGenre = booksByGenre,
                BooksByAuthor = booksByAuthor,
                BooksByPublisher = booksByPublisher,
                Books = books.Select(b => new BookModel
                {
                    BookID = b.BookID,
                    Title = b.Title,
                    BookCode = b.BookCode,
                    Genre = b.Genre,
                    Author = b.Author,
                    Publisher = b.Publisher,
                    Status = b.Status,
                    DatePublished = b.DatePublished,
                    Description = b.Description,
                    CoverImagePath = b.CoverImagePath,
                    IsEbook = b.IsEbook,
                    EbookPath = b.EbookPath,
                    ViewCount = b.ViewCount,
                    BorrowCount = b.BorrowCount,
                    AverageRating = b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0,
                    ReviewCount = b.Reviews.Count
                }).ToList()
            };

            return report;
        }

        // ADVANCED FEATURE #2: Member Report
        public MemberReportModel GetMemberReport()
        {
            var users = _userRepository.GetUsers().ToList();
            var activeBorrowings = _borrowingRepository.GetActiveBorrowings().ToList();
            var activeBorrowerIds = activeBorrowings.Select(b => b.UserId).Distinct().ToList();

            var report = new MemberReportModel
            {
                TotalMembers = users.Count(u => u.Role == "Member"),
                TotalAdmins = users.Count(u => u.Role == "Admin"),
                ActiveBorrowers = users.Count(u => activeBorrowerIds.Contains(u.UserId)),
                ReportGeneratedDate = DateTime.Now,
                Members = users.Select(u => new UserModel
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedBy = u.CreatedBy,
                    CreatedTime = u.CreatedTime,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedTime = u.UpdatedTime
                }).ToList()
            };

            return report;
        }
    }
}