namespace VisualBoy.Core.Modules.Items
{
	public enum ItemType : byte
	{
		MISC,
		WEAPON,
		ARMOR,
		SHIELD,
		ACCESSORY,
		USEABLE,
		KEY
	}

	public abstract class Item : Module
	{
		/// <summary>
		/// How much to add to the sprite's stats
		/// </summary>
		public int STRa, AGIa, INTa, STAa, LCKa;
		public int amount { get; internal set; }
		public ItemType ItemType { get; protected set; }

		public abstract void SetDefaults();
	}
}
