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
                Timestamp = replyJson.Timestamp,
                JsonData = JsonSerializer.Serialize(replyJson)
            };
            context.MeasurementSet.Add(replyJsonEntity);
            context.SaveChanges();
            Console.WriteLine("[DB] Data pushed successfully.");
        }
    }

    public List<ReplyJson> GetAllData()
    {
        List<ReplyJson> finalList = new List<ReplyJson>();
        using (var context = new DbHandler(_ip, _dbName, _username, _userPassword))
        {
            var replyJsonEntities = context.MeasurementSet.ToList();
            
            foreach (var entity in replyJsonEntities)
            {
                try
                {
                    var replyJson = JsonSerializer.Deserialize<ReplyJson>(entity.JsonData, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    finalList.Add(replyJson);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error converting JSON into object:\n" + entity.JsonData);
                }
            }
        }
        return finalList;
    }
}