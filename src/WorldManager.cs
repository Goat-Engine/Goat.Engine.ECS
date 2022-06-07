using MoreLinq;

namespace Goat.Engine.ECS;

public static class WorldManager
{
    private static List<World> _worlds = new List<World>();

    public static World CreateWorld()
    {
        World world = new World();
        _worlds.Add(world);
        return world;
    }

    public static bool DestroyWorld(World world)
    {
        return _worlds.Remove(world);
    }

    public static IReadOnlyCollection<World> GetWorlds()
    {
        return _worlds;
    }

    public static void ExecuteUpdateToAllWorlds()
    {
        _worlds.ForEach(w => w.ExecuteUpdate());
    }

    public static void ExecuteUpdate(World world)
    {
        world.ExecuteUpdate();
    }
}