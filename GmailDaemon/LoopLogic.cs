using GmailOAuth;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static GmailDaemon.LoopInfrastructure;

namespace GmailDaemon;

public static class LoopLogic
{
    private const char Backslash = '\\';
    private const string MaxSize = nameof(MaxSize);
    private const int SafeToExit = 1200000;
    private const int ThrowAwayClient = 2400000;
    private const int WaitBeforeRecheck = 120000;
    public static string[] GetEmlFiles(string directory) => Directory.GetFiles(directory, "*.eml", SearchOption.TopDirectoryOnly);

    public static async Task LoopAsync(ServiceAccount serviceAccount, AppSettings appSettings, bool continueOnError = false)
    {
        Console.WriteLine("Daemon Starting up...");
        Initialize();

        while (IsRunning())
        {
            // The sender email must be a Google Workspace account that has been granted domain-wide delegation to the service account.
            using (var client = await Smtp.CreateServiceAccountSmtpClientAsync(serviceAccount, appSettings.SenderEmail).ConfigureAwait(false))
            {
                while (IsRunning() && GetElapsedMilliseconds() < ThrowAwayClient)
                {
                    try
                    {
                        await foreach (var item in PickupFolderAsync(client!,
                        GetEmlFiles(appSettings.EmlDirectoryPath), continueOnError, appSettings.MaxSize))
                            Console.WriteLine(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.InnerException?.Message ?? ex.Message);

                        if (!continueOnError)
                        {
                            try
                            {
                                await client.DisconnectAsync(true).ConfigureAwait(false);
                            }
                            finally
                            {
                                StopRunning();
                            }

                            break;
                        }
                    }

                    // Wait before rechecking the folder
                    await Task.Delay(WaitBeforeRecheck).ConfigureAwait(false);
                }

                try
                {
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
                finally
                {
                    Restart();
                }
            }

            // Safely close the window here
            Console.WriteLine("-- Safe Exit Point --");
            await Task.Delay(SafeToExit).ConfigureAwait(false);
        }

        StopTheClock();
    }

    public static async IAsyncEnumerable<string> PickupFolderAsync(SmtpClient client, string[] files, bool continueOnError, int maxSize, bool keepFiles = false,
            int intervalInMilliseconds = 5000)
    {
        FileInfo fileInfo;
        MimeMessage? message = null!;
        System.Text.StringBuilder line = new();

        Console.WriteLine("Checking For Emails...");
        foreach (var file in files)
        {
            line.Clear();

            try
            {
                fileInfo = new FileInfo(file);
                if (fileInfo.Length >= maxSize)
                {
                    Console.WriteLine($"{file} is too large to email, moving to {MaxSize} folder.");

                    if (!Directory.Exists(Path.Combine(file[..file.LastIndexOf(Backslash)], MaxSize)))
                        Directory.CreateDirectory(Path.Combine(file[..file.LastIndexOf(Backslash)], MaxSize));

                    File.Move(file, Path.Combine(file[..file.LastIndexOf(Backslash)], MaxSize, file[(file.LastIndexOf(Backslash) + 1)..]));
                    continue;
                }

                message = MimeMessage.Load(file);
                line.Append(string.Concat($"{DateTimeOffset.Now:O} | To: ", message.To.ToString(), ' '));
                await Task.Delay(intervalInMilliseconds).ConfigureAwait(false);
                await client.SendAsync(message);//.ConfigureAwait(true);
                line.Append("Sent!");
                if (!keepFiles)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        line.AppendLine(ex.InnerException?.Message ?? ex.Message);

                        if (!continueOnError)
                            throw;
                    }
                }
            }
            catch (Exception ex)
            {
                line.Append(ex.InnerException?.Message ?? ex.Message);

                if (!continueOnError)
                    throw;
            }
            finally
            {
                message?.Dispose();
            }

            yield return line.ToString();
        }
    }
}