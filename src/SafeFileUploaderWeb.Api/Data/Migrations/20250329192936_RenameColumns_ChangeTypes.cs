using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SafeFileUploaderWeb.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumns_ChangeTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniqueFileName",
                table: "UserFiles",
                newName: "FileName");

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "UserFiles",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "UserFiles",
                newName: "UniqueFileName");

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "UserFiles",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldDefaultValue: "");
        }
    }
}
