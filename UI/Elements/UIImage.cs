using VisualBoy.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.UI
{
	public class UIImage : UIElement
	{
		private string Image;
		public float Alpha;
		public float Scale;

		public UIImage(string imagePath)
		{
			Image = imagePath;
			Alpha = 1f;
			Scale = 1f;
		}

		public override void OnInitialize()
		{
			MediaCache.LoadTexture(Image, out string key);
			Texture2D temp = MediaCache.GetTexture(key);
			MinWidth.Set(temp.Width, 0f);
			MinHeight.Set(temp.Height, 0f);
			Image = key;
		}

		protected override void DrawSelf(SpriteBatch batch)
		{
			batch.Draw(MediaCache.GetTexture(Image), Dimensions.Position, null, new Color(1f, 1f, 1f, Alpha), 0f, Vector2.Zero, Scale, default, 0f);
		}
	}
}
