namespace GoLive.Saturn.Auditing
{
    public class ChangeLog
    {
        public string Name { get; set; }
        public dynamic Previous { get; set; }
        public dynamic Current { get; set; }
    }
}