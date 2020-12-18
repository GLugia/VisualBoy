using Microsoft.Xna.Framework;

namespace VisualBoy.UI
{
	public class UIScrollWheelEvent : UIMouseEvent
	{
		public readonly int ScrollWheelValue;

		public UIScrollWheelEvent(UIElement target, Point mousePosition, int scrollWheelValue) : base(target, mousePosition)
		{
			ScrollWheelValue = scrollWheelValue;
		}
	}
}
