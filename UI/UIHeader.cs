using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.UI
{
	public class UIHeader : UIElement
	{
		private SpriteFont Font;
		public string Text;

		public UIHeader(SpriteFont font, string text)
		{
			Font = font;
			Text = text;
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			batch.DrawString(Font, Text, Dimensions.Position, Color.White);
		}
	}
}
