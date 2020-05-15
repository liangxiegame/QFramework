namespace QFramework
{
    public abstract class Request
    {
        public string Type { get; set; }
        public abstract bool Finished { get; }
        
        public abstract void Execute();
    }
}