namespace VisualBoy.Core
{
	public struct Class
	{
		public string Texture;
		public string NAMEc;
		public string NAMEb;
		public string NAMEu;

		public byte Magic_Level;

		public byte Level;
		public float StrongLevelChance;

		public short EXPc;
		public short EXPn;
		public double EXPb;

		public short HPm;
		public short HPc;
		public byte HPb;

		public short MPm;
		public short MPc;
		public byte MPb;

		public short STRc;
		public byte STRb;
		public short AGLc;
		public byte AGLb;
		public short INTc;
		public byte INTb;
		public short STAc;
		public byte STAb;
		public short LCKc;
		public byte LCKb;

		public short ATK;
		public short ACC;
		public short DEF;
		public short EVA;

		//public Item[] equips;

		private void GenerateStats()
		{
			HPm = (short)((1 + (HPb / 4)) * (Level / 100));

			if (HPm > 9999)
			{
				HPm = 9999;
			}
		}
	}
}
