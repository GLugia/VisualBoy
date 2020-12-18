using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace VisualBoy.UI
{
	public class UIElement : IComparable
	{
		#region Variables

		internal bool MouseHovering;
		public bool IsMouseHovering => MouseHovering;

		public string ID = "";
		public UIElement Parent;
		protected List<UIElement> Elements = new List<UIElement>();

		public StyleDimension Top, Left, Width;
		public StyleDimension MinWidth, MaxWidth, Height, MinHeight, MaxHeight;

		private bool Initialized;

		public float PaddingTop, PaddingLeft, PaddingRight, PaddingBottom;
		public float MarginTop, MarginLeft, MarginRight, MarginBottom;

		public float HAlign, VAlign;

		public CalculatedStyle InnerDimensions, OuterDimensions;
		public CalculatedStyle Dimensions = new CalculatedStyle(0f, 0f, Main.ScreenRect.Width, Main.ScreenRect.Height);

		#endregion

		#region ActivationMethods

		public void Activate()
		{
			if (!Initialized)
			{
				Initialize();
			}
			OnActivate();
			foreach (UIElement element in Elements)
			{
				element.Activate();
			}
			Recalculate();
		}

		public virtual void OnActivate() { }

		public void Deactivate()
		{
			OnDeactivate();
			foreach (UIElement element in Elements)
			{
				element.Deactivate();
			}
		}

		public virtual void OnDeactivate() { }

		public void Initialize()
		{
			OnInitialize();
			Initialized = true;
		}

		public virtual void OnInitialize() { }

		#endregion

		protected virtual void DrawSelf(SpriteBatch batch) { }
		protected virtual void DrawChildren(SpriteBatch batch)
		{
			foreach (UIElement element in Elements)
			{
				element.Draw(batch);
			}
		}

		public void Append(UIElement element)
		{
			element.Remove();
			element.Parent = this;
			element.MaxWidth.Set(Width.Pixels, 1f);
			element.MaxHeight.Set(Width.Pixels, 1f);
			Elements.Add(element);
			element.Recalculate();
		}

		public void Remove()
		{
			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}
		}

		public void RemoveChild(UIElement child)
		{
			Elements.Remove(child);
			child.Parent = null;
		}

		public void RemoveAllChildren()
		{
			foreach (UIElement element in Elements)
			{
				element.Parent = null;
			}

			Elements.Clear();
		}

		public bool HasChild(UIElement child)
		{
			return Elements.Contains(child);
		}

		public virtual void Draw(SpriteBatch batch)
		{
			DrawSelf(batch);
			DrawChildren(batch);
		}

		public virtual void Update(GameTime time)
		{
			foreach (UIElement element in Elements)
			{
				element.Update(time);
			}
		}

		public virtual void Recalculate()
		{
			CalculatedStyle calculatedStyle = Parent?.InnerDimensions ?? UserInterface.ActiveInstance.Dimensions;
			CalculatedStyle calculatedStyle2 = default;

			calculatedStyle2.X = Left.GetValue(calculatedStyle.Width) + calculatedStyle.X;
			calculatedStyle2.Y = Top.GetValue(calculatedStyle.Height) + calculatedStyle.Y;
			float value = MinWidth.GetValue(calculatedStyle.Width);
			float value2 = MaxWidth.GetValue(calculatedStyle.Width);
			float value3 = MinHeight.GetValue(calculatedStyle.Height);
			float value4 = MaxHeight.GetValue(calculatedStyle.Height);
			calculatedStyle2.Width = MathHelper.Clamp(Width.GetValue(calculatedStyle.Width), value, value2);
			calculatedStyle2.Height = MathHelper.Clamp(Height.GetValue(calculatedStyle.Height), value3, value4);
			calculatedStyle2.Width += MarginLeft + MarginRight;
			calculatedStyle2.Height += MarginTop + MarginBottom;
			calculatedStyle2.X += calculatedStyle.Width * HAlign - calculatedStyle2.Width * HAlign;
			calculatedStyle2.Y += calculatedStyle.Height * VAlign - calculatedStyle2.Height * VAlign;
			OuterDimensions = calculatedStyle2;

			calculatedStyle2.X += MarginLeft;
			calculatedStyle2.Y += MarginTop;
			calculatedStyle2.Width -= MarginLeft + MarginRight;
			calculatedStyle2.Height -= MarginTop + MarginBottom;
			Dimensions = calculatedStyle2;

			calculatedStyle2.X += PaddingLeft;
			calculatedStyle2.Y += PaddingTop;
			calculatedStyle2.Width -= PaddingLeft + PaddingRight;
			calculatedStyle2.Height -= PaddingTop + PaddingBottom;
			InnerDimensions = calculatedStyle2;

			RecalculateChildren();
		}

		public virtual void RecalculateChildren()
		{
			foreach (UIElement element in Elements)
			{
				element.Recalculate();
			}
		}

		public UIElement GetElementAt(Point point)
		{
			UIElement ret = null;

			foreach (UIElement element in Elements)
			{
				if (element.ContainsPoint(point))
				{
					ret = element;
					break;
				}
			}

			if (ret != null)
			{
				return ret.GetElementAt(point);
			}

			if (ContainsPoint(point))
			{
				return this;
			}

			return null;
		}

		public virtual bool ContainsPoint(Point point)
		{
			return point.X > Dimensions.X
				&& point.Y > Dimensions.Y
				&& point.X < Dimensions.X + Dimensions.Width
				&& point.Y < Dimensions.Y + Dimensions.Height;
		}

		public void SetPadding(float top, float bottom, float left, float right)
		{
			PaddingTop = top;
			PaddingBottom = bottom;
			PaddingLeft = left;
			PaddingRight = right;
		}

		public void SetMargin(float top, float bottom, float left, float right)
		{
			MarginTop = top;
			MarginBottom = bottom;
			MarginLeft = left;
			MarginRight = right;
		}

		public void CopyStyle(UIElement element)
		{
			Top = element.Top;
			Left = element.Left;
			Width = element.Width;
			Height = element.Height;
			PaddingTop = element.PaddingTop;
			PaddingBottom = element.PaddingBottom;
			PaddingLeft = element.PaddingLeft;
			PaddingRight = element.PaddingRight;
			MarginTop = element.MarginTop;
			MarginBottom = element.MarginBottom;
			MarginLeft = element.MarginLeft;
			MarginRight = element.MarginRight;
			HAlign = element.HAlign;
			VAlign = element.VAlign;
			MinWidth = element.MinWidth;
			MaxWidth = element.MaxWidth;
			MinHeight = element.MinHeight;
			MaxHeight = element.MaxHeight;
			Recalculate();
		}

		#region Events

		public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);
		public delegate void ScrollWheelEvent(UIScrollWheelEvent evt, UIElement listeningElement);

		public event MouseEvent OnLeftDown, OnLeftUp, OnLeftClick, OnLeftDoubleClick;
		public event MouseEvent OnRightDown, OnRightUp, OnRightClick, OnRightDoubleClick;
		public event MouseEvent OnMiddleDown, OnMiddleUp, OnMiddleClick, OnMiddleDoubleClick;
		public event MouseEvent OnMouseOver, OnMouseOut;
		public event ScrollWheelEvent OnScrollWheel;

		public virtual void LeftDown(UIMouseEvent evt)
		{
			OnLeftDown?.Invoke(evt, this);
			Parent?.LeftDown(evt);
		}

		public virtual void LeftUp(UIMouseEvent evt)
		{
			OnLeftUp?.Invoke(evt, this);
			Parent?.LeftDown(evt);
		}

		public virtual void LeftClick(UIMouseEvent evt)
		{
			OnLeftClick?.Invoke(evt, this);
			Parent?.LeftClick(evt);
		}

		public virtual void LeftDoubleClick(UIMouseEvent evt)
		{
			OnLeftDoubleClick?.Invoke(evt, this);
			Parent?.LeftDown(evt);
		}

		public virtual void RightDown(UIMouseEvent evt)
		{
			OnRightDown?.Invoke(evt, this);
			Parent?.RightDown(evt);
		}

		public virtual void RightUp(UIMouseEvent evt)
		{
			OnRightUp?.Invoke(evt, this);
			Parent?.RightUp(evt);
		}

		public virtual void RightClick(UIMouseEvent evt)
		{
			OnRightClick?.Invoke(evt, this);
			Parent?.RightClick(evt);
		}

		public virtual void RightDoubleClick(UIMouseEvent evt)
		{
			OnRightDoubleClick?.Invoke(evt, this);
			Parent?.RightDoubleClick(evt);
		}

		public virtual void MiddleDown(UIMouseEvent evt)
		{
			OnMiddleDown?.Invoke(evt, this);
			Parent?.MiddleDown(evt);
		}

		public virtual void MiddleUp(UIMouseEvent evt)
		{
			OnMiddleUp?.Invoke(evt, this);
			Parent?.MiddleUp(evt);
		}

		public virtual void MiddleClick(UIMouseEvent evt)
		{
			OnMiddleClick?.Invoke(evt, this);
			Parent?.MiddleClick(evt);
		}

		public virtual void MiddleDoubleClick(UIMouseEvent evt)
		{
			OnMiddleDoubleClick?.Invoke(evt, this);
			Parent?.MiddleDoubleClick(evt);
		}

		public virtual void MouseOver(UIMouseEvent evt)
		{
			MouseHovering = true;
			OnMouseOver?.Invoke(evt, this);
			Parent?.MouseOver(evt);
		}

		public virtual void MouseOut(UIMouseEvent evt)
		{
			MouseHovering = false;
			OnMouseOut?.Invoke(evt, this);
			Parent?.MouseOut(evt);
		}

		public virtual void ScrollWheel(UIScrollWheelEvent evt)
		{
			OnScrollWheel?.Invoke(evt, this);
			Parent?.ScrollWheel(evt);
		}

		#endregion

		public virtual int CompareTo(object obj)
		{
			return 0;
		}
	}
}
