using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.UI.Elements
{
	public class UITextPanel<T> : UIPanel
	{
		public bool IsLarge { get; private set; }
		public bool DrawPanel = true;
		public float TextScale = 1f;
		public Vector2 TextSize = Vector2.Zero;
		public string Text => text?.ToString() ?? "";
		private T text;
		public Color TextColor = Color.White;

		public UITextPanel(T text, float scale = 1f, bool large = false)
		{
			SetText(text, scale, large);
		}

		public override void Recalculate()
		{
			SetText(text, TextScale, IsLarge);
			base.Recalculate();
		}

		public void SetText(T text)
		{
			SetText(text, TextScale, IsLarge);
		}

		public virtual void SetText(T text, float scale, bool large)
		{
			Vector2 size = (large ? Main.Xiq12 : Main.Xiq12).MeasureString(text.ToString()) * scale;
			size.Y = (large ? 28f : 16f) * scale;
			this.text = text;
			TextScale = scale;
			TextSize = size;
			IsLarge = large;
			MinWidth.Set(size.X + PaddingLeft + PaddingRight, 0f);
			MinHeight.Set(size.Y + PaddingTop + PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			if (DrawPanel)
			{
				base.DrawSelf(batch);
			}

			Vector2 position = InnerDimensions.Position;
			position.X += (InnerDimensions.Width - TextSize.X) * TextScale * 0.5f;
			position.Y -= (InnerDimensions.Height - TextSize.Y);

			if (IsLarge)
			{
				batch.DrawBorderedString(Main.Xiq12, Text, position, TextColor);
				return;
			}

			batch.DrawBorderedString(Main.Xiq12, Text, position, TextColor);
		}
	}
}
