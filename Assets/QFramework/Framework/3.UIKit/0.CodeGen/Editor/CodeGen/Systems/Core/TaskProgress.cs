namespace QFramework.CodeGen
{
    public class TaskProgress
    {
        public TaskProgress(string message, float percentage)
        {
            Message = message;
            Percentage = percentage;
        }
        

        public string Message { get; set; }
        public float Percentage { get; set; }
    }
}