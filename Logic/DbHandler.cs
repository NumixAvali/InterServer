using System.Text.Json;
using InterServer.Controllers;
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
    
    // For cases, where DB credentials need to be overriden 
    public DbHandler(string ip, string dbName, string username, string userPassword)
    {
        _ip = ip;
        _dbName = dbName;
        _username = username;
        _userPassword = userPassword;
    }

    public DbHandler()
    {
        AppSettings settings = new SettingsController().GetSettings();
        
        _ip = settings.DbIp;
        _dbName = settings.DbName;
        _username = settings.DbUsername;
        _userPassword = settings.DbPassword;
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
            try
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
                        Console.WriteLine("[DB] Error converting JSON into object:\n" + entity.JsonData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[DB] Error getting all DB data.\n"+e);
                return null;
            }
        }
        return finalList;
    }

    public ReplyJson GetDataByTimestamp(long timestamp)
    {
        using (var context = new DbHandler(_ip, _dbName, _username, _userPassword))
        {
            try
            {
                var entry = context.MeasurementSet
                    .Single(m => m.Timestamp == timestamp);

                var dbEntry = JsonSerializer.Deserialize<ReplyJson>(entry.JsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                return dbEntry;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DB] Requested timestamp DB entry is unavailable.\n" + e);
                return null;
            }
        }
    }
    
    public List<ReplyJson> GetDataRange(double timestampStart, double timestampEnd)
    {
        List<ReplyJson> finalList = new List<ReplyJson>();
        using (var context = new DbHandler(_ip, _dbName, _username, _userPassword))
        {
            try
            {
                var replyJsonEntities = context.MeasurementSet
                    .Where(m => m.Timestamp >= timestampStart && m.Timestamp <= timestampEnd)
                    .ToList();
            
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
                        Console.WriteLine("[DB] Error converting JSON into object:\n" + entity.JsonData);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[DB] Error getting DB data range.\n"+e);
                return null;
            }
        }

        return finalList;
    }
    
    public ReplyJson GetLatestData()
    {
        using (var context = new DbHandler(_ip, _dbName, _username, _userPassword))
        {
            try
            {
                var entry = context.MeasurementSet
                    .OrderBy(m => m.Id)
                    .LastOrDefault();
        
                return JsonSerializer.Deserialize<ReplyJson>(entry.JsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DB] Error getting latest DB entry\n"+e);
                return null;
            }
        }
    }

}