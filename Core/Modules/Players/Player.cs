using VisualBoy.Core.Modules.Items;
using System;

namespace VisualBoy.Core.Modules.Players
{
	public class Player : Module
	{
		public Sprite[] inventory { get; private set; }
		public Sprite[] equips { get; private set; }
		public Class[] classes { get; private set; }

		public override void Activate(ref Sprite parent)
		{
			inventory = new Sprite[32];
			equips = new Sprite[5];
			classes = new Class[4];
		}

		public override void Deactivate(ref Sprite parent)
		{
			inventory = null;
			classes = null;
		}

		public bool PutItemInInventory(Sprite sprite)
		{
			if (sprite.GetModule<Item>() == null)
			{
				throw new NullReferenceException($"{sprite} is not an item");
			}

			for (int i = 0; i < inventory.Length; i++)
			{
				if (inventory[i].Name == sprite.Name)
				{
					inventory[i].GetModule<Item>().amount++;
					return true;
				}
			}

			for (int i = 0; i < inventory.Length; i++)
			{
				if (inventory[i] == Sprite.Empty)
				{
					inventory[i] = sprite;
					return true;
				}
			}

			return false;
		}

		public void Equip(Sprite sprite)
		{
			Item item = sprite.GetModule<Item>();

			if (item == null)
			{
				throw new NullReferenceException($"{sprite} is not an item");
			}

			switch (item.ItemType)
			{
				case ItemType.USEABLE:
				case ItemType.KEY:
				case ItemType.MISC: throw new InvalidCastException($"{sprite} is not equippable");
				default:
					{
						int type = (int)item.ItemType - 1;
						if (PutItemInInventory(equips[type]))
						{
							equips[type] = sprite;
							return;
						}

						break;
					}
			}
		}

		public bool HasItem(Sprite item)
		{
			foreach (Sprite slot in inventory)
			{
				if (slot == item)
				{
					return true;
				}
			}

			return false;
		}
	}
}
