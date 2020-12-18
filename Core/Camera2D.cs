using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.Core
{
	public class Camera2D
	{
		public Vector2 Position = Vector2.Zero;
		public float Rotation = 0f;
		public float Zoom = 1f;
		public Rectangle Bounds { get; private set; }

		public Camera2D(Viewport vp)
		{
			Bounds = vp.Bounds;
		}

		public Matrix Transform => Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0f))
				* Matrix.CreateRotationZ(Rotation)
				* Matrix.CreateScale(Zoom)
				* Matrix.CreateTranslation(new Vector3(Bounds.Width / 2f, Bounds.Height / 2f, 0f));

		public Rectangle VisibleArea
		{
			get
			{
				Matrix Inverse = Matrix.Invert(Transform);
				Vector2 xx = Vector2.Transform(Vector2.Zero, Inverse);
				Vector2 xy = Vector2.Transform(new Vector2(Main.ScreenRect.Right, 0f), Inverse);
				Vector2 yx = Vector2.Transform(new Vector2(0f, Main.ScreenRect.Bottom), Inverse);
				Vector2 yy = Vector2.Transform(new Vector2(Main.ScreenRect.Right, Main.ScreenRect.Bottom), Inverse);

				Vector2 min = new Vector2
					(
						MathHelper.Min(xx.X, MathHelper.Min(xy.X, MathHelper.Min(yx.X, yy.X))),
						MathHelper.Min(xx.Y, MathHelper.Min(xy.Y, MathHelper.Min(yx.Y, yy.Y)))
					);
				Vector2 max = new Vector2
					(
						MathHelper.Max(xx.X, MathHelper.Max(xy.X, MathHelper.Max(yx.X, yy.X))),
						MathHelper.Max(xx.Y, MathHelper.Max(xy.Y, MathHelper.Max(yx.Y, yy.Y)))
					);

				return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
			}
		}
	}
}
