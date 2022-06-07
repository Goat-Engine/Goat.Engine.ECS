namespace Goat.Engine.ECS;

public class Entity
{
    internal readonly List<Component> Components = new List<Component>();

    public readonly uint Id = (uint)Random.Shared.NextInt64(uint.MinValue, uint.MaxValue);
    public readonly World World;

    internal Entity(World world)
    {
        World = world;
    }

    public TComponent AddComponent<TComponent>()
        where TComponent : Component, new()
    {
        TComponent component = Activator.CreateInstance<TComponent>();
        component.Entity = this;

        Components.Add(component);
        World.AddComponent(component);

        return component;
    }

    public bool RemoveComponent<TComponent>()
        where TComponent : Component
    {
        Component? component = Components.Find(c => c.GetType() == typeof(TComponent));
        if (component != null)
        {
            return Components.Remove(component) && World.RemoveComponent(component);
        }

        return false;
    }
}