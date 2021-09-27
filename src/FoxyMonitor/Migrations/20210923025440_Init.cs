using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    poolName = table.Column<string>(type: "TEXT", nullable: true),
                    launcherId = table.Column<string>(type: "TEXT", nullable: false),
                    authToken = table.Column<string>(type: "TEXT", nullable: true),
                    lastBlockWon = table.Column<ulong>(type: "INTEGER", nullable: false),
                    lastAcceptedPartialTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    pendingBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    shares = table.Column<uint>(type: "INTEGER", nullable: false),
                    estCapacity = table.Column<decimal>(type: "TEXT", nullable: false),
                    collateral = table.Column<decimal>(type: "TEXT", nullable: false),
                    difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    distRatio = table.Column<string>(type: "TEXT", nullable: true),
                    payoutAddress = table.Column<string>(type: "TEXT", nullable: true),
                    poolPubKey = table.Column<string>(type: "TEXT", nullable: true),
                    lastUpdated = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "postPools",
                columns: table => new
                {
                    id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    poolApiName = table.Column<int>(type: "INTEGER", nullable: false),
                    poolUrl = table.Column<string>(type: "TEXT", nullable: true),
                    blockExplorerBlockUrlTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    blockExplorerCoinUrlTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    blockExplorerAddressUrlTemplate = table.Column<string>(type: "TEXT", nullable: true),
                    blockRewardDistributionDelay = table.Column<uint>(type: "INTEGER", nullable: false),
                    blocksPerDay = table.Column<uint>(type: "INTEGER", nullable: false),
                    defaultDistributionRatio = table.Column<string>(type: "TEXT", nullable: true),
                    historicalTimeInMinutes = table.Column<uint>(type: "INTEGER", nullable: false),
                    minimumPayout = table.Column<decimal>(type: "TEXT", nullable: false),
                    onDemandPayoutFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    poolFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    coin = table.Column<string>(type: "TEXT", nullable: true),
                    ticker = table.Column<string>(type: "TEXT", nullable: true),
                    version = table.Column<string>(type: "TEXT", nullable: true),
                    isTestnet = table.Column<bool>(type: "INTEGER", nullable: false),
                    poolAddress = table.Column<string>(type: "TEXT", nullable: true),
                    poolName = table.Column<string>(type: "TEXT", nullable: true),
                    farmingUrl = table.Column<string>(type: "TEXT", nullable: true),
                    height = table.Column<ulong>(type: "INTEGER", nullable: false),
                    difficulty = table.Column<uint>(type: "INTEGER", nullable: false),
                    receivedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    networkSpaceInTiB = table.Column<string>(type: "TEXT", nullable: true),
                    balance = table.Column<string>(type: "TEXT", nullable: true),
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
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    accountId = table.Column<uint>(type: "INTEGER", nullable: false),
                    level = table.Column<int>(type: "INTEGER", nullable: false),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    message = table.Column<string>(type: "TEXT", nullable: false),
                    url = table.Column<string>(type: "TEXT", nullable: true),
                    created = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    viewed = table.Column<bool>(type: "INTEGER", nullable: false),
                    pendingDeletion = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alerts_accounts_accountId",
                        column: x => x.accountId,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_accounts_displayName_poolName",
                table: "accounts",
                columns: new[] { "displayName", "poolName" },
                unique: true);

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
