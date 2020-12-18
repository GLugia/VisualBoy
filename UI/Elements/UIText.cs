using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.UI.Elements
{
	public class UIText : UIElement
	{
		public object Text { get; private set; } = "";
		public float TextScale = 1f;
		public Vector2 TextSize { get; private set; } = Vector2.Zero;
		public bool IsLarge { get; private set; }
		public Color TextColor = Color.White;

		public UIText(object text, float scale = 1f, bool large = false)
		{
			InternalSetText(text, scale, large);
		}

		public override void Recalculate()
		{
			InternalSetText(Text, TextScale, IsLarge);
			base.Recalculate();
		}

		public void SetText(object text, float scale = 1f, bool large = false)
		{
			InternalSetText(text, scale, large);
		}

		private void InternalSetText(object text, float scale, bool large)
		{
			Vector2 size = (large ? Main.Xiq12 : Main.Xiq12).MeasureString(text.ToString()) * scale;
			size.Y = (large ? 28f : 16f) * scale;
			Text = text;
			TextScale = scale;
			TextSize = size;
			IsLarge = large;
			MinWidth.Set(size.X + PaddingLeft + PaddingRight, 0f);
			MinHeight.Set(size.Y + PaddingTop + PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			base.DrawSelf(batch);

			Vector2 position = InnerDimensions.Position;
			position.X += (InnerDimensions.Width - TextSize.X) * TextScale * 0.5f;
			position.Y -= (InnerDimensions.Height - TextSize.Y);

			if (IsLarge)
			{
				batch.DrawBorderedString(Main.Xiq12, Text.ToString(), position, TextColor, TextScale);
				return;
			}

			batch.DrawBorderedString(Main.Xiq12, Text.ToString(), position, TextColor, TextScale);
		}
	}
}
