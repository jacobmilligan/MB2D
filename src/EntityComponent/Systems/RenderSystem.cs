//
// 	RenderSystem.cs
// 	MB2D Engine
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MB2D.EntityComponent
{
	/// <summary>
	/// Renders culled entities with a SpriteTransform to the window
	/// </summary>
	public class RenderSystem : EntitySystem
	{
		/// <summary>
		/// The sprite batch to draw to.
		/// </summary>
		private SpriteBatch _spriteBatch;

		/// <summary>
		/// Number of entities draw the last frame. Used for debugging
		/// </summary>
		private int _drawn;

		/// <summary>
		/// The cameras current bounds and position
		/// </summary>
		private RectangleF _cameraRect;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:MB2D.EntityComponent.RenderSystem"/> class.
		/// </summary>
		/// <param name="spriteBatch">Sprite batch to draw to.</param>
		public RenderSystem(SpriteBatch spriteBatch)
		  : base(typeof(SpriteTransform))
		{
			_spriteBatch = spriteBatch;
		}

		/// <summary>
		/// Re-orders the list of AssociatedEntities based on their current z-index
		/// </summary>
		protected override void PreProcess()
		{
			_drawn = 0;
			// Re-order based on z
			AssociatedEntities = AssociatedEntities.OrderBy(
			  entity => entity.GetComponent<SpriteTransform>().Z
			).ToList();
			// Update the current frames camera position
			_cameraRect = MBGame.Camera.BoundingRectangle;
		}

		/// <summary>
		/// Culls and then draws an entity to the window
		/// </summary>
		/// <param name="entity">Entity.</param>
		protected override void Process(Entity entity)
		{
			var sprite = entity.GetComponent<SpriteTransform>();

			// Cull the entity
			if ( sprite != null && _cameraRect.Intersects(sprite.Bounds) )
			{
				_spriteBatch.Draw(sprite.Target);
				_drawn++;
			}
		}
	}
}
