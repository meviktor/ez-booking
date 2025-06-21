using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingWebAPI.Testing.Common.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo_testing");

            migrationBuilder.CreateTable(
                name: "ResourceCategories",
                schema: "dbo_testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BaseCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceCategories_ResourceCategories_BaseCategoryId",
                        column: x => x.BaseCategoryId,
                        principalSchema: "dbo_testing",
                        principalTable: "ResourceCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                schema: "dbo_testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ValueType = table.Column<short>(type: "smallint", nullable: false),
                    RawValue = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: false),
                    Category = table.Column<short>(type: "smallint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                schema: "dbo_testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    County = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HouseOrFlatNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.CheckConstraint("CK_Site_StateCounty", "COALESCE(State, County) IS NOT NULL");
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "dbo_testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ResourceCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_ResourceCategories_ResourceCategoryId",
                        column: x => x.ResourceCategoryId,
                        principalSchema: "dbo_testing",
                        principalTable: "ResourceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resources_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "dbo_testing",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "dbo_testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    PasswordHash = table.Column<string>(type: "CHAR(60)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkHoursWeekly = table.Column<int>(type: "int", nullable: true),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "dbo_testing",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailConfirmationAttempts",
                schema: "dbo_testing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    FailReason = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailConfirmationAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailConfirmationAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo_testing",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailConfirmationAttempts_UserId",
                schema: "dbo_testing",
                table: "EmailConfirmationAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceCategories_BaseCategoryId",
                schema: "dbo_testing",
                table: "ResourceCategories",
                column: "BaseCategoryId");

            migrationBuilder.CreateIndex(
                name: "UQ_ResourceCategory_Name",
                schema: "dbo_testing",
                table: "ResourceCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_ResourceCategoryId",
                schema: "dbo_testing",
                table: "Resources",
                column: "ResourceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_SiteId",
                schema: "dbo_testing",
                table: "Resources",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "UQ_Resource_Name",
                schema: "dbo_testing",
                table: "Resources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Setting_NameCategory",
                schema: "dbo_testing",
                table: "Settings",
                columns: new[] { "Name", "Category" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Site_Name",
                schema: "dbo_testing",
                table: "Sites",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId",
                schema: "dbo_testing",
                table: "Users",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "UQ_User_Email",
                schema: "dbo_testing",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_User_UserName",
                schema: "dbo_testing",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailConfirmationAttempts",
                schema: "dbo_testing");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "dbo_testing");

            migrationBuilder.DropTable(
                name: "Settings",
                schema: "dbo_testing");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "dbo_testing");

            migrationBuilder.DropTable(
                name: "ResourceCategories",
                schema: "dbo_testing");

            migrationBuilder.DropTable(
                name: "Sites",
                schema: "dbo_testing");
        }
    }
}
