# Goat.Engine.ECS

Entity Component System with smart, race condition free scheduling system.

## Features

- Uses multiple threads of CoreCLR ThreadPool.
- Race condition free.
- Smart scheduling achieved through query builders.
- Joining multiple components possible.

## Usage

### Basic example:

```csharp
// Create new world
World world = WorldManager.CreateWorld();
// Create entity
Entity entity = world.CreateEntity();
// Add component to entity
entity.AddComponent<ChildComponent>();

// Query builder used for smart scheduling
QueryBuilder queryBuilder = new QueryBuilder()
    .System<ChildComponent>(ChildComponent.OnUpdate);
world.AddQuery(queryBuilder);

public class ChildComponent : Component {
    public static void OnUpdate(IEnumerable<Component> components)
    {
        foreach (var component = components.Cast<ChildComponent>())
        {
            // do something
        }
    }
}
```

### Advanced example:

```csharp
World world = WorldManager.CreateWorld();
Entity entity1 = world.CreateEntity();
entity1.AddComponent<ChildComponent>();
entity1.AddComponent<OtherComponent>();

Entity entity2 = world.CreateEntity();
entity2.AddComponent<ChildComponent>();

// Order of generics type in system is important!
QueryBuilder queryBuilder = new QueryBuilder()
    .System<ChildComponent, OtherComponent>(ChildComponent.OnUpdate);
world.AddQuery(queryBuilder);

public class ChildComponent : Component {
    // nothing
}

public class OtherComponent : Component {
    public static void OnUpdate(IEnumerable<(Component, Component)> components)
    {
        // Same order as defined in system of query builder.
        foreach ((OtherComponent otherComponent, ChildComponent childComponent) component = components.Cast<(OtherComponent, ChildComponent)>())
        {
            // Just entity1 can be found/joined here, since entity2 has not defined a OtherComponent type.
        }
    }
}
```
