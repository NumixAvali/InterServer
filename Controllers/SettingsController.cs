using System.Text.Json;
using InterServer.Logic;
using Microsoft.AspNetCore.Mvc;

namespace InterServer.Controllers;

[Route("interserver/internal")]
[Route("internal")]
[ApiController]
public class SettingsController : ControllerBase
{
    public AppSettings GetSettings()
    {
        if (!System.IO.File.Exists("./settings.json"))
        {
            Console.WriteLine("[Settings] Attempt to read non-existent settings file!");
            throw new InvalidDataException("Failed to read and return settings file value.");
        }

        string settingsString = System.IO.File.ReadAllText("./settings.json");
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
    
    private static void SaveSettings(AppSettings appSettings)
    {
        string jsonSettings = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        string[] backupLevels = { ".bak", ".bak1", ".bak2", ".bak3" };

        if (System.IO.File.Exists("./settings.json"))
        {
            try
            {
                for (int i = backupLevels.Length - 1; i > 0; i--)
                {
                    string currentBackupFile = $"./settings.json{backupLevels[i]}";
                    string previousBackupFile = $"./settings.json{backupLevels[i - 1]}";

                    if (System.IO.File.Exists(currentBackupFile))
                    {
                        System.IO.File.Delete(currentBackupFile);
                    }

                    if (System.IO.File.Exists(previousBackupFile))
                    {
                        System.IO.File.Move(previousBackupFile, currentBackupFile);
                    }
                }

                System.IO.File.Move("./settings.json", $"./settings.json{backupLevels[0]}");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Settings] Error backing up settings file:\n" + e);
                // throw;
            }
        }

        try
        {
            System.IO.File.WriteAllText("./settings.json", jsonSettings);
        }
        catch (Exception e)
        {
            Console.WriteLine("[Settings] Error writing settings file:\n" + e);
            throw;
        }
    }
    
    public string[] GetConfigs()
    {
        try
        {
            return Directory.GetFiles("./InverterConfigs","*.yaml");
        }
        catch (Exception e)
        {
            Console.WriteLine("[Settings] Error getting config list!\n"+e);
            throw;
        }
    }

    [HttpPost, Route("submit-settings")]
    public IActionResult ReceiveSettings([FromBody] AppSettings settings)
    {
        SaveSettings(settings);
        Console.WriteLine("[Settings] settings were updated.");
        return Ok();
    }
}