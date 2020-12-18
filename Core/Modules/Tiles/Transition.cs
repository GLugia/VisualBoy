using Microsoft.Xna.Framework;
using System;

namespace VisualBoy.Core.Modules.Tiles
{
	public class Transition : Tile
	{
		public Func<Sprite, bool> CustomFunction;
		public string TargetMap;

		public override void Update(GameTime _, ref Sprite parent)
		{
			if ((CustomFunction != null && CustomFunction.Invoke(Main.Player))
				|| (CustomFunction == null && Main.Player.Position == parent.Position))
			{
				//MapHandler.LoadMap(TargetMap);
			}
		}
	}
}
