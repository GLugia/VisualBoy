using VisualBoy.Core.Extensions;
using VisualBoy.Core.Modules.Players;
using Microsoft.Xna.Framework;
using System;

namespace VisualBoy.Core.Modules.Tiles
{
	public class Moveable : Tile
	{
		public Sprite[] RequiredItems;
		public bool Moving { get; private set; }
		private Point direction;
		private Vector2 MovedDistance;
		private const float StepSpeed = 1.5f;
		private const float FramesPerMovement = 30f;

		public override bool PreUpdate(GameTime time, ref Sprite parent)
		{
			Main.DEBUG_MISC_TEXT = parent.Position;
			return base.PreUpdate(time, ref parent);
		}

		public override void Update(GameTime time, ref Sprite parent)
		{
			if (!Moving)
			{
				bool flag = true;

				if (RequiredItems != null)
				{
					for (int i = 0; i < RequiredItems.Length; i++)
					{
						if (!Main.Player.GetModule<Player>().HasItem(RequiredItems[i]))
						{
							flag = false;
							break;
						}
					}
				}

				if (flag && !Moving && Main.Player.IsNextTo(parent) && Main.Player.MovingToward(parent))
				{
					Moving = true;

					if (Main.Player.Velocity.X > 0f)
					{
						direction.X = 1;
					}

					if (Main.Player.Velocity.X < 0f)
					{
						direction.X = -1;
					}

					if (Main.Player.Velocity.Y > 0f)
					{
						direction.Y = 1;
					}

					if (Main.Player.Velocity.Y < 0f)
					{
						direction.Y = -1;
					}

					if (Collision.HasCollision(parent.INSTANCE_HASH, parent.TilePosition + direction))
					{
						Moving = false;
					}
				}

				return;
			}

			Move(direction.X, direction.Y, ref parent);
		}

		private void Move(float x, float y, ref Sprite parent)
		{
			// 32f being tile size
			float speed = (32f / FramesPerMovement) * StepSpeed;
			// Set the velocity
			parent.Velocity += new Vector2(x * speed, y * speed);
			// Add it to moved distance. sprite position is updating in the base sprite class.
			MovedDistance += parent.Velocity;

			Vector2 abs = MovedDistance.Abs();
			// if the sprite is moving and moved either x or y OR hit a wall 20 frames ago, stop moving and listen for inputs again
			if (abs.X.Between(31.9f, 32.1f)
				|| abs.Y.Between(31.9f, 32.1f))
			{
				parent.Velocity = default;
				MovedDistance = default;
				direction = default;
				parent.CheckPosition = true;
				Moving = false;
			}
		}
	}
}
