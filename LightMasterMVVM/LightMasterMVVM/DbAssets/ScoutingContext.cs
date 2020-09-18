﻿using LightMasterMVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LightMasterMVVM.DbAssets
{
    public class ScoutingContext : DbContext
    {
        public DbSet<TeamMatch> Matches { get; set; }
        public DbSet<FRCTeamModel> FRCTeams { get; set; }
        public DbSet<TBA_DB_Model> TBAMatches { get; set; }
        public DbSet<TabletInstance> TabletInstances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=lightscoutx;User Id=strategy_member;Password=strategy");

    }
}
