﻿// <auto-generated />
using System;
using LightMasterMVVM.DbAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace LightMasterMVVM.Migrations
{
    [DbContext(typeof(ScoutingContext))]
    partial class ScoutingContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("LightMasterMVVM.Models.TeamMatch", b =>
                {
                    b.Property<int>("MatchID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("A_InitiationLine")
                        .HasColumnType("boolean");

                    b.Property<int>("DisabledSeconds")
                        .HasColumnType("integer");

                    b.Property<bool>("E_Balanced")
                        .HasColumnType("boolean");

                    b.Property<bool>("E_ClimbAttempt")
                        .HasColumnType("boolean");

                    b.Property<bool>("E_ClimbSuccess")
                        .HasColumnType("boolean");

                    b.Property<bool>("E_Park")
                        .HasColumnType("boolean");

                    b.Property<string>("EventCode")
                        .HasColumnType("text");

                    b.Property<int>("MatchNumber")
                        .HasColumnType("integer");

                    b.Property<int>("NumCycles")
                        .HasColumnType("integer");

                    b.Property<int[]>("PowerCellInner")
                        .HasColumnType("integer[]");

                    b.Property<int[]>("PowerCellLower")
                        .HasColumnType("integer[]");

                    b.Property<int[]>("PowerCellMissed")
                        .HasColumnType("integer[]");

                    b.Property<int[]>("PowerCellOuter")
                        .HasColumnType("integer[]");

                    b.Property<string>("RobotPosition")
                        .HasColumnType("text");

                    b.Property<string>("ScoutName")
                        .HasColumnType("text");

                    b.Property<bool>("T_ControlPanelPosition")
                        .HasColumnType("boolean");

                    b.Property<bool>("T_ControlPanelRotation")
                        .HasColumnType("boolean");

                    b.Property<string>("TeamName")
                        .HasColumnType("text");

                    b.Property<int>("TeamNumber")
                        .HasColumnType("integer");

                    b.HasKey("MatchID");

                    b.ToTable("Matches");
                });
#pragma warning restore 612, 618
        }
    }
}
