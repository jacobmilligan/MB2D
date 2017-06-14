//
// 	CollisionSystem.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MB2D.Collision;
using MB2D.Tiles;
using MonoGame.Extended.Shapes;
using MonoGame.Extended;

namespace MB2D.EntityComponent
{
	/// <summary>
	/// Checks collisions. Uses a spatial indexing grid for broad-phase collision checking
	/// and AABB checks for narrow phase
	/// </summary>
	public class CollisionSystem : EntitySystem
	{
		/// <summary>
		/// The current collision map
		/// </summary>
		private CollisionMap _map;

		/// <summary>
		/// The current tile map, used for checking terrain collisions
		/// </summary>
		private TileMap _tileMap;

		/// <summary>
		/// The number of comparisons made in the last frame. Used for debugging
		/// </summary>
		private int _comparisons;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.CollisionSystem"/> class.
		/// </summary>
		public CollisionSystem() : base(typeof(CollisionComponent))
		{
			ResetGrid(0, 0, 0, 0, 1);
		}

		/// <summary>
		/// Resets the grid position in the world.
		/// </summary>
		/// <param name="xMin">The grids left most x coordinate.</param>
		/// <param name="xMax">Right most x coordinae.</param>
		/// <param name="yMin">Top most y coordinate.</param>
		/// <param name="yMax">Bottom most y coordinate.</param>
		/// <param name="cellSize">The size of each cell in the grid.</param>
		public void ResetGrid(int xMin, int xMax, int yMin, int yMax, int cellSize)
		{
			_map = new CollisionMap(xMin, xMax, yMin, yMax, cellSize);
		}

		/// <summary>
		/// Clears the collision grid and inserts all entities before checking collisions
		/// </summary>
		protected override void PreProcess()
		{
			_comparisons = 0;

			_map.Clear();
			var maxEntities = AssociatedEntities.Count;

			for ( int e = 0; e < maxEntities; e++ )
			{

				// Insert all entities with collision components
				if ( AssociatedEntities[e].HasComponent<CollisionComponent>() )
				{

					var collision = AssociatedEntities[e].GetComponent<CollisionComponent>();
					collision.Event = false;
					collision.Collider = null;
					_map.Insert(AssociatedEntities[e], collision);

				}
			}
		}

		/// <summary>
		/// Override. Only processes entities with movement components.
		/// Still considers static entities, but only as possible neighbours.
		/// </summary>
		protected override void ProcessingLoop()
		{
			var entityCount = AssociatedEntities.Count;
			for ( int e = 0; e < entityCount; e++ )
			{
				var entity = AssociatedEntities[e];
				if ( entity.HasComponent<Movement>() )
				{
					Process(entity);
				}
			}
		}

		/// <summary>
		/// Checks all collisions within the entities known collision cells
		/// </summary>
		/// <param name="entity">Entity to check.</param>
		protected override void Process(Entity entity)
		{
			var collision = entity.GetComponent<CollisionComponent>();
			var sprite = entity.GetComponent<SpriteTransform>();

			var center = Vector2.Zero;

			if ( sprite != null )
			{
				center = sprite.Target.Origin;
			}

			if ( collision != null )
			{
				var neighbours = _map.GetCollisions(entity, collision);

				var boxCount = collision.Boxes.Count;

				// Check collisions for all of the entities AABB's for all neighbours
				for ( int b = 0; b < boxCount; b++ )
				{
					var box = collision.Boxes[b];

					// Move the box to align with the entities new position
					if ( sprite != null )
					{
						box.Width += sprite.Bounds.Width - box.Width;
						box.Height += sprite.Bounds.Height - box.Height;
						box.X += sprite.Target.Position.X - box.X - (sprite.Bounds.Width / 2);
						box.Y += sprite.Target.Position.Y - box.Y - (sprite.Bounds.Height / 2);

						collision.Boxes[b] = box;
					}

					// Do narrow phase checking for each neighbour
					foreach ( var n in neighbours )
					{
						var neighbourCollision = n.GetComponent<CollisionComponent>();
						collision.Event = HandleCollisions(box, neighbourCollision);

						if ( collision.Event )
						{
							// Update collision information for other systems' use
							collision.Collider = n;
							neighbourCollision.Event = true;
							neighbourCollision.Collider = entity;
							break;
						}

					}

					// Do tilemap collision checking
					if ( _tileMap != null )
					{
						var physics = entity.GetComponent<PhysicsComponent>();
						var movement = entity.GetComponent<Movement>();
						if ( physics != null && movement != null )
						{

							// Get all tiles surrounding the entity
							var top = (int)Math.Floor(box.Top / _tileMap.TileSize.Y);
							var left = (int)Math.Floor(box.Left / _tileMap.TileSize.X);
							var right = Math.Ceiling(box.Right / _tileMap.TileSize.X) - 1;
							var bottom = Math.Ceiling(box.Bottom / _tileMap.TileSize.X) - 1;

							int xSide = 1;
							int ySide = 1;

							// Check all the tiles around the entity, updating the negative velocity
							// of their movement based on the direction a collision was found
							for ( int x = left; x <= right; x++ )
							{
								for ( int y = top; y <= bottom; y++ )
								{

									var tileAABB = new Rectangle(
									  x * _tileMap.TileSize.X,
									  y * _tileMap.TileSize.Y,
									  _tileMap.TileSize.X,
									  _tileMap.TileSize.Y
									);

									// Check collision for that tile and alter velocity if collision was found
									if ( _tileMap[x, y].Flag == TileFlag.Impassable && box.Intersects(tileAABB) )
									{
										physics.Velocity = new Vector2(xSide * 5, ySide * 5);
										movement.Position = movement.LastPosition;
									}

									ySide -= 2;
								}

								xSide -= 2;
							}
						}
					}
				}

			}
		}

		/// <summary>
		/// Handles narrow-phase collisions using basic seperating axis
		/// </summary>
		/// <returns><c>true</c>, if a collision ocurred, <c>false</c> otherwise.</returns>
		/// <param name="box">Collision box.</param>
		/// <param name="neighbourCollision">Neighbours collision boxes to check against.</param>
		private bool HandleCollisions(RectangleF box, CollisionComponent neighbourCollision)
		{
			var hasCollision = false;
			var neighbourBoxes = neighbourCollision.Boxes;
			for ( int n = 0; n < neighbourBoxes.Count; n++ )
			{
				if ( box.Intersects(neighbourBoxes[n]) )
				{
					hasCollision = true;
				}
				_comparisons++;
			}
			return hasCollision;
		}

		/// <summary>
		/// Sets the current tile map to check for collisions
		/// </summary>
		/// <param name="tileMap">Tile map.</param>
		public void SetTileMap(TileMap tileMap)
		{
			_tileMap = tileMap;
		}

		/// <summary>
		/// Gets the current collision map.
		/// </summary>
		/// <value>The current map.</value>
		public CollisionMap CurrentMap
		{
			get { return _map; }
		}

		/// <summary>
		/// Gets the number of collision checks made last frame. Used for debugging.
		/// </summary>
		/// <value>The number of collision checks.</value>
		public int NumberOfChecks
		{
			get { return _comparisons; }
		}


	}
}
