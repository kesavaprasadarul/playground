﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AccountsTest.Core;

namespace AccountsTest.Migrations
{
    [DbContext(typeof(CoreAssets))]
    [Migration("20170325162917_AccountsDB")]
    partial class AccountsDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("AccountsTest.Core.Account", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountType");

                    b.HasKey("Id");

                    b.ToTable("Account");
                });

            modelBuilder.Entity("AccountsTest.Core.Transaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description");

                    b.Property<int>("PayType");

                    b.Property<double>("PriceAmount");

                    b.Property<int>("Tag");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Transaction");
                });
        }
    }
}