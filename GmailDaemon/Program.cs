using GmailDaemon;
using GmailOAuth;
using System;
using System.IO;
using System.Text.Json;
using static GmailDaemon.LoopLogic;

var jsonOptions = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

// Load the app settings from the JSON file.
AppSettings? appSettings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText("AppSettings.json"), jsonOptions);

if (appSettings is null)
{
    Console.WriteLine("App Settings Failed To Load! Exiting...");
    return;
}

// Validate the app settings.
if (string.IsNullOrWhiteSpace(appSettings.SenderEmail) || string.IsNullOrWhiteSpace(appSettings.EmlDirectoryPath) ||
    string.IsNullOrWhiteSpace(appSettings.ServiceAccountJsonFile))
{
    Console.WriteLine("Invalid Configuration within App Settings! Exiting...");
    return;
}

// Assuming user made a mistake. Set Limit To 1MB (Change as needed)
if (appSettings.MaxSize < 1048576)
    appSettings.MaxSize = 1048576;

// Load the service account credentials from the JSON file.
ServiceAccount? serviceAccount = JsonSerializer.Deserialize<ServiceAccount>(File.ReadAllText(appSettings.ServiceAccountJsonFile), jsonOptions);

if (serviceAccount is null)
{
    Console.WriteLine("Service Account File Failed To Load! Exiting...");
    return;
}

// Validate the service account credentials.
if (string.IsNullOrWhiteSpace(serviceAccount.Client_email) || string.IsNullOrWhiteSpace(serviceAccount.Private_key))
{
    Console.WriteLine("Invalid Configuration within Service Account File! Exiting...");
    return;
}

await LoopAsync(serviceAccount, appSettings, /* Continue, Even If There Are Errors */ true).ConfigureAwait(false);