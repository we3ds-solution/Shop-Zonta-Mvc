using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace marketmvc.Infrastructure.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// REVIEW WARNING: this migration also brings a stale model snapshot into sync
    /// (Identity tables, index renames, column length alignment, Items table drop,
    /// Customer verification columns drop). Do NOT apply to production without a
    /// backup + reviewed deployment plan and explicit ZONTA_AUTO_MIGRATE=true.
    /// The destructive parts (DropTable Items, DropColumn EmailVerifiedAt/LastLoginAt/
    /// PhoneVerifiedAt) must be confirmed against the real production schema first.
    /// </remarks>
    public partial class AddProductVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropColumn(
                name: "EmailVerifiedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PhoneVerifiedAt",
                table: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Status",
                table: "Products",
                newName: "IX_Product_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Sku",
                table: "Products",
                newName: "IX_Product_Sku_Unique");

            migrationBuilder.RenameIndex(
                name: "IX_Products_IsFeatured",
                table: "Products",
                newName: "IX_Product_IsFeatured");

            migrationBuilder.RenameIndex(
                name: "IX_Products_IsDeleted",
                table: "Products",
                newName: "IX_Product_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CreatedAt",
                table: "Products",
                newName: "IX_Product_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                newName: "IX_Product_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                newName: "IX_Product_Barcode_Unique");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                newName: "IX_Order_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_PaymentStatus",
                table: "Orders",
                newName: "IX_Order_PaymentStatus");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                newName: "IX_Order_OrderNumber_Unique");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_IsDeleted",
                table: "Orders",
                newName: "IX_Order_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                newName: "IX_Order_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerEmail",
                table: "Orders",
                newName: "IX_Order_CustomerEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CreatedAt",
                table: "Orders",
                newName: "IX_Order_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItem_OrderId_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_IsDeleted",
                table: "OrderItems",
                newName: "IX_OrderItem_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_IsDeleted",
                table: "Customers",
                newName: "IX_Customer_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_IsActive",
                table: "Customers",
                newName: "IX_Customer_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                newName: "IX_Customer_Email_Unique");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_CreatedAt",
                table: "Customers",
                newName: "IX_Customer_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Type",
                table: "Categories",
                newName: "IX_Category_Type");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                newName: "IX_Category_Name_Unique");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                newName: "IX_Category_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_IsActive",
                table: "Categories",
                newName: "IX_Category_IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "WeightUnit",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "Products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductVariantId",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VariantLabel",
                table: "OrderItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastPasswordChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockoutReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Roles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Variant_Product",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_VariantId",
                table: "OrderItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ApplicationUserId",
                table: "Customers",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CustomerId",
                table: "AspNetUsers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Variant_IsDeleted",
                table: "ProductVariants",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Variant_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Variant_Sku",
                table: "ProductVariants",
                column: "Sku");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_ApplicationUserId",
                table: "Customers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Product",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Customer",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_ApplicationUserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Product",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Customer",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category",
                table: "Products");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_VariantId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Customer_ApplicationUserId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "VariantLabel",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Status",
                table: "Products",
                newName: "IX_Products_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Sku_Unique",
                table: "Products",
                newName: "IX_Products_Sku");

            migrationBuilder.RenameIndex(
                name: "IX_Product_IsFeatured",
                table: "Products",
                newName: "IX_Products_IsFeatured");

            migrationBuilder.RenameIndex(
                name: "IX_Product_IsDeleted",
                table: "Products",
                newName: "IX_Products_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CreatedAt",
                table: "Products",
                newName: "IX_Products_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Product_CategoryId",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Barcode_Unique",
                table: "Products",
                newName: "IX_Products_Barcode");

            migrationBuilder.RenameIndex(
                name: "IX_Order_Status",
                table: "Orders",
                newName: "IX_Orders_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PaymentStatus",
                table: "Orders",
                newName: "IX_Orders_PaymentStatus");

            migrationBuilder.RenameIndex(
                name: "IX_Order_OrderNumber_Unique",
                table: "Orders",
                newName: "IX_Orders_OrderNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Order_IsDeleted",
                table: "Orders",
                newName: "IX_Orders_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CustomerId",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CustomerEmail",
                table: "Orders",
                newName: "IX_Orders_CustomerEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedAt",
                table: "Orders",
                newName: "IX_Orders_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_OrderId_ProductId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItem_IsDeleted",
                table: "OrderItems",
                newName: "IX_OrderItems_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_IsDeleted",
                table: "Customers",
                newName: "IX_Customers_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_IsActive",
                table: "Customers",
                newName: "IX_Customers_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_Email_Unique",
                table: "Customers",
                newName: "IX_Customers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Customer_CreatedAt",
                table: "Customers",
                newName: "IX_Customers_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Category_Type",
                table: "Categories",
                newName: "IX_Categories_Type");

            migrationBuilder.RenameIndex(
                name: "IX_Category_Name_Unique",
                table: "Categories",
                newName: "IX_Categories_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Category_IsDeleted",
                table: "Categories",
                newName: "IX_Categories_IsDeleted");

            migrationBuilder.RenameIndex(
                name: "IX_Category_IsActive",
                table: "Categories",
                newName: "IX_Categories_IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "WeightUnit",
                table: "Products",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerifiedAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhoneVerifiedAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsActive",
                table: "Items",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsDeleted",
                table: "Items",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Sku",
                table: "Items",
                column: "Sku",
                unique: true,
                filter: "[Sku] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Products_ProductId",
                table: "OrderItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
