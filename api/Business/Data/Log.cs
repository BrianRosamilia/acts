using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace StargateAPI.Business.Data
{
    [Table("Log")]
    public class Log
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string LogLevel { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Exception { get; set; }

        public string? AdditionalData { get; set; }
    }

    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Timestamp).IsRequired();
            builder.Property(x => x.LogLevel).IsRequired();
            builder.Property(x => x.Message).IsRequired();
        }
    }
}