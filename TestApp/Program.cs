using GmailOAuth;
using MimeKit;
using System;
using System.IO;
using System.Text.Json;
using TestApp;

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

if (string.IsNullOrWhiteSpace(appSettings.Subject) || string.IsNullOrWhiteSpace(appSettings.ReceiverEmail) ||
    string.IsNullOrWhiteSpace(appSettings.ProjectAuthJsonFile) || string.IsNullOrWhiteSpace(appSettings.To) ||
    string.IsNullOrWhiteSpace(appSettings.From) || string.IsNullOrWhiteSpace(appSettings.SenderEmail))
{
    Console.WriteLine("Invalid Configuration within App Settings! Exiting...");
    return;
}

// Load the service account credentials from the JSON file.
ProjectAuth? projectAuth = JsonSerializer.Deserialize<ProjectAuth>(File.ReadAllText(appSettings.ProjectAuthJsonFile), jsonOptions);

if (projectAuth is null)
{
    Console.WriteLine("Project Auth File Failed To Load! Exiting...");
    return;
}

// Create email message
using MimeMessage message = new();
message.From.Add(new MailboxAddress(appSettings.From, appSettings.SenderEmail));
message.To.Add(new MailboxAddress(appSettings.To, appSettings.ReceiverEmail));
message.Subject = $"{appSettings.Subject} {DateTime.Now}";
message.Body = new TextPart((appSettings.UseHtml!.Value) ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain)
{ Text = (appSettings.UseHtml!.Value) ? appSettings.BodyHtml : appSettings.BodyText };

// Opens Google login page to authorize the app.
using var smtp = await Smtp.CreateSmtpClientAsync(projectAuth, appSettings.SenderEmail).ConfigureAwait(false);

// Send the email
await smtp.SendAsync(message).ConfigureAwait(false);
Console.WriteLine("Sent!");
await smtp.DisconnectAsync(true).ConfigureAwait(false);