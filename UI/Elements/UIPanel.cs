using VisualBoy.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.UI.Elements
{
	public class UIPanel : UIElement
	{
		public Color BackgroundColor = new Color(63, 82, 151) * 0.7f;
		public Color BorderColor = Color.Black;

		private static string BorderTexture;
		private static string BackgroundTexture;
		private static readonly int CORNER_SIZE = 12;
		private static readonly int BAR_SIZE = 4;

		public override void OnActivate()
		{
			if (BorderTexture == null)
			{
				MediaCache.LoadTexture("UI/PanelBorder", out BorderTexture);
			}

			if (BackgroundTexture == null)
			{
				MediaCache.LoadTexture("UI/PanelBackground", out BackgroundTexture);
			}
		}

		public UIPanel()
		{
			SetPadding(CORNER_SIZE, CORNER_SIZE, CORNER_SIZE, CORNER_SIZE);
		}

		private void DrawPanel(SpriteBatch batch, string texture, Color color)
		{
			Rectangle dim = Dimensions.AsRectangle;
			Point rightside = new Point(dim.X + dim.Width - CORNER_SIZE, dim.Y + dim.Height - CORNER_SIZE / 2);
			int width = rightside.X - dim.X - CORNER_SIZE;
			int height = rightside.Y - dim.Y - CORNER_SIZE;
			Texture2D tex = MediaCache.GetTexture(texture);

			// Corners
			batch.Draw(tex, new Rectangle(dim.X, dim.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(0, 0, CORNER_SIZE, CORNER_SIZE)), color);
			batch.Draw(tex, new Rectangle(rightside.X, dim.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE + BAR_SIZE, 0, CORNER_SIZE, CORNER_SIZE)), color);
			batch.Draw(tex, new Rectangle(dim.X, rightside.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(0, CORNER_SIZE + BAR_SIZE, CORNER_SIZE, CORNER_SIZE)), color);
			batch.Draw(tex, new Rectangle(rightside.X, rightside.Y, CORNER_SIZE, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE + BAR_SIZE, CORNER_SIZE + BAR_SIZE, CORNER_SIZE, CORNER_SIZE)), color);

			batch.Draw(tex, new Rectangle(dim.X + CORNER_SIZE, dim.Y, width, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE, 0, BAR_SIZE, CORNER_SIZE)), color);
			batch.Draw(tex, new Rectangle(dim.X + CORNER_SIZE, rightside.Y, width, CORNER_SIZE), new Rectangle?(new Rectangle(CORNER_SIZE, CORNER_SIZE + BAR_SIZE, BAR_SIZE, CORNER_SIZE)), color);
			batch.Draw(tex, new Rectangle(dim.X, dim.Y + CORNER_SIZE, CORNER_SIZE, height), new Rectangle?(new Rectangle(0, CORNER_SIZE, CORNER_SIZE, BAR_SIZE)), color);
			batch.Draw(tex, new Rectangle(rightside.X, dim.Y + CORNER_SIZE, CORNER_SIZE, height), new Rectangle?(new Rectangle(CORNER_SIZE + BAR_SIZE, CORNER_SIZE, CORNER_SIZE, BAR_SIZE)), color);
			batch.Draw(tex, new Rectangle(dim.X + CORNER_SIZE, dim.Y + CORNER_SIZE, width, height), new Rectangle?(new Rectangle(CORNER_SIZE, CORNER_SIZE, BAR_SIZE, BAR_SIZE)), color);
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			DrawPanel(batch, BackgroundTexture, BackgroundColor);
			DrawPanel(batch, BorderTexture, BorderColor);
		}
	}
}
