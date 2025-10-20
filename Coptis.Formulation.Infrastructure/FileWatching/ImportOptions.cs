namespace Coptis.Formulation.Infrastructure.FileWatching;

public sealed class ImportOptions
{
    public bool Enabled { get; set; } = true;
    public string WatchFolder { get; set; } = "";
    public string SuccessFolder { get; set; } = "";
    public string ErrorFolder { get; set; } = "";
    public string SearchPattern { get; set; } = "*.json";
    public int PollSeconds { get; set; } = 2;
}
