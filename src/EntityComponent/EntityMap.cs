//
// ECSMap.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Maps entities, systems and components to one another and
  /// provides querying and updating access to all elements
  /// </summary>
  public class EntityMap
  {
    /// <summary>
    /// The last component mask created for an entity or system
    /// </summary>
    private ulong _lastMask;
    /// <summary>
    /// The next GUID to generate for a new entity
    /// </summary>
    private ulong _nextID;

    /// <summary>
    /// The currently registered EntitySystems
    /// </summary>
    private Dictionary<Type, EntitySystem> _systems;
    /// <summary>
    /// The currently registered IComponent derived types
    /// </summary>
    private Dictionary<Type, ulong> _components;
    /// <summary>
    /// All tags assigned to entities - ensures uniqueness
    /// </summary>
    private Dictionary<string, int> _tags;
    /// <summary>
    /// All created entities
    /// </summary>
    private List<Entity> _entities;

    private Dictionary<string, Action<Entity>> _blueprints;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.EntityMap"/> class.
    /// </summary>
    public EntityMap()
    {
      _lastMask = _nextID = 0;
      _systems = new Dictionary<Type, EntitySystem>();
      _entities = new List<Entity>();
      _components = new Dictionary<Type, ulong>();
      _tags = new Dictionary<string, int>();
      _blueprints = new Dictionary<string, Action<Entity>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.EntityMap"/> class.
    /// Uses an existing EntityMap to copy all registered systems and components as well as any
    /// persistant Entities.
    /// </summary>
    /// <param name="map">EntityMap to copy from</param>
    public EntityMap(EntityMap map)
    {
      _lastMask = map._lastMask;
      _nextID = map.NextID - 1;
      _systems = new Dictionary<Type, EntitySystem>(map._systems);
      _entities = new List<Entity>(map._entities);
      _components = new Dictionary<Type, ulong>(map._components);
      _blueprints = map._blueprints;
      _tags = new Dictionary<string, int>(map._tags);
    }

    /// <summary>
    /// Registers a new component type to the EntityMap
    /// </summary>
    /// <typeparam name="T">Type of component to register</typeparam>
    public void AddComponent<T>() where T : IComponent
    {
      var type = typeof(T);
      if ( !_components.ContainsKey(type) ) {
        // Couldn't get this to fail tests but just in case any bitwise-related bugs
        // I didn't think of crop up, make this increase by a power of 2 instead
        _components.Add(type, NextMask);
      }
    }

    /// <summary>
    /// Registers a new component type to the EntityMap
    /// </summary>
    /// <param name="componentType">Type of component to register</param>
    public void AddComponent(Type componentType)
    {
      if ( typeof(IComponent).IsAssignableFrom(componentType) ) {
        return;
      }

      if ( !_components.ContainsKey(componentType) ) {
        // Couldn't get this to fail tests but 
        // just in case any bitwise-related bugs
        // I didn't think of crop up, make this increase by a power of 2 instead
        _components.Add(componentType, NextMask);
      }
    }

    /// <summary>
    /// Registers a new EntitySystem to the map
    /// </summary>
    /// <typeparam name="T">EntitySystem type to add</typeparam>
    public void AddSystem<T>(params object[] args) where T : EntitySystem
    {
      var sysType = typeof(T);
      if ( !_systems.ContainsKey(sysType) ) {
        var sysT = CreateSystemInstance<T>(args);
        _systems.Add(sysType, sysT);
        var sys = _systems[sysType];
        // Generate a mask for the new system based off its valid components
        // defined in its constructor
        foreach ( var c in sys.ValidComponents ) {
          sys.ID |= NewOrExistingComponentID(c);
        }
      }
    }

    /// <summary>
    /// Adds a created entity to this map
    /// </summary>
    /// <param name="entity">Entity to add</param>
    public void AddEntity(Entity entity)
    {
      UpdateEntityMask(entity);
      // Add the tag if it doesn't exist otherwise
      // don't add the entity - it's a duplicate
      if ( !_tags.ContainsKey(entity.Tag) ) {

        if ( entity.Tag.Length > 0 ) {
          _tags.Add(entity.Tag, _entities.Count);
        }

        _entities.Add(entity);
        UpdateSystems(entity);

      } else {
        Debug.WriteLine("Duplicate Tag '" + entity.Tag + "'");
      }
    }

    /// <summary>
    /// Updates a specific entities component mask. Use after registering
    /// new components or systems.
    /// </summary>
    /// <param name="entity">Entity to update</param>
    public void UpdateEntityMask(Entity entity)
    {
      entity.Mask = 0;
      foreach ( var c in entity.ComponentTypeList ) {
        entity.Mask |= NewOrExistingComponentID(c);
      }
    }

    /// <summary>
    /// Updates each systems associated entity list, adding the specified Entity. Use after
    /// creating a new Entity and adding it manually
    /// </summary>
    /// <param name="entity">Entity to track in each system</param>
    public void UpdateSystems(Entity entity)
    {
      foreach ( var sys in _systems.Values ) {
        // For Strict systems - all bits must be the same
        if ( sys.Association == EntityAssociation.Strict ) {
          if ( (entity.Mask & sys.ID) == sys.ID ) {
            sys.AssociateEntity(entity);
          } else {
            sys.RemoveAssociation(entity);
          }
        }

        // For loose systems, only one of the bits needs to be set
        if ( sys.Association == EntityAssociation.Loose ) {
          if ( (entity.Mask & sys.ID) != 0 ) {
            sys.AssociateEntity(entity);
          } else {
            sys.RemoveAssociation(entity);
          }
        }
      }
    }

    /// <summary>
    /// Creates a new Entity with the given tag in this map. Auto-Registers the entity
    /// with all systems and updates its mask.
    /// </summary>
    /// <returns>The created entity</returns>
    /// <param name="tag">Tagname to give the entity</param>
    public Entity CreateEntity(string tag = "")
    {
      var entity = new Entity(this, tag);
      AddEntity(entity);
      return entity;
    }

    /// <summary>
    /// Gets the id of a specified component type if it exists.
    /// </summary>
    /// <returns>The component id mask.</returns>
    /// <typeparam name="T">Type of component to query for.</typeparam>
    public ulong GetComponentID<T>() where T : IComponent
    {
      ulong result = 0;
      var type = typeof(T);
      if ( _components.ContainsKey(type) ) {
        result = _components[type];
      } else {
        Debug.WriteLine("Invalid Component '{0}'", type);
      }
      return result;
    }

    /// <summary>
    /// Retrieves a pre-registered system from the map
    /// </summary>
    /// <returns>The system if it exists; null otherwise</returns>
    /// <typeparam name="T">Type of system to retrieve.</typeparam>
    public EntitySystem GetSystem<T>() where T : EntitySystem
    {
      EntitySystem result = null;
      var key = typeof(T);
      if ( key.BaseType == typeof(EntitySystem) && _systems.ContainsKey(key) ) {
        result = _systems[key];
      }

      if ( result != null && result.DestroyList.Count > 0 ) {
        // If the system was found, cleanup destroyed entities 
        // before the next system is run
        foreach ( var e in result.DestroyList ) {
          result.AssociatedEntities.Remove(e);
          _entities.Remove(e);
          if ( e.Tag != string.Empty ) {
            _tags.Remove(e.Tag);
          }
        }
      }

      return result;
    }

    public List<Entity> EntitiesWithComponent<T>() where T : IComponent
    {
      return _entities.Where(entity => entity.HasComponent<T>()).ToList();
    }

    /// <summary>
    /// Clears all entities from this map except for
    /// any marked as persistant.
    /// </summary>
    public void Clear()
    {
      // Clear associated entities for all systems
      foreach ( var system in _systems.Values ) {
        system.AssociatedEntities.RemoveAll(entity => !entity.Persistent);
      }
      _tags.Clear();
      // Remove all non-persistant entities
      _entities.RemoveAll(entity => !entity.Persistent);

      // Re-add all persistant entities tags to the map
      for ( int i = 0; i < _entities.Count; i++ ) {
        if ( _entities[i].Tag != string.Empty ) {
          _tags.Add(_entities[i].Tag, i);
        }
        UpdateSystems(_entities[i]);
      }
    }

    public void Reset()
    {
      // Clear associated entities for all systems
      foreach ( var system in _systems.Values ) {
        system.AssociatedEntities.Clear();
      }
      _tags.Clear();
      // Remove all non-persistant entities
      _entities.Clear();
    }

    public void MakeBlueprint(string id, Action<Entity> buildFunction)
    {
      if ( !_blueprints.ContainsKey(id) ) {
        _blueprints.Add(id, buildFunction);
      }
    }

    public void UseBlueprint(string name, Entity entity)
    {
      if ( _blueprints.ContainsKey(name) ) {
        _blueprints[name](entity);
      } else {
        Console.WriteLine("No blueprint with that name exists.");
      }
    }

    /// <summary>
    /// Creates a new instance of an EntitySystem
    /// </summary>
    /// <returns>The system instance.</returns>
    /// <param name="args">The systems constructor args.</param>
    /// <typeparam name="T">Type of system to create.</typeparam>
    private EntitySystem CreateSystemInstance<T>(params object[] args) where T : EntitySystem
    {
      return (T)Activator.CreateInstance(typeof(T), args);
    }

    /// <summary>
    /// Looks up if the component is already registered and adds a new instance of it to the map
    /// if it isn't
    /// </summary>
    /// <returns>The new or existing components ID.</returns>
    /// <param name="component">Component type to query for.</param>
    private ulong NewOrExistingComponentID(Type component)
    {
      ulong result = 0;
      if ( typeof(IComponent).IsAssignableFrom(component) ) {
        if ( _components.ContainsKey(component) ) {
          result = _components[component];
        } else {
          // Couldn't get this to fail tests but just in case any bitwise-related bugs
          // I didn't think of crop up, make this increase by a power of 2 instead
          result = NextMask;
          _components.Add(component, result);
        }
      } else {
        Debug.WriteLine("Invalid Component '{0}'", component);
      }
      return result;
    }

    /// <summary>
    /// Gets the <see cref="T:MB2D.EntityComponent.Entity"/> with the specified tag
    /// if it exists; null otherwise
    /// </summary>
    /// <param name="key">Tagname of the entity to retireve.</param>
    public Entity this[string key]
    {
      get
      {
        Entity result = null;
        if ( _tags.ContainsKey(key) ) {
          var index = _tags[key];
          if ( index < _entities.Count ) {
            result = _entities[index];
          }
        }
        return result;
      }
    }

    /// <summary>
    /// Gets the number of entities in the map.
    /// </summary>
    /// <value>The entity count.</value>
    public int EntityCount
    {
      get { return _entities.Count; }
    }

    /// <summary>
    /// Auto-increments the last generated GUID and retrieves the result
    /// </summary>
    /// <value>The next identifier.</value>
    public ulong NextID
    {
      get { return ++_nextID; }
    }

    /// <summary>
    /// Gets the next valid mask.
    /// </summary>
    /// <value>The next mask.</value>
    private ulong NextMask
    {
      get
      {
        // ensure powers of two
        if ( _lastMask < 1 ) {
          _lastMask = 1;
        } else {
          _lastMask *= 2;
        }
        return _lastMask;
      }
    }
  }
}

