using Microsoft.Xna.Framework;

namespace VisualBoy.UI
{
	public class UIMouseEvent : UIEvent
	{
		public readonly Point MousePosition;

		public UIMouseEvent(UIElement target, Point mousePosition) : base(target)
		{
			MousePosition = mousePosition;
		}
	}
}
