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

    public DbHandler()
    {
    }
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
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
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
        if (replyJson.Status != ResponseType.Ok || replyJson.Data == null)
        {
            Console.WriteLine("[DB] Received empty or errored 'replyJson' object, aborting upload.");
            return;
        }
        
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

    public List<FrameInfo> GetData()
    {
        using (var context = new DbHandler())
        {
            // This line causes DB timeout exception for some reason.
            // TODO: take a second look into this problem
            var replyJsonEntities = context.MeasurementSet.ToList();

            var frameInfos = replyJsonEntities.Select(entity =>
            {
                var replyJson = JsonSerializer.Deserialize<ReplyJson>(entity.JsonData);

                return replyJson.Data;
            }).ToList();

            return frameInfos;
        }
    }
}