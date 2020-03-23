namespace SM.Expressions
{
    public class SMIdentifier
    {
        public string Id { get; }
        public string Application { get; }
        public bool IsNoLog { get; set; }

        public SMIdentifier(string id, string application, bool isNoLog)
        {
            Id = id;
            Application = application;
            IsNoLog = isNoLog;
        }
    }
}