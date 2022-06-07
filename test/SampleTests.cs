using System.Numerics;
using Goat.Engine.ECS.Query;

namespace Goat.Engine.ECS.Test;

public class SampleTests
{
    private class ChildComponent : Component
    {
        public Vector3 Position { get; set; }

        public static void OnUpdate(IEnumerable<Component> components)
        {
            Console.WriteLine($"Called from child component. Found Number of items: {components.Count()}");
        }
    }

    private class OtherComponent : Component
    {
        public Vector2 Position { get; set; }

        public static void OnUpdate(IEnumerable<Component> components)
        {
            Console.WriteLine($"Called from other component. Found Number of items: {components.Count()}");
        }

        public static void OnUpdateShared(IEnumerable<(Component, Component)> components)
        {
            Console.WriteLine($"Called from shared component. Found Number of items: {components.Count()}");
        }
    }

    [Test]
    public void TestEverything()
    {
        World world = WorldManager.CreateWorld();
        Entity entity1 = world.CreateEntity();
        entity1.AddComponent<ChildComponent>();
        entity1.AddComponent<OtherComponent>();

        Entity entity2 = world.CreateEntity();
        entity2.AddComponent<ChildComponent>();

        QueryBuilder builder1 = new QueryBuilder()
            .System<ChildComponent>(ChildComponent.OnUpdate);
        world.AddQuery(builder1);

        QueryBuilder builder2 = new QueryBuilder()
            .System<OtherComponent>(OtherComponent.OnUpdate);
        world.AddQuery(builder2);

        QueryBuilder builder3 = new QueryBuilder()
            .System<ChildComponent, OtherComponent>(OtherComponent.OnUpdateShared);
        world.AddQuery(builder3);

        for (int i = 0; i < 5; ++i)
        {
            WorldManager.ExecuteUpdateToAllWorlds();
        }
    }
}