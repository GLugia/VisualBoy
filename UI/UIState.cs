namespace VisualBoy.UI
{
	public class UIState : UIElement
	{
		public UIState()
		{
			Width.Percent = 1f;
			Height.Percent = 1f;
			MaxWidth.Set(Main.Graphics.PreferredBackBufferWidth, 1f);
			MaxHeight.Set(Main.Graphics.PreferredBackBufferHeight, 1f);
			Recalculate();
		}
	}
}
