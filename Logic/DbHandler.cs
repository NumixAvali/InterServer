using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterServer.Logic;

public class DbHandler : DbContext
{
    private readonly string _ip;
    private readonly string _dbName;
    private readonly string _username;
    private readonly string _userPassword;

    public DbSet<ReplyJsonEntity> MeasurementSet { get; set; }

    public DbHandler(string ip, string dbName, string username, string userPassword)
    {
        _ip = ip;
        _dbName = dbName;
        _username = username;
        _userPassword = userPassword;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = $"Server={_ip};Database={_dbName};Uid={_username};Pwd={_userPassword};";
        optionsBuilder.UseMySQL(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReplyJsonEntity>(ConfigureReplyJsonEntity);
    }

    private void ConfigureReplyJsonEntity(EntityTypeBuilder<ReplyJsonEntity> builder)
    {
        // Configure entity properties
        builder.Property(e => e.JsonData)
            .IsRequired();
    }
    
    public void UploadData(ReplyJson replyJson)
    {
        using (var context = new DbHandler(_ip, _dbName, _username, _userPassword))
        {
            var replyJsonEntity = new ReplyJsonEntity
            {
                JsonData = JsonSerializer.Serialize(replyJson)
            };
            context.MeasurementSet.Add(replyJsonEntity);
            context.SaveChanges();
            Console.WriteLine("[DB] Data pushed successfully.");
        }
    }

    public FrameInfo[] GetData()
    {
        throw new NotImplementedException();
    }
}