//
// 	CollisionMap.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MB2D.EntityComponent;
using MB2D.Geometry;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace MB2D.Collision
{
	/// <summary>
	/// A 2D Grid that represents a particular space in the game world to check for collisions.
	/// Uses spatial indexing to determine where an entity will be located at any given time.
	/// For best results, the cellsize and overall size of the map should be tweaked for each
	/// individual game screen and environment.
	/// </summary>
	public class CollisionMap
	{
		/// <summary>
		/// The grid of cells containing all known entities
		/// </summary>
		private CollisionCell[,] _cells;

		/// <summary>
		/// List of Entity ID's checked for collisions this iteration.
		/// </summary>
		private LinkedList<ulong> _checkedIDs;

		/// <summary>
		/// All non-empty cells since the last insert operation. Improves performance by 
		/// limiting the number of clear operations called as only cells with 
		/// entities in them will have their Clear() method called, rather than all the cells.
		/// </summary>
		private List<CollisionCell> _nonEmptyCells;

		/// <summary>
		/// The size of each cell (width and height).
		/// </summary>
		private int _cellSize;

		/// <summary>
		/// The geometric grid structure used for debugging purposes
		/// </summary>
		private Grid _grid;

		/// <summary>
		/// The minimum x and y coordinates in the grid
		/// </summary>
		private Vector2 _min;

		/// <summary>
		/// The maximum x and y coordinates to use in the grid
		/// </summary>
		private Vector2 _max;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MB2D.Collision.CollisionMap"/> class.
		/// </summary>
		/// <param name="xMin">The grids left most x coordinate.</param>
		/// <param name="xMax">Right most x coordinae.</param>
		/// <param name="yMin">Top most y coordinate.</param>
		/// <param name="yMax">Bottom most y coordinate.</param>
		/// <param name="cellSize">The size of each cell in the grid.</param>
		public CollisionMap(int xMin, int xMax, int yMin, int yMax, int cellSize)
		{
			var width = (xMax - xMin) / cellSize;
			var height = (yMax - yMin) / cellSize;
			if ( width <= 0 )
			{
				width = 1;
			}
			if ( height <= 0 )
			{
				height = 1;
			}
			_cells = new CollisionCell[width, height];
			_grid = new Grid(height, width, cellSize, cellSize);

			_min = new Vector2(xMin, yMin);
			_max = new Vector2(xMax, yMax);

			_cellSize = cellSize;

			_checkedIDs = new LinkedList<ulong>();
			_nonEmptyCells = new List<CollisionCell>();
		}

		/// <summary>
		/// Indexes a world-based coordinate into the collision grid
		/// </summary>
		/// <returns>The grid-based position.</returns>
		/// <param name="position">World-based position to index.</param>
		public Point IndexOf(Point position)
		{
			var pos = MBMath.WrapGrid(
			  position.X / _cellSize,
			  position.Y / _cellSize,
			  _grid.ColCount, _grid.RowCount
			);
			return new Point
			{
				X = pos.X,
				Y = pos.Y
			};
		}

		/// <summary>
		/// Checks if a particular index exists in the grid
		/// </summary>
		/// <returns><c>true</c>, if index exists, <c>false</c> otherwise.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool IndexExists(int x, int y)
		{
			var result = false;
			var xSize = _cells.GetLength(0);
			var ySize = _cells.GetLength(1);
			if ( x >= 0 && x < xSize && y >= 0 && y < ySize )
			{
				result = true;
			}
			return result;
		}

		/// <summary>
		/// Checks if a particular index exists in the grid
		/// </summary>
		/// <returns><c>true</c>, if index exists, <c>false</c> otherwise.</returns>
		/// <param name="index">Index to check.</param>
		public bool IndexExists(Point index)
		{
			return IndexExists(index.X, index.Y);
		}

		/// <summary>
		/// Inserts an entity and its associated collision component into the grid
		/// </summary>
		/// <param name="entity">Entity to insert.</param>
		/// <param name="collision">The entities collision component.</param>
		public void Insert(Entity entity, CollisionComponent collision)
		{
			var boxes = collision.Boxes;
			var numBoxes = boxes.Count;

			// Check all AABB's for their cells
			for ( int b = 0; b < numBoxes; b++ )
			{

				var corners = boxes[b].GetCorners();

				// Check the four corners of each AABB to see what cells they exist in
				foreach ( var corner in corners )
				{

					var index = IndexOf(corner.ToPoint());
					if ( IndexExists(index) )
					{
						// The corner is inside the grid space so insert the entity
						var cell = _cells[index.X, index.Y];
						// Builds up the cells over time rather than all upfront
						// to prevent situations with big grids and not many entities.
						if ( cell == null )
						{
							_cells[index.X, index.Y] = new CollisionCell();
							cell = _cells[index.X, index.Y];
						}
						// Check if the cell doesn't already have the entity
						// and insert
						if ( !cell.Contains(entity) )
						{
							cell.Add(entity);
							_nonEmptyCells.Add(cell);
						}
					}

				}

			}
		}

		/// <summary>
		/// Gets a list of all entities located in the same cell/s as a specific single entity
		/// </summary>
		/// <returns>The entities neighbours.</returns>
		/// <param name="entity">Entity to get collisions for.</param>
		/// <param name="collision">Collision component to use in checking.</param>
		public List<Entity> GetCollisions(Entity entity, CollisionComponent collision)
		{
			var result = new List<Entity>();
			var boxes = collision.Boxes;

			Vector2[] corners = new Vector2[4];
			for ( int b = 0; b < boxes.Count; b++ )
			{
				corners = boxes[b].GetCorners();
			}

			// Reset set of checked ID's
			_checkedIDs.Clear();

			// Check each corner of the entities AABB for neighbours
			foreach ( var corner in corners )
			{
				var index = IndexOf(corner.ToPoint());
				if ( IndexExists(index) )
				{
					var cell = _cells[index.X, index.Y];

					if ( cell != null )
					{
						var maxItems = cell.Items.Count;
						// Valid cell, grab all the neighbours if they haven't already been checked
						foreach ( var e in cell.Items )
						{
							if ( !_checkedIDs.Contains(e.ID) && e.ID != entity.ID )
							{
								result.Add(e);
								_checkedIDs.AddFirst(e.ID); // fast insert
							}
						}

					}

				}
			}

			return result;
		}

		/// <summary>
		/// Updates the position of the collision grid.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void UpdatePosition(int x, int y)
		{
			_min.X = x;
			_min.Y = y;
			_max.X = _max.X + x;
			_max.Y = _max.Y + y;
		}

		/// <summary>
		/// Clears all non-empty cells of the grid from their entities
		/// </summary>
		public void Clear()
		{
			var nonEmptySize = _nonEmptyCells.Count;
			for ( int c = 0; c < nonEmptySize; c++ )
			{
				_nonEmptyCells[c].Clear();
			}
			_nonEmptyCells.Clear();
		}

		/// <summary>
		/// Gets the geometric representation of the grid
		/// </summary>
		/// <value>The grid.</value>
		public Grid Grid
		{
			get { return _grid; }
		}

		/// <summary>
		/// Gets the current position of the grid.
		/// </summary>
		/// <value>The position.</value>
		public Vector2 Position
		{
			get { return _min; }
		}

		/// <summary>
		/// Gets the upper bounds of the x and y coordinates in the grid
		/// </summary>
		/// <value>The max coordinates.</value>
		public Vector2 Max
		{
			get { return _max; }
		}
	}
}
