namespace Task_Management_API.Entities
{
    public class AppSettings
    {
        public string ApplicationName { get; set; } = string.Empty;
        public int MaxTasks { get; set; }
        public bool IsFeatureXEnabled { get; set; }
    }
}
