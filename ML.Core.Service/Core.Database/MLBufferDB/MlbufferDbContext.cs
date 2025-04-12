using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core.Database.MLBufferDB;

public partial class MlbufferDbContext : DbContext
{
    public MlbufferDbContext()
    {
    }

    public MlbufferDbContext(DbContextOptions<MlbufferDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aproject> Aprojects { get; set; }

    public virtual DbSet<TransFile> TransFiles { get; set; }

    public virtual DbSet<TransPage> TransPages { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=MLBuffer_db;User Id=sa;Password=Admin1234!;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Thai_CI_AI");

        modelBuilder.Entity<Aproject>(entity =>
        {
            entity.ToTable("AProject");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProjectNo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TransFile>(entity =>
        {
            entity.Property(e => e.CreaterDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DocNo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FilePath)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.TransId).HasColumnName("Trans_Id");

            entity.HasOne(d => d.Trans).WithMany(p => p.TransFiles)
                .HasForeignKey(d => d.TransId)
                .HasConstraintName("FK_TransFiles_Transaction");
        });

        modelBuilder.Entity<TransPage>(entity =>
        {
            entity.ToTable("TransPage");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Hocr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("HOcr");
            entity.Property(e => e.Locr)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LOcr");
            entity.Property(e => e.RecDrawingNo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.SymbolType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.File).WithMany(p => p.TransPages)
                .HasForeignKey(d => d.FileId)
                .HasConstraintName("FK_TransPage_TransPage");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Transaction_AProject");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
