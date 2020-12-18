using VisualBoy.Core.Extensions;
using VisualBoy.Core.Modules.Players;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace VisualBoy.Core.Modules.NPCs
{
	public class TownAI : AIm
	{
		private bool Moving = false;
		private const float StepSpeed = 1.5f;
		private const float FramesPerMovement = 30f;
		private Vector2 MovedDistance = default;
		private sbyte KeyCode = 1;
		private byte StepTimer = Main.rand.Byte();

		public override void Update(GameTime time, ref Sprite parent)
		{
			if (!Moving)
			{
				ait[0]++;

				if (ait[0] >= StepTimer)
				{
					ait[0] = 0f;
					StepTimer = Main.rand.Byte();

					if (Main.rand.Single() <= 0.5f)
					{
						Moving = true;
					}
					else
					{
						KeyCode = (sbyte)Main.rand.Int16(-1, 4);

						switch (KeyCode)
						{
							case -1: break;
							case 0: parent.Velocity.Y = -0.0001f; break;
							case 1: parent.Velocity.Y = 0.0001f; break;
							case 2: parent.Velocity.X = -0.0001f; break;
							case 3: parent.Velocity.X = 0.0001f; break;
						}

						parent.CheckPosition = true;
					}
				}

				return;
			}

			List<sbyte> possibleDirections = new List<sbyte>()
			{
				0, 1, 2, 3
			};
		Move:

			switch (KeyCode)
			{
				case 0: // Up
					{
						if (Collision.HasCollision(parent.INSTANCE_HASH, parent.TilePosition + new Point(0, -1)))
						{
							possibleDirections.Remove(0);
							if (possibleDirections.Count == 0)
							{
								Moving = false;
								return;
							}

							KeyCode = possibleDirections[Main.rand.Int32(0, possibleDirections.Count)];
							goto Move;
						}

						Move(0, -1, ref parent);
						break;
					}
				case 1: // Down
					{
						if (Collision.HasCollision(parent.INSTANCE_HASH, parent.TilePosition + new Point(0, 1)))
						{
							possibleDirections.Remove(1);
							if (possibleDirections.Count == 0)
							{
								Moving = false;
								return;
							}

							KeyCode = possibleDirections[Main.rand.Int32(0, possibleDirections.Count)];
							goto Move;
						}

						Move(0, 1, ref parent);
						break;
					}
				case 2: // Left
					{
						if (Collision.HasCollision(parent.INSTANCE_HASH, parent.TilePosition + new Point(-1, 0)))
						{
							possibleDirections.Remove(2);
							if (possibleDirections.Count == 0)
							{
								Moving = false;
								return;
							}

							KeyCode = possibleDirections[Main.rand.Int32(0, possibleDirections.Count)];
							goto Move;
						}

						Move(-1, 0, ref parent);
						break;
					}
				case 3: // Right
					{
						if (Collision.HasCollision(parent.INSTANCE_HASH, parent.TilePosition + new Point(1, 0)))
						{
							possibleDirections.Remove(3);
							if (possibleDirections.Count == 0)
							{
								Moving = false;
								return;
							}

							KeyCode = possibleDirections[Main.rand.Int32(0, possibleDirections.Count)];
							goto Move;
						}

						Move(1, 0, ref parent);
						break;
					}
				case -1: // None
					{
						Moving = false;
						break;
					}
				default: // idk how you'd get here but ok dood
					{
						break;
					}
			}
		}

		private void Move(float x, float y, ref Sprite parent)
		{
			// 32f being tile size
			float speed = (32f / FramesPerMovement) * StepSpeed;
			// Set the velocity
			parent.Velocity += new Vector2(x * speed, y * speed);
			// Add it to moved distance. sprite position is updating in the base sprite class.
			MovedDistance += parent.Velocity;

			Vector2 abs = new Vector2(Math.Abs(MovedDistance.X), Math.Abs(MovedDistance.Y));
			// if the sprite is moving and moved either x or y OR hit a wall 20 frames ago, stop moving and listen for inputs again
			if (abs.X.Between(31.9f, 32.1f)
				|| abs.Y.Between(31.9f, 32.1f))
			{
				//parent.Position += parent.Velocity;
				parent.Velocity = default;
				MovedDistance = default;
				parent.CheckPosition = true;
				Moving = false;
			}
		}
	}
}
