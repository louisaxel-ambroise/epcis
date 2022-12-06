﻿using FasTnT.Application.Relational.Configuration;
using FasTnT.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Migrations.Postgres;

internal class PostgresModelConfiguration : BaseRelationalModelConfiguration
{
    public override void Apply(ModelBuilder modelBuilder)
    {
        base.Apply(modelBuilder);

        modelBuilder.Entity<Event>().Property<long>("Id").UseIdentityColumn();
    }
}