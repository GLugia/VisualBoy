using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.Core.Extensions
{
	public static class SpriteBatchExtensions
	{
		public static void DrawBorderedString(this SpriteBatch batch, SpriteFont font, string text, Vector2 position, Color color, float scale = 1f)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					if (string.IsNullOrEmpty(text))
					{
						return;
					}

					batch.DrawString(font, text, position + (new Vector2(i, j) * scale), color.Invert(), 0f, default, scale, SpriteEffects.None, 0f);
				}
			}

			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			batch.DrawString(font, text, position, color, 0f, default, scale, SpriteEffects.None, 0f);
		}
	}
}
