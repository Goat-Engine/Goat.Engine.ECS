namespace Goat.Engine.ECS;

public abstract class Component
{
    public Entity Entity { get; internal set; }
}