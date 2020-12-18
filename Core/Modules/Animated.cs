using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VisualBoy.Core.Modules
{
	public class Animated : Module
	{
		private List<Rectangle> Frames;
		private int Direction, LastDirection, CurrentFrame;
		private List<Rectangle>[] Animations;

		public override void Load(ref Sprite parent)
		{
			Frames = new List<Rectangle>();
			Animations = new List<Rectangle>[5];

			for (int i = 0; i < Animations.Length; i++)
			{
				Animations[i] = new List<Rectangle>();
			}

			MediaCache.LoadTexture(parent.TexturePath, out parent.Texture);
			Texture2D tex = MediaCache.GetTexture(parent.Texture);

			int j = 0;
			for (int i = 0; i < tex.Width; i += parent.Width)
			{
				Frames.Add(new Rectangle(i, j, parent.Width, parent.Height));

				if (i == tex.Width - parent.Width && j == tex.Height - parent.Height)
				{
					break;
				}

				if (i == tex.Width - parent.Width)
				{
					j += parent.Height;
					i = 0;
				}
			}
		}

		public override void Unload(ref Sprite parent)
		{
			Frames = null;
			Animations = null;
		}

		public override void Activate(ref Sprite parent)
		{
			MediaCache.LoadTexture(parent.TexturePath, out parent.Texture);
		}

		public override void Deactivate(ref Sprite parent)
		{
			MediaCache.UnloadTexture(parent.Texture);
		}

		public override void Update(GameTime time, ref Sprite parent)
		{
			LastDirection = Direction;

			if (parent.Velocity != Vector2.Zero && FrameWatcher.TotalFrames % 10 == 0)
			{
				CurrentFrame++;
			}
			else if (parent.Velocity == Vector2.Zero)
			{
				CurrentFrame = 0;
			}

			if (CurrentFrame > Animations[Direction].Count - 1)
			{
				CurrentFrame = 0;
			}

			if (parent.Velocity.X > 0)
			{
				Direction = 4;
			}
			else if (parent.Velocity.X < 0)
			{
				Direction = 3;
			}
			else if (parent.Velocity.Y < 0)
			{
				Direction = 2;
			}
			else if (parent.Velocity.Y > 0)
			{
				Direction = 1;
			}

			if (LastDirection != Direction)
			{
				CurrentFrame = 0;
			}
		}

		public override void Draw(SpriteBatch batch, ref Sprite parent)
		{
			Rectangle? rect = new Rectangle?(Animations[Direction][CurrentFrame]);
			batch.Draw(MediaCache.GetTexture(parent.Texture), parent.Position, rect, Color.White);
		}

		public Animated SetIdleFrames(params int[] frames)
		{
			for (int i = 0; i < frames.Length; i++)
			{
				Animations[0].Add(Frames[frames[i]]);
			}

			return this;
		}

		public Animated SetLeftFrames(params int[] frames)
		{
			for (int i = 0; i < frames.Length; i++)
			{
				Animations[3].Add(Frames[frames[i]]);
			}

			return this;
		}

		public Animated SetRightFrames(params int[] frames)
		{
			for (int i = 0; i < frames.Length; i++)
			{
				Animations[4].Add(Frames[frames[i]]);
			}

			return this;
		}

		public Animated SetDownFrames(params int[] frames)
		{
			for (int i = 0; i < frames.Length; i++)
			{
				Animations[1].Add(Frames[frames[i]]);
			}

			return this;
		}

		public Animated SetUpFrames(params int[] frames)
		{
			for (int i = 0; i < frames.Length; i++)
			{
				Animations[2].Add(Frames[frames[i]]);
			}

			return this;
		}
	}
}
