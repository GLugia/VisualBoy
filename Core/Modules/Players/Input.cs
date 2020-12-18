using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace VisualBoy.Core.Modules.Players
{
	public class Input : Module
	{
		// Whether or not to listen for inputs
		public bool Paused = false;
		// Whether or not the sprite is moving
		public bool Moving { get; private set; }
		// The total distance traveled since the last keypress
		private Vector2 MovedDistance;
		// The speed in pixels the sprite is allowed to move
		public const float StepSpeed = 1f;
		// The number of frames it should take for a movement to be "complete"
		public const float FramesPerMovement = 20f;
		// 0 is up, 1 is down, 2 is left, 3 is right // -1 is default
		private sbyte KeyCode = -1;
		// The number of frames the sprite has spent colliding with a wall
		private long WallHitTimer = 0;
		// The last velocity the sprite had before it hit a wall
		private Vector2 LastVelOnWall;

		// Called before sprite position is updated but after velocity has been reset
		public override void Update(GameTime time, ref Sprite parent)
		{
			if (Paused)
			{
				return; // if something called Disable, stop checking inputs
			}

			// When the sprite collides with another object, we save their velocity and set it here
			if (LastVelOnWall != default)
			{
				parent.Velocity = LastVelOnWall;
				return;
			}

			if (!Moving) // if the sprite is not moving
			{
				if (Main.kbState.IsKeyDown(Keys.W)) // check for w
				{
					KeyCode = 0; // w was pressed
					Moving = true; // sprite is moving
				}
				else if (Main.kbState.IsKeyDown(Keys.S)) // check for s
				{
					KeyCode = 1; // s was pressed
					Moving = true; // sprite is moving
				}
				else if (Main.kbState.IsKeyDown(Keys.A)) // etc
				{
					KeyCode = 2;
					Moving = true;
				}
				else if (Main.kbState.IsKeyDown(Keys.D))
				{
					KeyCode = 3;
					Moving = true;
				}
			}

			switch (KeyCode)
			{
				case 0: // Up
					{
						Move(0, -1, ref parent);
						break;
					}
				case 1: // Down
					{
						Move(0, 1, ref parent);
						break;
					}
				case 2: // Left
					{
						Move(-1, 0, ref parent);
						break;
					}
				case 3: // Right
					{
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

		private void Move(int x, int y, ref Sprite parent)
		{
			// 32f being tile size
			float speed = (32f / FramesPerMovement) * StepSpeed;
			// Set the velocity
			parent.Velocity += new Vector2(x * speed, y * speed);
			// Add it to moved distance. sprite position is updating in the base sprite class.
			MovedDistance += parent.Velocity;

			Vector2 abs = new Vector2(Math.Abs(MovedDistance.X), Math.Abs(MovedDistance.Y));
			// if the sprite is moving and moved either x or y OR hit a wall 20 frames ago, stop moving and listen for inputs again
			if ((Moving
				&& (abs.X.Between(31.9f, 32.1f)
				|| abs.Y.Between(31.9f, 32.1f)))
				|| FrameWatcher.TotalFrames - WallHitTimer == 20)
			{
				//parent.Position += parent.Velocity;
				WallHitTimer = 0;
				parent.Velocity = default;
				MovedDistance = default;
				parent.CheckPosition = true;
				Moving = false;
				KeyCode = -1;
			}
		}

		public override void PostUpdate(GameTime time, ref Sprite parent)
		{
			if (FrameWatcher.TotalFrames - WallHitTimer == 20)
			{
				LastVelOnWall = default;
				WallHitTimer = 0;
				parent.Velocity = default;
				MovedDistance = default;
				parent.CheckPosition = true; // tells the base class it's ok to round the position to tile coordinates
				Moving = false;
				KeyCode = -1;
				return;
			}

			// I set velocity to a super low number in collision to allow animations to update properly
			// this also lets me check if a sprite collided with something without my specific collision class existing
			// so long as someone sets the velocity to this number anyway
			if (parent.Velocity.AnyEqual(0.0001f, -0.0001f) && WallHitTimer == 0)
			{
				LastVelOnWall = parent.Velocity;
				WallHitTimer = FrameWatcher.TotalFrames;
			}
		}

		public Input Enable()
		{
			Paused = false;
			return this;
		}

		public Input Disable()
		{
			Paused = true;
			return this;
		}
	}
}
