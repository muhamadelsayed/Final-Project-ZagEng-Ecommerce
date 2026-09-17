using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260917020000_AddProductSoftDelete")]
public partial class AddProductSoftDelete : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "deleted_at",
            table: "products",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "is_deleted",
            table: "products",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "deleted_at", table: "products");
        migrationBuilder.DropColumn(name: "is_deleted", table: "products");
    }
}
