using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.Core.Modules
{
	public class Image : Module
	{
		public override void Draw(SpriteBatch batch, ref Sprite parent)
		{
			batch.Draw(MediaCache.GetTexture(parent.Texture), new Rectangle((int)parent.Position.X, (int)parent.Position.Y, parent.Width, parent.Height), Color.White);
		}
	}
}
