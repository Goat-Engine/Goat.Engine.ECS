using Goat.Engine.ECS.Query;
using Goat.Engine.ECS.Tasks;

namespace Goat.Engine.ECS;

public class World
{
    private readonly List<Entity> _entities = new List<Entity>();
    private readonly List<Component> _components = new List<Component>();
    private readonly List<QueryBuilder> _queryBuilders = new List<QueryBuilder>();
    private TaskChain? _taskChain;

    public void AddQuery(QueryBuilder queryBuilder)
    {
        _queryBuilders.Add(queryBuilder);
        _taskChain = null;
    }

    public bool RemoveQuery(QueryBuilder queryBuilder)
    {
        _taskChain = null;
        return _queryBuilders.Remove(queryBuilder);
    }

    public Entity CreateEntity()
    {
        Entity entity = new Entity(this);
        _entities.Add(entity);
        return entity;
    }

    public bool DestroyEntity(Entity entity)
    {
        return _entities.Remove(entity);
    }

    internal void ExecuteUpdate()
    {
        _taskChain ??= TaskBuilder.BuildChain(_queryBuilders, _components);
        _taskChain.GetParallelTask().Wait();
        _taskChain.ResetTasks();
    }

    internal void AddComponent(Component component)
    {
        _components.Add(component);
        _taskChain = null;
    }

    internal bool RemoveComponent(Component component)
    {
        _taskChain = null;
        return _components.Remove(component);
    }
}