using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FoxyMonitor.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    displayName = table.Column<string>(type: "TEXT", nullable: false),
                    poolType = table.Column<int>(type: "INTEGER", nullable: false),
                    poolName = table.Column<string>(type: "TEXT", nullable: false),
                    launcherId = table.Column<string>(type: "TEXT", nullable: false),
                    authToken = table.Column<string>(type: "TEXT", nullable: false),
                    lastBlockWon = table.Column<ulong>(type: "INTEGER", nullable: false),
                    lastAcceptedPartialTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    pendingBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    shares = table.Column<uint>(type: "INTEGER", nullable: false),
                    estCapacity = table.Column<decimal>(type: "TEXT", nullable: false),
                    collateral = table.Column<decimal>(type: "TEXT", nullable: false),
                    difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    distRatio = table.Column<string>(type: "TEXT", nullable: false),
                    payoutAddress = table.Column<string>(type: "TEXT", nullable: false),
                    poolPubKey = table.Column<string>(type: "TEXT", nullable: false),
                    lastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    accountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    level = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    message = table.Column<string>(type: "TEXT", nullable: false),
                    url = table.Column<string>(type: "TEXT", nullable: false),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    viewed = table.Column<bool>(type: "INTEGER", nullable: false),
                    pendingDeletion = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "postPools",
                columns: table => new
                {
                    id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    poolApiName = table.Column<int>(type: "INTEGER", nullable: false),
                    poolUrl = table.Column<string>(type: "TEXT", nullable: false),
                    blockExplorerBlockUrlTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    blockExplorerCoinUrlTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    blockExplorerAddressUrlTemplate = table.Column<string>(type: "TEXT", nullable: false),
                    blockRewardDistributionDelay = table.Column<uint>(type: "INTEGER", nullable: false),
                    blocksPerDay = table.Column<uint>(type: "INTEGER", nullable: false),
                    defaultDistributionRatio = table.Column<string>(type: "TEXT", nullable: false),
                    historicalTimeInMinutes = table.Column<uint>(type: "INTEGER", nullable: false),
                    minimumPayout = table.Column<decimal>(type: "TEXT", nullable: false),
                    onDemandPayoutFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    poolFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    coin = table.Column<string>(type: "TEXT", nullable: false),
                    ticker = table.Column<string>(type: "TEXT", nullable: false),
                    version = table.Column<string>(type: "TEXT", nullable: false),
                    isTestnet = table.Column<bool>(type: "INTEGER", nullable: false),
                    poolAddress = table.Column<string>(type: "TEXT", nullable: false),
                    poolName = table.Column<string>(type: "TEXT", nullable: false),
                    farmingUrl = table.Column<string>(type: "TEXT", nullable: false),
                    height = table.Column<ulong>(type: "INTEGER", nullable: false),
                    difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    receivedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    networkSpaceInTiB = table.Column<string>(type: "TEXT", nullable: false),
                    balance = table.Column<string>(type: "TEXT", nullable: false),
                    avgEffort = table.Column<decimal>(type: "TEXT", nullable: false),
                    dailyRewardPerPiB = table.Column<decimal>(type: "TEXT", nullable: false),
                    lastPayoutTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    lastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postPools", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "postAccountHistoricalItems",
                columns: table => new
                {
                    id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    accountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    estCapacity = table.Column<decimal>(type: "TEXT", nullable: false),
                    shareCount = table.Column<uint>(type: "INTEGER", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    shares = table.Column<uint>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postAccountHistoricalItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_postAccountHistoricalItems_accounts_accountId",
                        column: x => x.accountId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_accountId",
                table: "alerts",
                column: "accountId");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_pendingDeletion",
                table: "alerts",
                column: "pendingDeletion");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_viewed",
                table: "alerts",
                column: "viewed");

            migrationBuilder.CreateIndex(
                name: "IX_postAccountHistoricalItems_accountId",
                table: "postAccountHistoricalItems",
                column: "accountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");

            migrationBuilder.DropTable(
                name: "postAccountHistoricalItems");

            migrationBuilder.DropTable(
                name: "postPools");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
