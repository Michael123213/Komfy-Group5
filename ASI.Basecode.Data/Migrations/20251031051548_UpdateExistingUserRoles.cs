using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing users with their proper roles
            migrationBuilder.Sql(@"
                UPDATE Users SET Role = 'Admin' WHERE UserId = 'admin' AND (Role IS NULL OR Role = '');
                UPDATE Users SET Role = 'Member' WHERE UserId = 'user' AND (Role IS NULL OR Role = '');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
