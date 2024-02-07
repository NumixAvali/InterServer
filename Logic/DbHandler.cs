namespace InterServer.Logic;
using MySql.Data.MySqlClient;

public class DbHandler
{
    private string Ip { get; set; }
    private string DbName { get; set; }
    private string Username { get; set; }
    private string UserPassword { get; set; }

    public DbHandler(string ip, string dbName, string username, string userPassword)
    {
        Ip = ip;
        DbName = dbName;
        Username = username;
        UserPassword = userPassword;
    }

    public void UploadData()
    {
        string connectionString = $"Server={Ip};Database={DbName};Uid={Username};Pwd={UserPassword};";
        
        using var connection = new MySqlConnection(connectionString);
        connection.Open();

        try
        {
            string sql = "INSERT INTO Measurements (column1, column2, ...) VALUES (@value1, @value2, ...)";
        
            using var command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@value1", "value1");
            command.Parameters.AddWithValue("@value2", "value2");

            command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        connection.Close();
    }

    public FrameInfo[] GetData()
    {
        throw new NotImplementedException();
    }
}