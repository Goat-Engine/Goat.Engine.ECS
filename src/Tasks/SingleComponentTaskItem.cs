namespace Goat.Engine.ECS.Tasks;

public class SingleComponentTaskItem : TaskItem
{
    private readonly Action<IEnumerable<Component>> _action;
    private readonly IEnumerable<Component> _filteredComponents;

    public SingleComponentTaskItem(Action<IEnumerable<Component>> action, IEnumerable<Component> filteredComponents, Type componentType) : base(componentType)
    {
        _action = action;
        _filteredComponents = filteredComponents;
    }

    public override Task? GetActionTask()
    {
        if (ActionTask == null)
        {
            ActionTask = Task.Run(() => _action.Invoke(_filteredComponents));
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
        }
        return SchedulingTask;
    }
}