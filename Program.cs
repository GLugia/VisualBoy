﻿using System;

namespace VisualBoy
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			using (Main game = new Main())
			{
				game.Run();
			}
		}
	}
}
