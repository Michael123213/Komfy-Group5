using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update passwords to encrypted versions
            // admin password: "admin" -> "QpillzkpeKyc+8j/cuKetg=="
            // user password: "user" -> "kQFwF4qT5I8C+TfGr5H8IA=="
            migrationBuilder.Sql(@"
                UPDATE Users SET Password = 'QpillzkpeKyc+8j/cuKetg==' WHERE UserId = 'admin';
                UPDATE Users SET Password = 'kQFwF4qT5I8C+TfGr5H8IA==' WHERE UserId = 'user';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
