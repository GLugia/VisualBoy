using VisualBoy.UI.Elements;
using Microsoft.Xna.Framework;

namespace VisualBoy.UI.States
{
	public class UIMainMenu : UIState
	{
		private readonly UIText[] Buttons;

		public UIMainMenu() : base()
		{
			Buttons = new UIText[]
			{
				new UIText("Load", 0.75f, true),
				new UIText("New", 0.75f, true),
				new UIText("Options", 0.75f, true),
				new UIText("Map Maker", 0.75f, true),
				new UIText("Quit", 0.75f, true)
			};

			for (int i = 0; i < Buttons.Length; i++)
			{
				Buttons[i].TextColor = Color.White;
				Buttons[i].HAlign = 0.5f;
				Buttons[i].VAlign = 0.75f + (((Buttons.Length / 75f) * Buttons[i].TextScale) * i);
				Buttons[i].OnMouseOver += StartHover;
				Buttons[i].OnMouseOut += StopHover;
				Buttons[i].OnLeftClick += DoClick;
				Append(Buttons[i]);
			}
		}

		private void StartHover(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIText)listeningElement).TextScale = 0.85f;
			listeningElement.Recalculate();
		}

		private void StopHover(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIText)listeningElement).TextScale = 0.75f;
			listeningElement.Recalculate();
		}

		private void DoClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (listeningElement is UIText text)
			{
				switch (text.Text)
				{
					case "Map Maker": Main.instance.MenuUI.SetState(new UIMapMaker()); break;
					case "Quit": Main.instance.Exit(); break;
					default: break;
				}
			}
			else
			{
				Main.DEBUG_MISC_TEXT = "Clicked: N/a";
			}
		}
	}
}
