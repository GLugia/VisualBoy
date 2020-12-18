using BitBiter.IO;
using Microsoft.Xna.Framework.Input;

namespace VisualBoy.Core
{
	public static class Settings
	{
		public static float VolMaster { get; internal set; } = 0.5f;
		public static float VolMusic { get; internal set; } = 0.5f;
		public static float VolSound { get; internal set; } = 0.5f;

		public static Keys Left { get; internal set; } = Keys.A;
		public static Keys Right { get; internal set; } = Keys.D;
		public static Keys Up { get; internal set; } = Keys.W;
		public static Keys Down { get; internal set; } = Keys.S;
		public static Keys Activate { get; internal set; } = Keys.Space;
		public static Keys Cancel { get; internal set; } = Keys.Escape;
		public static Keys Start { get; internal set; } = Keys.Enter;
		public static Keys Select { get; internal set; } = Keys.LeftShift | Keys.RightShift;

		public static int WindowWidth { get; internal set; } = 480;
		public static int WindowHeight { get; internal set; } = 272;
		public static float WindowScale { get; internal set; } = 3.0f;

		internal static void Init()
		{
			VolMaster = VolMusic = VolSound = 0.5f;
			Left = Keys.A;
			Right = Keys.D;
			Up = Keys.W;
			Down = Keys.S;
			Activate = Keys.Space;
			Cancel = Keys.Escape;
			Start = Keys.Enter;
			Select = Keys.LeftShift | Keys.RightShift;
			WindowWidth = 480;
			WindowHeight = 272;
			WindowScale = 3.0f;
		}

		internal static void Load(SharpTag tag)
		{
			VolMaster = tag.GetFloat("va");
			VolMusic = tag.GetFloat("vm");
			VolSound = tag.GetFloat("vs");
			Left = (Keys)tag.GetInt("kl");
			Right = (Keys)tag.GetInt("kr");
			Up = (Keys)tag.GetInt("ku");
			Down = (Keys)tag.GetInt("kd");
			Activate = (Keys)tag.GetInt("ka");
			Cancel = (Keys)tag.GetInt("kb");
			Start = (Keys)tag.GetInt("kp");
			Select = (Keys)tag.GetInt("ks");
			WindowWidth = tag.GetInt("ww");
			WindowHeight = tag.GetInt("wh");
			WindowScale = tag.GetFloat("ws");
		}

		internal static SharpTag Save()
		{
			return new SharpTag
			{
				{ "va", VolMaster },
				{ "vm", VolMusic },
				{ "vs", VolSound },
				{ "kl", Left },
				{ "kr", Right },
				{ "ku", Up },
				{ "kd", Down },
				{ "ka", Activate },
				{ "kb", Cancel },
				{ "kp", Start },
				{ "ks", Select },
				{ "ww", WindowWidth },
				{ "wh", WindowHeight },
				{ "ws", WindowScale }
			};
		}
	}
}
