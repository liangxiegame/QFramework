namespace QFramework.Example
{
    public class TaskFactory
    {
        public static TimeTickTask Create(int type)
        {
            if (type == TimeTickTask.TypeA)
            {
                return new ATypeTask();
            }
            else if (type == TimeTickTask.TypeB)
            {
                return new BTypeTask();
            }
            else if (type == TimeTickTask.TypeC)
            {
                return new BTypeTask();
            }

            return null;
        }
    }
}