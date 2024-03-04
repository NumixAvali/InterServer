using System.Text.Json;
using InterServer.Logic;

namespace InterServer.Controllers;

public class SettingsController
{
    public AppSettings GetSettings()
    {
        if (!File.Exists("./settings.json"))
        {
            Console.WriteLine("[Settings] Attempt to read non-existent settings file!");
            throw new InvalidDataException("Failed to read and return settings file value.");
        }

        string settingsString = File.ReadAllText("./settings.json");
        AppSettings appSettingsObj;

        try
        {
            appSettingsObj = JsonSerializer.Deserialize<AppSettings>(settingsString)!;
            return appSettingsObj;
        }
        catch (Exception e)
        {
            Console.WriteLine("[Settings] Malformed settings file:\n"+e);
            throw;
        }
    }
    
    public void SaveSettings(AppSettings appSettings)
    {
        string jsonSettings = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        if (File.Exists("./settings.json"))
        {
            try
            {
                File.Move("./settings.json","./settings.json.bak");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Settings] Error backing up settings file:\n"+e);
                // throw;
            }
        }

        try
        {
            File.WriteAllText("./settings.json",jsonSettings);
        }
        catch (Exception e)
        {
            Console.WriteLine("[Settings] Error writing settings file:\n"+e);
            throw;
        }
    }

}