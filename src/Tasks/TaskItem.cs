namespace Goat.Engine.ECS.Tasks;

public abstract class TaskItem
{
    public readonly Type ComponentType;

    public List<TaskItem> Dependencies { get; } = new List<TaskItem>();
    public List<TaskItem> Children { get; } = new List<TaskItem>();

    protected Task? ActionTask { get; set; }
    protected Task? SchedulingTask { get; set; }

    protected TaskItem(Type componentType)
    {
        ComponentType = componentType;
    }

    public TaskItem? FindChildrenOfType(Type type)
    {
        return Children.Find(i => i.ComponentType == type) ?? this;
    }

    public abstract Task? GetActionTask();

    public abstract Task? GetSchedulingTask();

    public void ResetTask()
    {
        ActionTask = null;
        SchedulingTask = null;
    }
}