using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace VisualBoy.Core.Modules
{
	public class Collision : Module
	{
		private static Dictionary<int, Queue<Point>> Collisions;
		private static Dictionary<int, Point> New;

		public delegate void CollisionEvent(ref Sprite origin, ref Sprite other);
		public event CollisionEvent OnCollide;

		public override void Activate(ref Sprite parent)
		{
			if (Collisions == null)
			{
				Collisions = new Dictionary<int, Queue<Point>>();
				Queue<Point> mappoints = new Queue<Point>();

				for (int i = -1; i <= Main.currentmap.Width; i++)
				{
					for (int j = -1; j <= Main.currentmap.Height; j++)
					{
						if (i == -1 || i == Main.currentmap.Width || j == -1 || j == Main.currentmap.Height)
						{
							mappoints.Enqueue(new Point(i, j));
						}
					}
				}

				Collisions.Add(-1, mappoints);
			}

			if (New == null)
			{
				New = new Dictionary<int, Point>();
			}

			Queue<Point> origin = new Queue<Point>();
			origin.Enqueue(parent.TilePosition);
			Collisions.Add(parent.INSTANCE_HASH, origin);
		}

		public override void Deactivate(ref Sprite parent)
		{
			New.Remove(parent.INSTANCE_HASH);
			Collisions.Remove(parent.INSTANCE_HASH);
		}

		public override bool PreUpdate(GameTime time, ref Sprite parent)
		{
			return Main.currentmap != default;
		}

		public override void Update(GameTime _, ref Sprite parent)
		{
			if (Main.currentmap == default)
			{
				parent.Velocity = default;
				return;
			}

			if (parent.Velocity != default)
			{
				if (parent.Velocity.X < 0f && !CanMove(parent.INSTANCE_HASH, parent.TilePosition, -1, 0))
				{
					parent.Velocity.X = -0.0001f;
					parent.CheckPosition = true;
				}

				if (parent.Velocity.X > 0f && !CanMove(parent.INSTANCE_HASH, parent.TilePosition, 1, 0))
				{
					parent.Velocity.X = 0.0001f;
					parent.CheckPosition = true;
				}

				if (parent.Velocity.Y < 0f && !CanMove(parent.INSTANCE_HASH, parent.TilePosition, 0, -1))
				{
					parent.Velocity.Y = -0.0001f;
					parent.CheckPosition = true;
				}

				if (parent.Velocity.Y > 0f && !CanMove(parent.INSTANCE_HASH, parent.TilePosition, 0, 1))
				{
					parent.Velocity.Y = 0.0001f;
					parent.CheckPosition = true;
				}
			}
		}

		public override void PostUpdate(GameTime time, ref Sprite parent)
		{
			if (Collisions.TryGetValue(parent.INSTANCE_HASH, out Queue<Point> qu)
				&& New.TryGetValue(parent.INSTANCE_HASH, out Point pos))
			{
				if (parent.Velocity == default && pos == parent.TilePosition)
				{
					qu.Dequeue();
					New.Remove(parent.INSTANCE_HASH);
				}
				else if (parent.Velocity.AnyEqual(0.0001f, -0.0001f))
				{
					qu.Clear();
					qu.Enqueue(parent.TilePosition);
					New.Remove(parent.INSTANCE_HASH);
				}
			}
		}

		private bool CanMove(int id, Point tilePos, int x = 0, int y = 0)
		{
			Point npos = new Point(tilePos.X + x, tilePos.Y + y);

			bool flag = true;

			foreach ((int _, Queue<Point> points) in Collisions)
			{
				if (points.Contains(npos))
				{
					flag = false;
					break;
				}
			}

			if (flag)
			{
				if (New.ContainsKey(id))
				{
					New.Remove(id);
				}

				if (Collisions.TryGetValue(id, out Queue<Point> qu))
				{
					qu.Enqueue(npos);
				}

				New.Add(id, npos);
				return true;
			}

			if (New.TryGetValue(id, out Point pos))
			{
				return pos == npos;
			}

			return false;
		}

		public static bool HasCollision(int sprite, Point point)
		{
			foreach ((int hash, Queue<Point> qu) in Collisions)
			{
				if (hash == sprite)
				{
					continue;
				}

				if (qu.Contains(point))
				{
					return true;
				}
			}

			return false;
		}

		public override void Draw(SpriteBatch batch, ref Sprite parent)
		{
#if DEBUG
			Color HitboxColor = Color.FromNonPremultiplied(96, 196, 255, 255);
			Texture2D pixel = MediaCache.GetTexture("pixel");
			foreach (Queue<Point> qu in Collisions.Values)
			{
				foreach (Point point in qu)
				{
					batch.Draw(pixel, new Rectangle(point.X * 32, point.Y * 32, 32, 1), HitboxColor);
					batch.Draw(pixel, new Rectangle(point.X * 32, point.Y * 32, 1, 32), HitboxColor);
					batch.Draw(pixel, new Rectangle(point.X * 32, (point.Y + 1) * 32, 33, 1), HitboxColor);
					batch.Draw(pixel, new Rectangle((point.X + 1) * 32, point.Y * 32, 1, 32), HitboxColor);
				}
			}
#endif
		}
	}
}
