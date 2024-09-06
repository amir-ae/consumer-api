﻿// <auto-generated />
using System;
using Commerce.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Commerce.Infrastructure.Migrations
{
    [DbContext(typeof(CommerceDbContext))]
    partial class CommerceDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("commerce")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Commerce.Domain.Common.ValueObjects.CustomerOrderLink", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("customer_id");

                    b.Property<string>("OrderId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("order_id");

                    b.Property<string>("CentreId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("centre_id");

                    b.HasKey("CustomerId", "OrderId");

                    b.ToTable("customer_order", "commerce");
                });

            modelBuilder.Entity("Commerce.Domain.Common.ValueObjects.CustomerProductLink", b =>
                {
                    b.Property<string>("CustomerId")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("customer_id");

                    b.Property<string>("ProductId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("product_id");

                    b.HasKey("CustomerId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("customer_product", "commerce");
                });

            modelBuilder.Entity("Commerce.Domain.Common.ValueObjects.ProductOrderLink", b =>
                {
                    b.Property<string>("ProductId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("product_id");

                    b.Property<string>("OrderId")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("order_id");

                    b.Property<string>("CentreId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("centre_id");

                    b.HasKey("ProductId", "OrderId");

                    b.ToTable("product_order", "commerce");
                });

            modelBuilder.Entity("Commerce.Domain.Customers.Customer", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("id");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("address");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("aggregate_id");

                    b.Property<int>("CityId")
                        .HasColumnType("integer")
                        .HasColumnName("city_id");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("first_name");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("full_name");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_deleted");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("last_name");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("middle_name");

                    b.Property<int>("Role")
                        .HasColumnType("integer")
                        .HasColumnName("role");

                    b.Property<int>("Version")
                        .HasColumnType("integer")
                        .HasColumnName("version");

                    b.HasKey("Id");

                    b.ToTable("customers", "commerce");
                });

            modelBuilder.Entity("Commerce.Domain.Products.Product", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("id");

                    b.Property<string>("AggregateId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("aggregate_id");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("brand");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by");

                    b.Property<DateTimeOffset?>("DateOfDemandForCompensation")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_of_demand_for_compensation");

                    b.Property<DateTimeOffset?>("DateOfPurchase")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date_of_purchase");

                    b.Property<string>("DealerId")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("dealer_id");

                    b.Property<string>("DemanderFullName")
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)")
                        .HasColumnName("demander_full_name");

                    b.Property<string>("DeviceType")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("device_type");

                    b.Property<string>("InvoiceNumber")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("invoice_number");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true)
                        .HasColumnName("is_active");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_deleted");

                    b.Property<bool>("IsUnrepairable")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_unrepairable");

                    b.Property<DateTimeOffset?>("LastModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_modified_at");

                    b.Property<Guid?>("LastModifiedBy")
                        .HasColumnType("uuid")
                        .HasColumnName("last_modified_by");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("model");

                    b.Property<string>("OwnerId")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)")
                        .HasColumnName("owner_id");

                    b.Property<string>("PanelModel")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("panel_model");

                    b.Property<string>("PanelSerialNumber")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("panel_serial_number");

                    b.Property<decimal?>("PurchasePrice")
                        .HasColumnType("numeric")
                        .HasColumnName("purchase_price");

                    b.Property<int?>("SerialId")
                        .HasColumnType("integer")
                        .HasColumnName("serial_id");

                    b.Property<int>("Version")
                        .HasColumnType("integer")
                        .HasColumnName("version");

                    b.Property<string>("WarrantyCardNumber")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("warranty_card_number");

                    b.HasKey("Id");

                    b.ToTable("products", "commerce");
                });

            modelBuilder.Entity("Commerce.Domain.Common.ValueObjects.CustomerOrderLink", b =>
                {
                    b.HasOne("Commerce.Domain.Customers.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Commerce.Domain.Common.ValueObjects.CustomerProductLink", b =>
                {
                    b.HasOne("Commerce.Domain.Customers.Customer", "Customer")
                        .WithMany("Products")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Commerce.Domain.Products.Product", "Product")
                        .WithMany("Customers")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Commerce.Domain.Common.ValueObjects.ProductOrderLink", b =>
                {
                    b.HasOne("Commerce.Domain.Products.Product", "Product")
                        .WithMany("Orders")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Commerce.Domain.Customers.Customer", b =>
                {
                    b.OwnsOne("Commerce.Domain.Customers.ValueObjects.PhoneNumber", "PhoneNumber", b1 =>
                        {
                            b1.Property<string>("CustomerId")
                                .HasColumnType("character varying(36)");

                            b1.Property<string>("CountryCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("character varying(10)")
                                .HasColumnName("phone_number_country_code");

                            b1.Property<string>("CountryId")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("character varying(10)")
                                .HasColumnName("phone_number_country_id");

                            b1.Property<string>("Description")
                                .HasMaxLength(50)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("phone_number_description");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(30)
                                .HasColumnType("character varying(30)")
                                .HasColumnName("phone_number_value");

                            b1.HasKey("CustomerId");

                            b1.ToTable("customers", "commerce");

                            b1.WithOwner()
                                .HasForeignKey("CustomerId");
                        });

                    b.Navigation("PhoneNumber")
                        .IsRequired();
                });

            modelBuilder.Entity("Commerce.Domain.Customers.Customer", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("Commerce.Domain.Products.Product", b =>
                {
                    b.Navigation("Customers");

                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
