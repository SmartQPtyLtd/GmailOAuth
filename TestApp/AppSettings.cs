namespace TestApp;


public class AppSettings
{
    public string? BodyHtml { get; set; }
    public string? BodyText { get; set; }
    public string From { get; set; } = null!;
    public string ProjectAuthJsonFile { get; set; } = null!;
    public string ReceiverEmail { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string To { get; set; } = null!;
    public bool? UseHtml { get; set; } = false;
}