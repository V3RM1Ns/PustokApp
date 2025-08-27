using Microsoft.EntityFrameworkCore;
using Pustok.Models;

namespace Pustok.Configs;

public class BookTagConfig: IEntityTypeConfiguration<BookTag>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<BookTag> builder)
    {
        builder.HasKey(bt => new { bt.BookId, bt.TagId });

        builder
            .HasOne(bt => bt.Book)
            .WithMany(b => b.BookTags)
            .HasForeignKey(bt => bt.BookId);

        builder
            .HasOne(bt => bt.Tag)
            .WithMany(t => t.BookTags)
            .HasForeignKey(bt => bt.TagId);
    }
}