//
// ECSystem.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MB2D.EntityComponent
{

  public enum EntityAssociation { Strict, Loose }

  /// <summary>
  /// Performs logic on an entity.
  /// </summary>
  public abstract class EntitySystem
  {
    /// <summary>
    /// The entities this system should know about
    /// </summary>
    private List<Entity> _entities;
    /// <summary>
    /// The valid components used in this system
    /// </summary>
    private List<Type> _components;

    /// <summary>
    /// The list of entities to destroy next frame
    /// </summary>
    private List<Entity> _toDestroy;

    /// <summary>
    /// All GUID's of entities this system knows about
    /// </summary>
    protected Dictionary<ulong, Entity> _idEntityMap;

    /// <summary>
    /// Defines if this system should be Loose or Strict
    /// </summary>
    protected EntityAssociation _associationType;

    /// <summary>
    /// Used for timing runtime of a system. Used for debugging
    /// </summary>
    private Stopwatch _timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.EntitySystem"/> class.
    /// Checks if the passed in types are valid components and if not, all components are deregistered
    /// leaving a system that knows about nothing and can't operate on any entities.
    /// </summary>
    /// <param name="components">Components this system is interested in</param>
    public EntitySystem(params Type[] components)
    {
      _entities = new List<Entity>();
      _toDestroy = new List<Entity>();
      _components = new List<Type>();
      _idEntityMap = new Dictionary<ulong, Entity>();
      _associationType = EntityAssociation.Strict;

      ID = 0;

      // Check if entities components are valid
      foreach ( var c in components ) {
        if ( !(typeof(IComponent).IsAssignableFrom(c)) ) {
          Console.WriteLine("MB2D Engine: '{0}' is not a valid component.", c);
          _components.Clear();
          break;
        } else {
          _components.Add(c);
        }
      }
    }

    /// <summary>
    /// Runs this systems Process() method on all entities
    /// </summary>
    public virtual void Run(params object[] args)
    {
      if ( _toDestroy.Count > 0 ) {
        _toDestroy.Clear();
      }

      PreProcess(); // execute logic to setup data before processing all entities
      ProcessingLoop(); // process entities
      PostProcess(); // cleanup
    }

    /// <summary>
    /// Executes Process() on all AssociatedEntities.
    /// </summary>
    protected virtual void ProcessingLoop()
    {
      var entityCount = _entities.Count;
      for ( int i = 0; i < entityCount; i++ ) {
        if ( _entities[i].Active ) {
          Process(_entities[i]);
        }
      }
    }

    /// <summary>
    /// Adds the specific entity to the destroy list to be cleaned up the
    /// next time a system is run
    /// </summary>
    /// <param name="entity">Entity to destroy.</param>
#if TESTING
    public void Destroy(Entity entity)
#else
    public void Destroy(Entity entity)
#endif
    {
      _toDestroy.Add(entity);
      _idEntityMap.Remove(entity.ID);
    }

    /// <summary>
    /// Associates a new entity with this system.
    /// </summary>
    /// <param name="entity">Entity to associate</param>
    public void AssociateEntity(Entity entity)
    {
      if ( !_idEntityMap.ContainsKey(entity.ID) ) {
        _entities.Add(entity);
        _idEntityMap.Add(entity.ID, entity);
      }
      PostAssociate(entity);
    }

    /// <summary>
    /// Decouples an association with this system
    /// </summary>
    /// <param name="entity">Entity to decouple.</param>
    public void RemoveAssociation(Entity entity)
    {
      _entities.Remove(entity);
      _idEntityMap.Remove(entity.ID);
    }

    /// <summary>
    /// Used to setup any data needed before processing all entities, such as sorting a list ahead
    /// of time 
    /// </summary>
    protected virtual void PreProcess() { }

    /// <summary>
    /// Used to cleanup or execute any teardown logic needed
    /// </summary>
    protected virtual void PostProcess() { }

    /// <summary>
    /// Used to define any logic or extra data needed after an entity is associated with the system.
    /// </summary>
    /// <param name="entity">Entity.</param>
    protected virtual void PostAssociate(Entity entity) { }

    /// <summary>
    /// Executes this systems logic on a single entity
    /// </summary>
    /// <param name="entity">Entity to operate on</param>
    protected abstract void Process(Entity entity);

    /// <summary>
    /// Gets or sets the associated entities list.
    /// </summary>
    /// <value>The associated entities.</value>
    public List<Entity> AssociatedEntities
    {
      get { return _entities; }

      set
      {
        _entities = value;
      }
    }

    /// <summary>
    /// Gets the list of components this system is interested in
    /// </summary>
    /// <value>The valid components.</value>
    public List<Type> ValidComponents
    {
      get { return _components; }
    }

    /// <summary>
    /// Gets or sets this systems ID mask
    /// </summary>
    /// <value>The identifier mask.</value>
    public ulong ID { get; set; }

    /// <summary>
    /// Gets the destroy list.
    /// </summary>
    /// <value>The destroy list.</value>
    public List<Entity> DestroyList
    {
      get { return _toDestroy; }
    }

    /// <summary>
    /// Gets the systems association level.
    /// </summary>
    /// <value>The association level.</value>
    public EntityAssociation Association
    {
      get { return _associationType; }
    }
  }
}

