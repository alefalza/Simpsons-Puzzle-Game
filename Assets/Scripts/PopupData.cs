namespace Core.Services.PopupService
{
    public enum Priority
    {
        Urgent = 4,
        High = 3,
        Medium = 2,
        Low = 1
    }
    
    public abstract class PopupData
    {
        public Priority Priority { get; private set; }

        protected PopupData(Priority priority)
        {
            Priority = priority;
        }
    }
}
