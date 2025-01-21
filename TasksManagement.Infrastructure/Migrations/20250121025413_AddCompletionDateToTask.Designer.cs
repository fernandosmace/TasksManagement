﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TasksManagement.Infrastructure.Database;

#nullable disable

namespace TasksManagement.Infrastructure.Migrations
{
    [DbContext(typeof(SqlDbContext))]
    [Migration("20250121025413_AddCompletionDateToTask")]
    partial class AddCompletionDateToTask
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TasksManagement.Domain.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Comments_Id");

                    b.HasIndex("TaskId")
                        .HasDatabaseName("IX_Comments_TaskId");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Projects_Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Projects_Name");

                    b.HasIndex("UserId")
                        .HasDatabaseName("IX_Projects_UserId");

                    b.ToTable("Projects", (string)null);
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.TaskHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ChangedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ChangedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Changes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("TaskItemId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("TaskItemId");

                    b.ToTable("TaskHistory");
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.TaskItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CompletionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("CompletionDate")
                        .HasDatabaseName("IX_Tasks_CompletionDate");

                    b.HasIndex("DueDate")
                        .HasDatabaseName("IX_Tasks_DueDate");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Tasks_Id");

                    b.HasIndex("ProjectId")
                        .HasDatabaseName("IX_Tasks_ProjectId");

                    b.ToTable("Tasks", (string)null);
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Users_Id");

                    b.HasIndex("Name")
                        .HasDatabaseName("IX_Users_Name");

                    b.HasIndex("Role")
                        .HasDatabaseName("IX_Users_Role");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.Comment", b =>
                {
                    b.HasOne("TasksManagement.Domain.Entities.TaskItem", null)
                        .WithMany("Comments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.Project", b =>
                {
                    b.HasOne("TasksManagement.Domain.Entities.User", null)
                        .WithMany("Projects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Projects_Users");
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.TaskHistory", b =>
                {
                    b.HasOne("TasksManagement.Domain.Entities.TaskItem", null)
                        .WithMany("History")
                        .HasForeignKey("TaskItemId");
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.TaskItem", b =>
                {
                    b.HasOne("TasksManagement.Domain.Entities.Project", null)
                        .WithMany("Tasks")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.Project", b =>
                {
                    b.Navigation("Tasks");
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.TaskItem", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("History");
                });

            modelBuilder.Entity("TasksManagement.Domain.Entities.User", b =>
                {
                    b.Navigation("Projects");
                });
#pragma warning restore 612, 618
        }
    }
}
