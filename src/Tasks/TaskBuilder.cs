using MoreLinq;
using Goat.Engine.ECS.Query;

namespace Goat.Engine.ECS.Tasks;

public static class TaskBuilder
{
    public static TaskChain BuildChain(List<QueryBuilder> queryBuilders, List<Component> components)
    {
        TaskChain taskChain = new TaskChain();

        IEnumerable<QueryBuilder>[] queryBuilderPartitions = new[]
        {
            queryBuilders.Where(builder => builder.ExecutionOrder == ExecutionOrder.EarlyUpdate),
            queryBuilders.Where(builder => builder.ExecutionOrder == ExecutionOrder.Update),
            queryBuilders.Where(builder => builder.ExecutionOrder == ExecutionOrder.LateUpdate),
        };

        foreach (var partition in queryBuilderPartitions)
        {
            foreach (var queryBuilder in partition.OrderBy(builder => builder.ComponentTypes.Length))
            {
                foreach (TaskItem taskItem in BuildTaskItems(queryBuilder, components))
                {
                    TaskItem? parent = taskChain.FindParentOfType(taskItem.ComponentType);

                    if (parent != null)
                    {
                        taskItem.Dependencies.Add(parent);
                        parent.Children.Add(taskItem);
                    }
                    else
                    {
                        taskChain.RootTaskItems.Add(taskItem);
                    }
                    taskChain.AllTaskItems.Add(taskItem);
                }
            }
        }

        return taskChain;
    }

    private static TaskItem[] BuildTaskItems(QueryBuilder queryBuilder, List<Component> components)
    {
        return queryBuilder.ComponentTypes.Length switch
        {
            1 => BuildTaskForOneComponent(queryBuilder, components),
            2 => BuildTaskForTwoComponent(queryBuilder, components),
            _ => throw new NotImplementedException()
        };
    }

    private static TaskItem[] BuildTaskForOneComponent(QueryBuilder queryBuilder, List<Component> components)
    {
        IEnumerable<Component> filteredComponents = components.Where(c => c.GetType() == queryBuilder.ComponentTypes[0]);
        return new TaskItem[]
        {
            new SingleComponentTaskItem(queryBuilder.Action, filteredComponents, queryBuilder.ComponentTypes[0])
        };
    }

    private static TaskItem[] BuildTaskForTwoComponent(QueryBuilder queryBuilder, List<Component> components)
    {
        IEnumerable<Component> lhComponents = components.Where(c => c.GetType() == queryBuilder.ComponentTypes[0]);
        IEnumerable<Component> rhComponents = components.Where(c => c.GetType() == queryBuilder.ComponentTypes[1]);
        List<(Component, Component)> filteredComponents =
            lhComponents.Cartesian(rhComponents, (c1, c2) => (c1, c2)).ToList();

        DoubleComponentTaskItem item1 =
            new DoubleComponentTaskItem(queryBuilder.Action2, filteredComponents, queryBuilder.ComponentTypes[0]);
        DoubleComponentTaskItem item2 =
            new DoubleComponentTaskItem(queryBuilder.Action2, filteredComponents, queryBuilder.ComponentTypes[1]);

        item1.OtherRelatedTaskItems.Add(item2);
        item2.OtherRelatedTaskItems.Add(item1);

        return new TaskItem[]
        {
            item1, item2
        };
    }
}