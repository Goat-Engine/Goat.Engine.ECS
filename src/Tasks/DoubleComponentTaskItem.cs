namespace Goat.Engine.ECS.Tasks;

public class DoubleComponentTaskItem : TaskItem
{
    private readonly Action<IEnumerable<(Component, Component)>> _action;
    private readonly IEnumerable<(Component, Component)> _filteredComponents;

    public List<DoubleComponentTaskItem> OtherRelatedTaskItems { get; } = new List<DoubleComponentTaskItem>();

    public DoubleComponentTaskItem(Action<IEnumerable<(Component, Component)>> action, IEnumerable<(Component, Component)> filteredComponents, Type componentType) : base(componentType)
    {
        _action = action;
        _filteredComponents = filteredComponents;
    }

    public override Task? GetActionTask()
    {
        if (ActionTask == null)
        {
            ActionTask = Task.Run(() => _action.Invoke(_filteredComponents));
            OtherRelatedTaskItems.ForEach(item => item.ActionTask = ActionTask);
        }

        return ActionTask;
    }

    public override Task? GetSchedulingTask()
    {
        if (SchedulingTask == null)
        {
            Task[] dependencyTasks = Dependencies.Select(d => d.GetActionTask()).ToArray()!;
            Task[] childrenTasks = Children.Select(c => c.GetActionTask()).ToArray()!;

            SchedulingTask = Task.WhenAll(dependencyTasks).ContinueWith(_ =>
            {
                ActionTask!.ContinueWith(_ =>
                {
                    if (childrenTasks is { Length: > 0 })
                    {
                        Task.WaitAll(childrenTasks);
                    }
                }).Wait();
            });
            OtherRelatedTaskItems.ForEach(item => item.SchedulingTask = SchedulingTask);
        }

        return SchedulingTask;
    }
}