using Microsoft.EntityFrameworkCore.Migrations;
using ASI.Basecode.Data.Utilities;
using System;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedBooksFromGoogleAPI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Clear existing books to start fresh
            Console.WriteLine("Clearing existing books from the database...");
            migrationBuilder.Sql("DELETE FROM Reviews WHERE BookID IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM Borrowings WHERE BookID IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM Books");
            migrationBuilder.Sql("DBCC CHECKIDENT ('Books', RESEED, 0)"); // Reset identity
            Console.WriteLine("Books table cleared successfully.");

            // Step 2: Fetch books from Google Books API (or fallback data)
            Console.WriteLine("Fetching books from Google Books API...");
            var books = GoogleBooksSeeder.GetSeedBooks();
            Console.WriteLine($"Successfully fetched {books.Count} books.");

            // Step 3: Insert books into the database
            foreach (var book in books)
            {
                migrationBuilder.InsertData(
                    table: "Books",
                    columns: new[]
                    {
                        "BookID", "Title", "BookCode", "Author", "Publisher", "Genre",
                        "Status", "DatePublished", "Description", "CoverImagePath",
                        "IsEbook", "EbookPath", "ViewCount", "BorrowCount"
                    },
                    values: new object[]
                    {
                        book.BookID,
                        book.Title,
                        book.BookCode,
                        book.Author ?? "Unknown Author",
                        book.Publisher ?? "Unknown Publisher",
                        book.Genre,
                        book.Status,
                        book.DatePublished,
                        book.Description ?? "No description available.",
                        book.CoverImagePath ?? "",
                        book.IsEbook,
                        book.EbookPath ?? "",
                        book.ViewCount,
                        book.BorrowCount
                    });
            }

            Console.WriteLine("Books seeded successfully!");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Delete all seeded books
            migrationBuilder.Sql("DELETE FROM Reviews WHERE BookID IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM Borrowings WHERE BookID IS NOT NULL");
            migrationBuilder.Sql("DELETE FROM Books WHERE BookID >= 1 AND BookID <= 50");
        }
    }
}
