using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VisualBoy.UI.Elements
{
	public class UITextBox : UITextPanel<string>
	{
		public bool Active;
		private int Cursor;
		private int FrameCount;
		private const int MaxLength = 10;
		private int CursorDelayTimer = 8;

		public UITextBox(string text, float scale = 1f, bool large = false) : base(text, scale, large)
		{
			Cursor = text.Length;
		}

		public void Write(char c)
		{
			if (Cursor == Text.Length)
			{
				SetText(Text + c);
			}
			else if (Cursor < Text.Length)
			{
				SetText(Text.Insert(Cursor, c.ToString()));
			}
		}

		public override void SetText(string text, float scale, bool large)
		{
			int diff = Text.Length - text.Length;
			if (Text.Length == MaxLength && diff <= 0)
			{
				return;
			}
			Cursor -= diff;
			base.SetText(text, scale, large);
			Cursor = Math.Min(Cursor, text.Length);
			Main.DEBUG_MISC_TEXT = "Cursor: " + Cursor;
		}

		public void Backspace()
		{
			if (Cursor > 0)
			{
				string a = Text.Substring(0, Cursor - 1);

				if (Cursor < Text.Length)
				{
					string b = Text.Substring(Cursor);
					a += b;
				}

				SetText(a);
			}

			Main.DEBUG_MISC_TEXT = "Cursor: " + Cursor;
		}

		public void CursorLeft()
		{
			if (Cursor > 0 && CursorDelayTimer <= 0)
			{
				Cursor--;
				CursorDelayTimer = 8;
			}

			Main.DEBUG_MISC_TEXT = "Cursor: " + Cursor;
		}

		public void CursorRight()
		{
			if (Cursor < Text.Length && CursorDelayTimer <= 0)
			{
				Cursor++;
				CursorDelayTimer = 8;
			}

			Main.DEBUG_MISC_TEXT = "Cursor: " + Cursor;
		}

		public override void Update(GameTime time)
		{
			CursorDelayTimer--;
			base.Update(time);
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			base.DrawSelf(batch);

			FrameCount++;

			if (Active && (FrameCount %= 40) <= 20)
			{
				Vector2 position = InnerDimensions.Position;
				string str = Cursor > Text.Length ? Text : Text.Substring(0, Cursor);
				Vector2 vec = (IsLarge ? Main.Xiq12 : Main.Xiq12).MeasureString(str) * TextScale;

				if (IsLarge)
				{
					position.Y -= 6f * TextScale;
				}
				else
				{
					position.Y += 6f * TextScale;
				}

				position.X += (InnerDimensions.Width - TextSize.X) * 0.5f + vec.X - (IsLarge ? 8f : 4f) * TextScale + 6f;

				if (IsLarge)
				{
					batch.DrawBorderedString(Main.Xiq12, "|", position, TextColor, TextScale);
					return;
				}

				batch.DrawBorderedString(Main.Xiq12, "|", position, TextColor, TextScale);
			}

			Active = false;
		}
	}
}
