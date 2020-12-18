namespace VisualBoy.UI.States
{
	public class UISplash : UIState
	{
		/*public bool Complete => !FadingIn && !FadingOut;
		private bool FadingIn;
		private bool FadingOut;
		private UIImage image;

		public override void OnInitialize()
		{
			image = new UIImage("test");
			image.Left.Set(0f, 0f);
			image.Top.Set(0f, 0f);
			//image.MaxWidth.Set(Width.Pixels, 1f);
			//image.MaxHeight.Set(Height.Pixels, 1f);
			image.Width.Set(Width.Pixels, 1f);
			image.Height.Set(Height.Pixels, 1f);
			image.Alpha = 0f;
			Append(image);
			FadingIn = true;
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			batch.DrawString(Main.Xiq12, $"In: {FadingIn} - Out: {FadingOut} - Alpha: {image?.Alpha ?? -1f}", Vector2.UnitY * 32, Color.White);
		}

		public override void Update(GameTime time)
		{
			base.Update(time);

			if (FadingIn)
			{
				if (image.Alpha >= 0.8f)
				{
					FadingIn = false;
					FadingOut = true;
					return;
				}

				if (image.Alpha < 1f)
				{
					image.Alpha += 1f / 240f;
				}
			}

			if (FadingOut)
			{
				if (image.Alpha <= 0f)
				{
					FadingOut = false;
					return;
				}

				if (image.Alpha > 0f)
				{
					image.Alpha -= 1f / 240f;
				}
			}
		}*/
	}
}
