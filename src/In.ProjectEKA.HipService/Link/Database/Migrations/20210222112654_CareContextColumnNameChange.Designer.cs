﻿// <auto-generated />
using System;
using In.ProjectEKA.HipService.Link.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace In.ProjectEKA.HipService.Link.Database.Migrations
{
    [DbContext(typeof(LinkPatientContext))]
    [Migration("20210222112654_CareContextColumnNameChange")]
    partial class CareContextColumnNameChange
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("In.ProjectEKA.HipService.Link.Model.CareContext", b =>
                {
                    b.Property<string>("CareContextNumber")
                        .HasColumnType("text");

                    b.Property<string>("LinkReferenceNumber")
                        .HasColumnType("text");

                    b.HasKey("CareContextNumber", "LinkReferenceNumber")
                        .HasName("Id");

                    b.HasIndex("LinkReferenceNumber");

                    b.ToTable("CareContext");
                });

            modelBuilder.Entity("In.ProjectEKA.HipService.Link.Model.InitiatedLinkRequest", b =>
                {
                    b.Property<string>("RequestId")
                        .HasColumnType("text");

                    b.Property<string>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("LinkReferenceNumber")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValueSql("false");

                    b.Property<string>("TransactionId")
                        .HasColumnType("text");

                    b.HasKey("RequestId");

                    b.ToTable("InitiatedLinkRequest");
                });

            modelBuilder.Entity("In.ProjectEKA.HipService.Link.Model.LinkEnquires", b =>
                {
                    b.Property<string>("LinkReferenceNumber")
                        .HasColumnType("text");

                    b.Property<string>("ConsentManagerId")
                        .HasColumnType("text");

                    b.Property<string>("ConsentManagerUserId")
                        .HasColumnType("text");

                    b.Property<string>("DateTimeStamp")
                        .HasColumnType("text");

                    b.Property<string>("PatientReferenceNumber")
                        .HasColumnType("text");

                    b.HasKey("LinkReferenceNumber");

                    b.ToTable("LinkEnquires");
                });

            modelBuilder.Entity("In.ProjectEKA.HipService.Link.Model.LinkedAccounts", b =>
                {
                    b.Property<string>("LinkReferenceNumber")
                        .HasColumnType("text");

                    b.Property<string>("CareContexts")
                        .HasColumnType("text");

                    b.Property<string>("ConsentManagerUserId")
                        .HasColumnType("text");

                    b.Property<string>("DateTimeStamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasDefaultValueSql("now()");

                    b.Property<string>("PatientReferenceNumber")
                        .HasColumnType("text");

                    b.Property<Guid>("PatientUuid")
                        .HasColumnType("uuid");

                    b.HasKey("LinkReferenceNumber");

                    b.ToTable("LinkedAccounts");
                });

            modelBuilder.Entity("In.ProjectEKA.HipService.Link.Model.CareContext", b =>
                {
                    b.HasOne("In.ProjectEKA.HipService.Link.Model.LinkEnquires", "LinkEnquires")
                        .WithMany("CareContexts")
                        .HasForeignKey("LinkReferenceNumber")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
