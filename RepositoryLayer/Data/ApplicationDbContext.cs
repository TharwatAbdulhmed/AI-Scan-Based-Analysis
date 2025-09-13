using System;
using DomainLayer.models;
using DomainLayer.models.AuthModles;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLayer.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        // DbSet Properties
        public DbSet<ScanAnalysis> ScanAnalyses { get; set; }
        public DbSet<AnalysisResult> AnalysisResults { get; set; }
        public DbSet<ModelType> ModelTypes { get; set; }
        public DbSet<Study> Studies { get; set; }

        // Constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Model Configuration
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ScanAnalysis and AnalysisResult relationship
            modelBuilder.Entity<ScanAnalysis>()
                .HasOne(sa => sa.analysisResult) // ScanAnalysis has one AnalysisResult
                .WithOne()                      // AnalysisResult belongs to one ScanAnalysis
                .HasForeignKey<AnalysisResult>(ar => ar.ScanAnalysisId) // Foreign key in AnalysisResult
                .OnDelete(DeleteBehavior.Cascade); // Cascading delete behavior

            // Configure ScanAnalysis entity
            modelBuilder.Entity<ScanAnalysis>(entity =>
            {
                entity.HasKey(sa => sa.Id); // Primary key
                entity.Property(sa => sa.ImagePath).IsRequired(false); // Optional property
                entity.Property(sa => sa.Result).IsRequired(false);    // Optional property
                entity.Property(sa => sa.CreatedDate).HasDefaultValueSql("GETUTCDATE()"); // Default value
            });

            // Configure AnalysisResult entity
            modelBuilder.Entity<AnalysisResult>(entity =>
            {
                entity.HasKey(ar => ar.Id); // Primary key
                entity.Property(ar => ar.Prediction).IsRequired(false); // Optional property
                entity.Property(ar => ar.Confidence).IsRequired();      // Required property
            });

            // Configure Study and ModelType relationship
            modelBuilder.Entity<Study>()
                .HasOne(s => s.ModelType)
                .WithMany()
                .HasForeignKey(s => s.ModelTypeId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
        }
    }
}