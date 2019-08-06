namespace QF.GraphDesigner
{
    public class TaskProgress
    {
        public TaskProgress(string message, float percentage)
        {
            Message = message;
            Percentage = percentage;
        }

        public TaskProgress(float percentage, string message)
        {
            Percentage = percentage;
            Message = message;
        }

        public string Message { get; set; }
        public float Percentage { get; set; }
    }
}