﻿// <auto-generated />
using System;
using FoxyMonitor.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoxyMonitor.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211023031320_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.11");

            modelBuilder.Entity("FoxyMonitor.Models.Account", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AlertOnBlockWon")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AlertOnSlowPartials")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AuthToken")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Collateral")
                        .HasColumnType("TEXT");

                    b.Property<uint>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("DistributionRatio")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("EstCapacity")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("LastAcceptedPartialTime")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LastBlockWon")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LastUpdated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LauncherId")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan>("MaxPartialTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("PayoutAddress")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PayoutAddressBalance")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("PendingBalance")
                        .HasColumnType("TEXT");

                    b.Property<string>("PoolName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PoolPubKey")
                        .HasColumnType("TEXT");

                    b.Property<int>("PoolType")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Shares")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("LauncherId", "PoolName")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("FoxyMonitor.Models.Alert", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlertType")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Created")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId", "AlertType")
                        .IsUnique();

                    b.ToTable("Alerts");
                });

            modelBuilder.Entity("FoxyMonitor.Models.PostAccountHistoricalDbItem", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("CreatedAt")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("EstCapacity")
                        .HasColumnType("TEXT");

                    b.Property<uint>("ShareCount")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Shares")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AccountId", "CreatedAt")
                        .IsUnique();

                    b.ToTable("PostAccountHistoricalDbItems");
                });

            modelBuilder.Entity("FoxyMonitor.Models.PostPoolHistoricalDbItem", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Blocks")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Effort")
                        .HasColumnType("TEXT");

                    b.Property<string>("NetworkCapacityInTib")
                        .HasColumnType("TEXT");

                    b.Property<string>("PoolEcInTib")
                        .HasColumnType("TEXT");

                    b.Property<uint>("PostPoolId")
                        .HasColumnType("INTEGER");

                    b.Property<uint?>("PostPoolInfoId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("Timestamp")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PostPoolInfoId");

                    b.HasIndex("PostPoolId", "Timestamp")
                        .IsUnique();

                    b.ToTable("PostPoolHistoricalDbItems");
                });

            modelBuilder.Entity("FoxyMonitor.Models.PostPoolInfo", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("AverageEffort")
                        .HasColumnType("TEXT");

                    b.Property<string>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockExplorerAddressUrlTemplate")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockExplorerBlockUrlTemplate")
                        .HasColumnType("TEXT");

                    b.Property<string>("BlockExplorerCoinUrlTemplate")
                        .HasColumnType("TEXT");

                    b.Property<uint>("BlockRewardDistributionDelay")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("BlocksPerDay")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Coin")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DailyRewardPerPiB")
                        .HasColumnType("TEXT");

                    b.Property<string>("DefaultDistributionRatio")
                        .HasColumnType("TEXT");

                    b.Property<uint>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FarmingUrl")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("HistoricalTimeInMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsTestnet")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LastPayoutTime")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("LastUpdated")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("MinimumPayout")
                        .HasColumnType("TEXT");

                    b.Property<string>("NetworkSpaceInTiB")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("OnDemandPayoutFee")
                        .HasColumnType("TEXT");

                    b.Property<string>("PoolAddress")
                        .HasColumnType("TEXT");

                    b.Property<int>("PoolApiName")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("PoolFee")
                        .HasColumnType("TEXT");

                    b.Property<string>("PoolName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PoolUrl")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ReceivedAt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Ticker")
                        .HasColumnType("TEXT");

                    b.Property<string>("Version")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PoolApiName")
                        .IsUnique();

                    b.ToTable("PostPools");
                });

            modelBuilder.Entity("FoxyMonitor.Models.PostAccountHistoricalDbItem", b =>
                {
                    b.HasOne("FoxyMonitor.Models.Account", null)
                        .WithMany("PostAccountHistoricalDbItems")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FoxyMonitor.Models.PostPoolHistoricalDbItem", b =>
                {
                    b.HasOne("FoxyMonitor.Models.PostPoolInfo", null)
                        .WithMany("PoolHistoricalDbItems")
                        .HasForeignKey("PostPoolInfoId");
                });

            modelBuilder.Entity("FoxyMonitor.Models.Account", b =>
                {
                    b.Navigation("PostAccountHistoricalDbItems");
                });

            modelBuilder.Entity("FoxyMonitor.Models.PostPoolInfo", b =>
                {
                    b.Navigation("PoolHistoricalDbItems");
                });
#pragma warning restore 612, 618
        }
    }
}
