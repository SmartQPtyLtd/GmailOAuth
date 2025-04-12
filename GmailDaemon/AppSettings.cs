namespace GmailDaemon;

public class AppSettings
{
    public string SenderEmail { get; set; } = null!;
    public string EmlDirectoryPath { get; set; } = null!;
    public string ServiceAccountJsonFile { get; set; } = null!;
    public int MaxSize { get; set; }
}