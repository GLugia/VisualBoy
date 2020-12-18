using VisualBoy.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VisualBoy.Core
{
	public static class FrameWatcher
	{
		public static long TotalFrames { get; private set; } = 0;
		public static long TotalUpdates { get; private set; } = 0;
		public static short CurrentFrames { get; private set; } = 0;
		public static short CurrentUpdates { get; private set; } = 0;
		public static double FPS { get; private set; } = 0.0d;
		
		private static double Last = 0.0d;
		private static double Now = 0.0d;
		private static double Elapsed = 0.0d;

		internal static void Update(GameTime time)
		{
			Now = time.TotalGameTime.TotalSeconds;
			Elapsed = Now - Last;

			if (Elapsed > 0.1d || CurrentUpdates == short.MaxValue)
			{
				FPS = CurrentFrames / Elapsed;

				Elapsed = 0.0d;
				CurrentFrames = 0;
				CurrentUpdates = 0;
				Last = Now;
			}

			CurrentUpdates++;
			TotalUpdates++;
		}

		internal static void Draw(SpriteBatch batch)
		{
			string avg = Math.Floor(FPS).ToString();
			Vector2 measure = Main.Xiq12.MeasureString(avg) * 0.4f;
			Vector2 pos = new Vector2(Main.ScreenRect.Width - measure.X, 0f);
			batch.DrawBorderedString(Main.Xiq12, avg, pos, Color.White, 0.4f);

			CurrentFrames++;
			TotalFrames++;
		}
	}
}
