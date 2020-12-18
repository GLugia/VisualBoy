using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace VisualBoy.UI
{
	public class UserInterface
	{
		public UIState CurrentState { get; private set; }

		private const double DOUBLE_CLICK_TIME = 500.0;
		private const double STATE_CHANGE_CLICK_DISABLE_TIME = 200.0;
		private const int MAX_HISTORY_SIZE = 32;
		private const int HISTORY_PRUNE_SIZE = 4;

		public static UserInterface ActiveInstance = new UserInterface();

		private readonly List<UIState> History = new List<UIState>();

		public Point MousePosition;
		private bool WasMouseLeftDown, WasMouseRightDown, WasMouseMiddleDown;
		private UIElement LastElementHover, LastElementLeftDown, LastElementRightDown,
			LastElementMiddleDown, LastElementLeftClicked, LastElementRightClicked, LastElementMiddleClicked;
		private double LastMouseLeftDownTime, LastMouseRightDownTime, LastMouseMiddleDownTime,
			ClickDisabledTimeRemaining;

		public bool IsVisible;

		public CalculatedStyle Dimensions => new CalculatedStyle(0f, 0f, Main.ScreenRect.Width, Main.ScreenRect.Height);

		public UserInterface()
		{
			ActiveInstance = this;
		}

		public void Use()
		{
			if (ActiveInstance != this)
			{
				ActiveInstance = this;
				Recalculate();
				return;
			}

			ActiveInstance = this;
		}

		public void ResetLasts()
		{
			LastElementHover = null;
			LastElementLeftDown = null;
			LastElementLeftClicked = null;
			LastElementRightDown = null;
			LastElementRightClicked = null;
		}

		private void ResetState()
		{
			MousePosition = new Point(Main.MouseX, Main.MouseY);
			WasMouseLeftDown = Main.MouseLeft;
			WasMouseRightDown = Main.MouseRight;
			WasMouseMiddleDown = Main.MouseMiddle;

			if (LastElementHover != null)
			{
				LastElementHover.MouseOut(new UIMouseEvent(LastElementHover, MousePosition));
			}

			LastElementHover = null;
			LastElementLeftDown = null;
			LastElementRightDown = null;
			LastElementLeftClicked = null;
			LastElementRightClicked = null;
			LastMouseLeftDownTime = 0.0;
			LastMouseRightDownTime = 0.0;
			LastMouseMiddleDownTime = 0.0;
			ClickDisabledTimeRemaining = Math.Max(ClickDisabledTimeRemaining, STATE_CHANGE_CLICK_DISABLE_TIME);
		}

		public void Update(GameTime time)
		{
			if (CurrentState == null)
			{
				return;
			}

			MousePosition = new Point(Main.MouseX, Main.MouseY);
			bool left = Main.MouseLeft && Main.HasFocus;
			bool right = Main.MouseRight && Main.HasFocus;
			bool middle = Main.MouseMiddle && Main.HasFocus;
			UIElement element = Main.HasFocus ? CurrentState.GetElementAt(MousePosition) : null;
			ClickDisabledTimeRemaining = Math.Max(0.0, ClickDisabledTimeRemaining - time.ElapsedGameTime.TotalMilliseconds);

			if (element != LastElementHover)
			{
				LastElementHover?.MouseOut(new UIMouseEvent(LastElementHover, MousePosition));
				element?.MouseOver(new UIMouseEvent(element, MousePosition));
				LastElementHover = element;
			}

			if (element != null && ClickDisabledTimeRemaining <= 0.0)
			{
				if (left && !WasMouseLeftDown)
				{
					LastElementLeftDown = element;
					element.LeftDown(new UIMouseEvent(element, MousePosition));

					if (LastElementLeftClicked == element && time.TotalGameTime.TotalMilliseconds - LastMouseLeftDownTime < DOUBLE_CLICK_TIME)
					{
						element.LeftDoubleClick(new UIMouseEvent(element, MousePosition));
						LastElementLeftClicked = null;
					}

					LastMouseLeftDownTime = time.TotalGameTime.TotalMilliseconds;
				}
				else if (!left && WasMouseLeftDown)
				{
					if (LastElementLeftDown.ContainsPoint(MousePosition))
					{
						LastElementLeftDown.LeftClick(new UIMouseEvent(LastElementLeftDown, MousePosition));
						LastElementLeftClicked = LastElementLeftDown;
					}

					LastElementLeftDown?.LeftUp(new UIMouseEvent(LastElementLeftDown, MousePosition));
					LastElementLeftDown = null;
				}

				if (right && !WasMouseRightDown)
				{
					LastElementRightDown = element;
					element.RightDown(new UIMouseEvent(element, MousePosition));

					if (LastElementRightClicked == element && time.TotalGameTime.TotalMilliseconds - LastMouseRightDownTime < DOUBLE_CLICK_TIME)
					{
						element.RightDoubleClick(new UIMouseEvent(element, MousePosition));
						LastElementRightClicked = null;
					}

					LastMouseRightDownTime = time.TotalGameTime.TotalMilliseconds;
				}
				else if (!right && WasMouseRightDown)
				{
					if (LastElementRightDown.ContainsPoint(MousePosition))
					{
						LastElementRightDown.RightClick(new UIMouseEvent(LastElementRightDown, MousePosition));
						LastElementRightClicked = LastElementRightDown;
					}

					LastElementRightDown.RightUp(new UIMouseEvent(LastElementRightDown, MousePosition));
					LastElementRightDown = null;
				}

				if (middle && !WasMouseMiddleDown)
				{
					LastElementMiddleDown = element;
					element.MiddleDown(new UIMouseEvent(element, MousePosition));

					if (LastElementMiddleClicked == element && time.TotalGameTime.TotalMilliseconds - LastMouseMiddleDownTime < DOUBLE_CLICK_TIME)
					{
						element.MiddleDoubleClick(new UIMouseEvent(element, MousePosition));
						LastElementMiddleClicked = null;
					}

					LastMouseMiddleDownTime = time.TotalGameTime.TotalMilliseconds;
				}
				else if (!middle && WasMouseMiddleDown)
				{
					if (LastElementMiddleDown.ContainsPoint(MousePosition))
					{
						LastElementMiddleDown.MiddleClick(new UIMouseEvent(LastElementMiddleDown, MousePosition));
						LastElementMiddleClicked = LastElementMiddleDown;
					}

					LastElementMiddleDown.MiddleUp(new UIMouseEvent(LastElementMiddleDown, MousePosition));
					LastElementMiddleDown = null;
				}

				if (Main.mouseState.ScrollWheelValue != 0)
				{
					element.ScrollWheel(new UIScrollWheelEvent(element, MousePosition, Main.mouseState.ScrollWheelValue));
				}
			}

			WasMouseLeftDown = left;
			WasMouseRightDown = right;
			WasMouseMiddleDown = middle;
			CurrentState?.Update(time);
		}

		public void Draw(SpriteBatch batch)
		{
			Use();
			CurrentState?.Draw(batch);
		}

		public void SetState(UIState state)
		{
			if (state != null)
			{
				AddToHistory(state);
			}

			CurrentState?.Deactivate();
			CurrentState = state;
			ResetState();
			state?.Activate();
			state?.Recalculate();
		}

		public void GoBack()
		{
			if (History.Count >= 2)
			{
				UIState state = History[History.Count - 2];
				History.RemoveRange(History.Count - 2, 2);
				SetState(state);
			}
		}

		private void AddToHistory(UIState state)
		{
			History.Add(state);
			if (History.Count > MAX_HISTORY_SIZE)
			{
				History.RemoveRange(0, HISTORY_PRUNE_SIZE);
			}
		}

		public void Recalculate()
		{
			CurrentState?.Recalculate();
		}

		internal void RefreshState()
		{
			CurrentState?.Deactivate();
			ResetState();
			CurrentState?.Activate();
			CurrentState?.Recalculate();
		}

		public bool IsElementHovered()
		{
			return IsVisible && LastElementHover != null && !(LastElementHover is UIState);
		}
	}
}
