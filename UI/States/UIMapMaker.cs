using VisualBoy.Core;
using VisualBoy.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace VisualBoy.UI.States
{
	public class UIMapMaker : UIState
	{
		private Map ActiveMap;
		private readonly UITextBox[] Field;
		private int ActiveField;

		public UIMapMaker() : base()
		{
			Main.instance.Window.TextInput += SetText;
			ActiveField = -1;
			Field = new UITextBox[3];
		}

		public override void OnDeactivate()
		{
			if (ActiveMap == default)
			{
				return;
			}

			if (!Directory.Exists("Maps"))
			{
				Directory.CreateDirectory("Maps");
			}

			Map.Save(ActiveMap);
		}

		private void SetText(object sender, TextInputEventArgs args)
		{
			if (ActiveField == -1)
			{
				return;
			}

			switch (args.Key)
			{
				case Keys.Escape: Main.instance.MenuUI.GoBack(); break;
				case Keys.Back: Field[ActiveField].Backspace(); break;
				case Keys.Tab:
				case Keys.Enter: ActiveField++; break;
				default: Field[ActiveField].Write(args.Character); break;
			}
		}

		public override void OnInitialize()
		{
			Field[0] = new UITextBox("Map Name", 1f, true)
			{
				TextColor = Color.White,
				HAlign = 0.5f,
				VAlign = 0.5f
			};
			Field[0].Width.Set(350f, 0f);
			Append(Field[0]);

			Recalculate();
		}

		public override void LeftClick(UIMouseEvent evt)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Field[i] != null && Field[i].ContainsPoint(evt.MousePosition))
				{
					ActiveField = i;
					Main.DEBUG_MISC_TEXT = "Active input box set to " + i;
				}
			}
		}

		public override void Update(GameTime time)
		{
			if (ActiveField > 2)
			{
				ActiveField--;
			}

			if (ActiveField != -1 && Field[ActiveField] != null)
			{
				Field[ActiveField].Active = true;

				foreach (Keys key in Main.kbState.GetPressedKeys())
				{
					switch (key)
					{
						case Keys.Left: Field[ActiveField].CursorLeft(); break;
						case Keys.Right: Field[ActiveField].CursorRight(); break;
						default: break;
					}
				}
			}

			CheckContainsText();

			base.Update(time);
		}

		private void CheckContainsText()
		{
			bool flag = true;

			for (int i = 0; i < 3; i++)
			{
				if (Field[i] != null && string.IsNullOrEmpty(Field[i].Text))
				{
					flag = false;
				}
			}

			if (flag)
			{
				//Button[0]?.Enable();
				Main.DEBUG_MISC_TEXT = flag;
			}
		}
	}
}
