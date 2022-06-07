namespace Goat.Engine.ECS.Query;

public class QueryBuilder
{
    public Type[] ComponentTypes { get; private set; }
    public Action<IEnumerable<Component>> Action { get; private set; }
    public Action<IEnumerable<(Component, Component)>> Action2 { get; private set; }
    public ExecutionOrder ExecutionOrder { get; private set; }

    public QueryBuilder()
    {
        Action = components => throw new NullReferenceException($"{nameof(QueryBuilder)}.{nameof(QueryBuilder.Action)} not defined.");
        Action2 = components => throw new NullReferenceException($"{nameof(QueryBuilder)}.{nameof(QueryBuilder.Action2)} not defined.");
    }

    public QueryBuilder System<TComponent>(Action<IEnumerable<Component>> action, ExecutionOrder executionOrder = ExecutionOrder.Update)
        where TComponent : Component
    {
        ComponentTypes = new[] { typeof(TComponent) };
        Action = action;
        ExecutionOrder = executionOrder;
        return this;
    }

    public QueryBuilder System<TComponent1, TComponent2>(Action<IEnumerable<(Component, Component)>> action, ExecutionOrder executionOrder = ExecutionOrder.Update)
        where TComponent1 : Component
        where TComponent2 : Component
    {
        ComponentTypes = new[] { typeof(TComponent1), typeof(TComponent2) };
        Action2 = action;
        ExecutionOrder = executionOrder;
        return this;
    }
}