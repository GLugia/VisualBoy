using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.UI.Elements
{
	public class UIHeader : UIElement
	{
		private string text;
		public string Text
		{
			get => text;
			set
			{
				if (text != value)
				{
					text = value;
					Vector2 vector = Main.Xiq12.MeasureString(Text);
					Width.Set(vector.X, 0f);
					Height.Set(vector.Y, 0f);
					Recalculate();
				}
			}
		}

		public UIHeader() : this("") { }
		public UIHeader(string text)
		{
			Text = text;
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			batch.DrawBorderedString(Main.Xiq12, Text, Dimensions.Position, Color.White);
		}
	}
}
