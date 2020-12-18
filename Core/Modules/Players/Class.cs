using Microsoft.Xna.Framework;

namespace VisualBoy.Core.Modules.Players
{
	public abstract class Class
	{
		public string Nameb => null;
		public string Nameu => null;
		public string Namec { get; internal set; }

		public string Texture { get; internal set; }
		public string TexturePathb => null;
		public string TexturePathu => null;
		public string TexturePathc { get; internal set; }

		public byte Level { get; internal set; }
		public short EXPn { get; internal set; }
		public short EXPc { get; internal set; }
		public byte STRb, AGIb, INTb, STAb, LCKb;
		public short STRc, AGIc, INTc, STAc, LCKc;

		public short ATK { get; internal set; }
		public short ACC { get; internal set; }
		public short DEF { get; internal set; }
		public short EVA { get; internal set; }

		public float LuckyLevelChance => 0f;

		public abstract void SetDefaults();

		public void Update(GameTime _)
		{
			UpdateStats();
			UpdateLevel();
		}

		private void UpdateStats()
		{

		}

		private void UpdateLevel()
		{
			if (EXPc >= EXPn)
			{
				EXPc = (short)(EXPc - EXPn);
				EXPn = (short)(EXPn * 1.01f);
				Level++;
			}
		}
	}
}
