using VisualBoy.Core.Modules.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace VisualBoy.Core.Modules.Tiles
{
	public class Activatable : Tile
	{
		public Sprite[] RequiredItems;
		public delegate void VoidEvent(ref Sprite parent);
		public event VoidEvent CustomFunction;
		public bool Activated;

		public override void Update(GameTime _, ref Sprite parent)
		{
			if (!Activated && Main.Player.IsNextTo(parent) && Main.kbState.IsKeyDown(Keys.Space))
			{
				bool CanActivate = true;

				if (RequiredItems != null)
				{
					for (int i = 0; i < RequiredItems.Length; i++)
					{
						if (!Main.Player.GetModule<Player>().HasItem(RequiredItems[i]))
						{
							CanActivate = false;
							break;
						}
					}
				}

				if (CanActivate)
				{
					CustomFunction?.Invoke(ref parent);
					Activated = true;
				}
			}
		}
	}
}
