namespace Goat.Engine.ECS.Tasks;

public class TaskChain
{
    public List<TaskItem> RootTaskItems { get; } = new List<TaskItem>();
    public List<TaskItem> AllTaskItems { get; } = new List<TaskItem>();

    public TaskItem? FindParentOfType(Type type)
    {
        return RootTaskItems.Find(i => i.ComponentType == type)?.FindChildrenOfType(type);
    }

    public Task GetParallelTask()
    {
        // Prepare action tasks.
        foreach (TaskItem item in AllTaskItems)
        {
            item.GetActionTask();
        }

        return Task.Run(() =>
        {
            Parallel.ForEach(RootTaskItems, (item, state) =>
            {
                item.GetSchedulingTask().Wait();
            });
        });
    }

    public void ResetTasks()
    {
        foreach (TaskItem item in AllTaskItems)
        {
            item.ResetTask();
        }
    }
}