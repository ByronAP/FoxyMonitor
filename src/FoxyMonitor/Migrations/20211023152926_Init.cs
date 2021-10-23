using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FoxyMonitor.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    PoolType = table.Column<int>(type: "INTEGER", nullable: false),
                    PoolName = table.Column<string>(type: "TEXT", nullable: true),
                    LauncherId = table.Column<string>(type: "TEXT", nullable: true),
                    AuthToken = table.Column<string>(type: "TEXT", nullable: true),
                    LastBlockWon = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LastAcceptedPartialTime = table.Column<ulong>(type: "INTEGER", nullable: false),
                    PendingBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    Shares = table.Column<uint>(type: "INTEGER", nullable: false),
                    EstCapacity = table.Column<decimal>(type: "TEXT", nullable: false),
                    Collateral = table.Column<decimal>(type: "TEXT", nullable: false),
                    Difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    DistributionRatio = table.Column<string>(type: "TEXT", nullable: true),
                    PayoutAddress = table.Column<string>(type: "TEXT", nullable: true),
                    PoolPubKey = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<ulong>(type: "INTEGER", nullable: false),
                    AlertOnSlowPartials = table.Column<bool>(type: "INTEGER", nullable: false),
                    AlertOnBlockWon = table.Column<bool>(type: "INTEGER", nullable: false),
                    PayoutAddressBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    MaxPartialTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Alerts",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    AlertType = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    Created = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostPools",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoolApiName = table.Column<int>(type: "INTEGER", nullable: false),
                    PoolUrl = table.Column<string>(type: "TEXT", nullable: true),
                    BlockExplorerBlockUrlTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    BlockExplorerCoinUrlTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    BlockExplorerAddressUrlTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    BlockRewardDistributionDelay = table.Column<uint>(type: "INTEGER", nullable: false),
                    BlocksPerDay = table.Column<uint>(type: "INTEGER", nullable: false),
                    DefaultDistributionRatio = table.Column<string>(type: "TEXT", nullable: true),
                    HistoricalTimeInMinutes = table.Column<uint>(type: "INTEGER", nullable: false),
                    MinimumPayout = table.Column<decimal>(type: "TEXT", nullable: false),
                    OnDemandPayoutFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    PoolFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    Coin = table.Column<string>(type: "TEXT", nullable: true),
                    Ticker = table.Column<string>(type: "TEXT", nullable: true),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    IsTestnet = table.Column<bool>(type: "INTEGER", nullable: false),
                    PoolAddress = table.Column<string>(type: "TEXT", nullable: true),
                    PoolName = table.Column<string>(type: "TEXT", nullable: true),
                    FarmingUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Height = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    ReceivedAt = table.Column<ulong>(type: "INTEGER", nullable: false),
                    NetworkSpaceInTiB = table.Column<string>(type: "TEXT", nullable: true),
                    Balance = table.Column<string>(type: "TEXT", nullable: true),
                    AverageEffort = table.Column<decimal>(type: "TEXT", nullable: false),
                    DailyRewardPerPiB = table.Column<decimal>(type: "TEXT", nullable: false),
                    LastPayoutTime = table.Column<ulong>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountBalanceHistoricalDbItems",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PoolName = table.Column<string>(type: "TEXT", nullable: true),
                    LauncherId = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Balance = table.Column<ulong>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<uint>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalanceHistoricalDbItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountBalanceHistoricalDbItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostAccountHistoricalDbItems",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    EstCapacity = table.Column<decimal>(type: "TEXT", nullable: false),
                    ShareCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    Shares = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostAccountHistoricalDbItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostAccountHistoricalDbItems_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostPoolHistoricalDbItems",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PostPoolId = table.Column<uint>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Blocks = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Effort = table.Column<decimal>(type: "TEXT", nullable: false),
                    PoolEcInTib = table.Column<string>(type: "TEXT", nullable: true),
                    NetworkCapacityInTib = table.Column<string>(type: "TEXT", nullable: true),
                    PostPoolInfoId = table.Column<uint>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostPoolHistoricalDbItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostPoolHistoricalDbItems_PostPools_PostPoolInfoId",
                        column: x => x.PostPoolInfoId,
                        principalTable: "PostPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalanceHistoricalDbItems_AccountId",
                table: "AccountBalanceHistoricalDbItems",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountBalanceHistoricalDbItems_PoolName_LauncherId_Timestamp",
                table: "AccountBalanceHistoricalDbItems",
                columns: new[] { "PoolName", "LauncherId", "Timestamp" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_LauncherId_PoolName",
                table: "Accounts",
                columns: new[] { "LauncherId", "PoolName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alerts_AccountId_AlertType",
                table: "Alerts",
                columns: new[] { "AccountId", "AlertType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostAccountHistoricalDbItems_AccountId_CreatedAt",
                table: "PostAccountHistoricalDbItems",
                columns: new[] { "AccountId", "CreatedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPoolHistoricalDbItems_PostPoolId_Timestamp",
                table: "PostPoolHistoricalDbItems",
                columns: new[] { "PostPoolId", "Timestamp" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostPoolHistoricalDbItems_PostPoolInfoId",
                table: "PostPoolHistoricalDbItems",
                column: "PostPoolInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PostPools_PoolApiName",
                table: "PostPools",
                column: "PoolApiName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalanceHistoricalDbItems");

            migrationBuilder.DropTable(
                name: "Alerts");

            migrationBuilder.DropTable(
                name: "PostAccountHistoricalDbItems");

            migrationBuilder.DropTable(
                name: "PostPoolHistoricalDbItems");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "PostPools");
        }
    }
}
