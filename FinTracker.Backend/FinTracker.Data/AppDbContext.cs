using Microsoft.EntityFrameworkCore;
using FinTracker.Domain.Models;

namespace FinTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Scope> Scopes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TransactionTag> TransactionTags { get; set; }
    public DbSet<Position> TransactionItems { get; set; }
    public DbSet<TransactionLink> TransactionLinks { get; set; }
    public DbSet<TransactionLinkEntry> TransactionLinkEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Transaction
        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.Id);  // PRIMARY KEY
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .IsRequired()  // NOT NULL
            .HasPrecision(18, 4);
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Currency)
            .IsRequired();  // NOT NULL
        modelBuilder.Entity<Transaction>()
            .Property(t => t.DateUtc)
            .IsRequired();  // NOT NULL
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .IsRequired();  // NOT NULL
        modelBuilder.Entity<Transaction>()
            .Property(t => t.IsDeleted)
            .IsRequired();  // NOT NULL
        modelBuilder.Entity<Transaction>()
            .Property(t => t.CategoryId)
            .IsRequired();  // NOT NULL
        //modelBuilder.Entity<Transaction>()
        //    .HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();// хранить как строку ("Income" / "Expense")
        #endregion

        #region Category
        modelBuilder.Entity<Category>()
            .HasKey(c => c.Id);  // PRIMARY KEY
        modelBuilder.Entity<Category>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);  // NOT NULL
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();  // UNIQUE
        #endregion

        #region Scope
        modelBuilder.Entity<Scope>()
            .HasKey(s => s.Id);  // PRIMARY KEY
        modelBuilder.Entity<Scope>()
            .Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);  // NOT NULL
        modelBuilder.Entity<Scope>()
            .HasIndex(s => s.Name)
            .IsUnique();  // UNIQUE
        #endregion

        #region Tag
        modelBuilder.Entity<Tag>()
            .HasKey(t => t.Id);  // PRIMARY KEY
        modelBuilder.Entity<Tag>()
            .Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(50);  // NOT NULL
        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Name)
            .IsUnique();  // UNIQUE
        #endregion

        #region TransactionTag
        modelBuilder.Entity<TransactionTag>()
            .HasKey(tt => new { tt.TransactionId, tt.TagId });  // PRIMARY KEY
                                                                //modelBuilder.Entity<TransactionTag>()
                                                                //    .HasQueryFilter(tt => !tt.Transaction.IsDeleted);
        #endregion

        #region TransactionItem
        modelBuilder.Entity<Position>()
            .HasKey(ti => ti.Id);
        modelBuilder.Entity<Position>()
            .Property(ti => ti.Amount)
                .HasPrecision(18, 4)
                .IsRequired();
        modelBuilder.Entity<Position>()
            .Property(ti => ti.Name)
                .IsRequired()
                .HasMaxLength(200);
        modelBuilder.Entity<Position>()
            .HasOne(ti => ti.Transaction)
                .WithMany(t => t.Positions)
                .HasForeignKey(ti => ti.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Position>()
            .HasOne(ti => ti.Category)
                .WithMany()
                .HasForeignKey(ti => ti.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        #endregion

        #region TransactionLinks
        modelBuilder.Entity<TransactionLink>()
            .HasKey(tl => tl.Id);
        modelBuilder.Entity<TransactionLink>()
            .Property(tl => tl.Type)
            .IsRequired()
            .HasConversion<string>(); // хранить как строку ("Compensation" / "Transfer")
        #endregion

        #region TransactionLinkEntries
        modelBuilder.Entity<TransactionLinkEntry>()
            .HasKey(tle => new { tle.TransactionLinkId, tle.TransactionId });
        modelBuilder.Entity<TransactionLinkEntry>()
            .HasOne(tle => tle.TransactionLink)
            .WithMany(tl => tl.Entries)
            .HasForeignKey(tle => tle.TransactionLinkId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TransactionLinkEntry>()
            .HasOne(tle => tle.Transaction)
            .WithMany(t => t.LinkEntries)
            .HasForeignKey(tle => tle.TransactionId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion

        base.OnModelCreating(modelBuilder);
    }
}
