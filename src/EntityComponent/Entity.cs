//
// Entity.cs
// MB2D Engine
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Collections.Generic;

namespace MB2D.EntityComponent
{
  /// <summary>
  /// Represents a tagged and id'd container for components that can be
  /// operated on by systems.
  /// </summary>
  public class Entity
  {
    /// <summary>
    /// Gets and sets the entities component mask.
    /// </summary>
    /// <value>The component mask.</value>
    public ulong Mask { get; set; }

    /// <summary>
    /// Gets and sets the entities Globally Unique ID in the EntityMap
    /// </summary>
    /// <value>GUID</value>
    public ulong ID { get; set; }

    /// <summary>
    /// The entities tag name
    /// </summary>
    private string _tag;
    /// <summary>
    /// The entities parent container
    /// </summary>
    private EntityMap _container;
    /// <summary>
    /// The Components this entity currently has attached
    /// </summary>
    private Dictionary<Type, IComponent> _components;

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:MB2D.EntityComponent.Entity"/> class.
    /// </summary>
    /// <param name="tag">Tag name to give entity</param>
    private Entity(string tag = "")
    {
      _components = new Dictionary<Type, IComponent>();
      _tag = tag;
      Mask = 0;
      Persistent = false;
      Active = true;
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:MB2D.EntityComponent.Entity"/> class.
    /// </summary>
    /// <param name="container">The entities parent EntityMap</param>
    /// <param name="tag">Tagname to give the entity</param>
    public Entity(EntityMap container, string tag = "") : this(tag)
    {
      _container = container;
      ID = container.NextID;
    }

    /// <summary>
    /// Attaches a new component to the entity.
    /// </summary>
    /// <param name="args">The components constructor arguments</param>
    /// <typeparam name="T">Type of component to attach</typeparam>
    public IComponent Attach<T>(params object[] args) where T : IComponent
    {
      var component = NewComponent<T>(args);
      // Add to dict if new component, otherwise just 
      // reassign the current one to be the new one
      if ( !_components.ContainsKey(typeof(T)) ) {
        _components.Add(typeof(T), component);
        _container.UpdateEntityMask(this);
        _container.UpdateSystems(this);
      } else {
        component = _components[typeof(T)];
      }
      return component;
    }

    /// <summary>
    /// Attaches a new component to the entity.
    /// </summary>
    /// <param name="component">Pre constructed component to add</param>
    public void Attach(IComponent component)
    {
      _components.Add(component.GetType(), component);
      _container.UpdateEntityMask(this);
      _container.UpdateSystems(this);
    }

    /// <summary>
    /// Detatches a specific component from the entity
    /// </summary>
    /// <typeparam name="T">The type of component to detatch.</typeparam>
    public void Detach<T>() where T : IComponent
    {
      if ( _components.ContainsKey(typeof(T)) ) {
        _components.Remove(typeof(T));
        _container.UpdateEntityMask(this);
        _container.UpdateSystems(this);
      }
    }

    /// <summary>
    /// Detachs all of the entities attached components.
    /// </summary>
    public void DetachAll()
    {
      _components.Clear();
      _container.UpdateEntityMask(this);
      _container.UpdateSystems(this);
    }

    /// <summary>
    /// Queries the entity to see if it has a component attached and returns it if it does
    /// </summary>
    /// <returns>The component if the entity has it attached, null otherwise</returns>
    /// <typeparam name="T">Component to query the entity for.</typeparam>
    public T GetComponent<T>() where T : IComponent
    {
      var cType = typeof(T);
      if ( _components.ContainsKey(cType) ) {
        return (T)_components[cType];
      } else {
        return default(T);
      }
    }

    /// <summary>
    /// Checks if an entity has a specific component attached
    /// </summary>
    /// <returns><c>true</c>, if component is attached, <c>false</c> otherwise.</returns>
    /// <typeparam name="T">The type of component to check.</typeparam>
    public bool HasComponent<T>() where T : IComponent
    {
      return _components.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Creates a new instance of an object derived from IComponent
    /// </summary>
    /// <returns>The new component instance</returns>
    /// <param name="args">Constructor arguments for the component type</param>
    /// <typeparam name="T">Type of component to create</typeparam>
    private IComponent NewComponent<T>(params object[] args) where T : IComponent
    {
      return (T)Activator.CreateInstance(typeof(T), args);
    }

    /// <summary>
    /// Gets this entities tagname
    /// </summary>
    /// <value>The tagname</value>
    public string Tag
    {
      get { return _tag; }
    }

    /// <summary>
    /// Gets the list if components attached to this entity.
    /// </summary>
    /// <value>The component list.</value>
    public Dictionary<Type, IComponent>.ValueCollection ComponentList
    {
      get { return _components.Values; }
    }

    /// <summary>
    /// Gets the types of components this entity has attached
    /// </summary>
    /// <value>The component type list.</value>
    public Dictionary<Type, IComponent>.KeyCollection ComponentTypeList
    {
      get { return _components.Keys; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:MB2D.EntityComponent.Entity"/> is persistant in its
    /// parent <see cref="T:MB2D.EntityComponent.EntityMap"/>.
    /// </summary>
    /// <value><c>true</c> if persistant; otherwise, <c>false</c>.</value>
    public bool Persistent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:MB2D.EntityComponent.Entity"/> is active.
    /// Inactive entities are skipped over in each EntitySystems Process() method but aren't destroyed. Allowing semi-persistant
    /// entities.
    /// </summary>
    /// <value><c>true</c> if the entity is active; otherwise, <c>false</c>.</value>
    public bool Active { get; set; }
  }

}

