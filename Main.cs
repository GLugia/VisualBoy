using BitBiter;
using BitBiter.IO;
using VisualBoy.Core;
using VisualBoy.Core.Extensions;
using VisualBoy.Core.Modules;
using VisualBoy.Core.Modules.NPCs;
using VisualBoy.Core.Modules.Players;
using VisualBoy.Core.Modules.Tiles;
using VisualBoy.UI;
using VisualBoy.UI.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualBoy
{
	public class Main : Game
	{
		#region Variables

		public static Main instance;
		private static SharpTag RootTag;
		public static string RootPath, AssetPath;
		public static GraphicsDeviceManager Graphics { get; private set; }
		public static SpriteBatch SpriteBatch { get; private set; }
		public static bool HasFocus => instance.IsActive;

		public static MouseState mouseState;
		public static bool MouseLeft, MouseMiddle, MouseRight;
		public static bool MouseLeftReleased, MouseMiddleReleased, MouseRightReleased;
		public static int MouseX, MouseY;

		public static KeyboardState kbState;

		internal readonly UserInterface MenuUI = new UserInterface();

		private static Camera2D Camera;
		public static Vector2 MouseWorld => Vector2.Transform(new Vector2(MouseX, MouseY), Matrix.Invert(Camera.Transform));
		public static Vector2 MouseScreen => Vector2.Transform(new Vector2(MouseX, MouseY), Camera.Transform);
		public static Sprite Player;
		public static Map currentmap;
		public static Map[] bufferedmaps;
		public static BitRandom rand;

		public static SpriteFont Xiq12;

		public static Rectangle ScreenRect => new Rectangle(0, 0, Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);

		public static object DEBUG_MEDIA_TEXT = "", DEBUG_MISC_TEXT = "";

		#endregion

		public Main() : base()
		{
			Graphics = new GraphicsDeviceManager(this)
			{
				GraphicsProfile = GraphicsProfile.HiDef
			};

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			instance = this;
			RootPath = Directory.GetCurrentDirectory();
			AssetPath = Path.Combine(Path.Combine(Directory.GetCurrentDirectory(), "Content"), "assets");
		}

		#region Init

		protected override void Initialize()
		{
			try
			{
				RootTag = BitIO.FromFile("ff.dat", false);
			}
			catch (Exception e)
			{
				string message = e.Message;
				BitIO.ToFile(new SharpTag(), "ff.dat", false);
				RootTag = BitIO.FromFile("ff.dat", false);
			}

			this.IsFixedTimeStep = true;
			if (RootTag.ContainsKey("SETTINGS"))
			{
				Settings.Load(RootTag.GetSharpTag("SETTINGS"));
			}
			else
			{
				Settings.Init();
			}
			Graphics.PreferredBackBufferWidth = (int)(Settings.WindowWidth * Settings.WindowScale);
			Graphics.PreferredBackBufferHeight = (int)(Settings.WindowHeight * Settings.WindowScale);
			Graphics.ApplyChanges();

#if DEBUG
			Window.Title = "(DEBUG) Final Fantasy: Dawn of Souls";
#else
			Window.Title = "Final Fantasy: Dawn of Souls";
#endif
			Window.AllowUserResizing = false;
			Window.AllowAltF4 = false;

			MediaCache.Init(GraphicsDevice);
			InitDefaultMedia();
			InitVariables();
			base.Initialize();
		}

		private void InitDefaultMedia()
		{
			Texture2D tex = new Texture2D(GraphicsDevice, 1, 1)
			{
				Name = "pixel"
			};
			tex.SetData(new Color[] { Color.White });
			MediaCache.AddTexture(tex);
		}

		private void InitVariables()
		{
			rand = new BitRandom();
			/*
			{
				Player = Sprite.New("player_base", "NPC/player_thief", Vector2.Zero, 32, 32); // @TODO load from root
				Player.Append<Player>();
				Player.Append<Input>().Enable();
				Player.Append<Animated>().SetIdleFrames(0).SetDownFrames(0, 1).SetUpFrames(2, 3).SetRightFrames(4, 5).SetLeftFrames(6, 7);
				Player.Append<Collision>();

				currentmap = Map.Create("Test", 5, 5);

				for (int i = 0; i < 5; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						currentmap.Tiles[i, j] = Sprite.New($"test{{{i},{j}}}", "test", new Vector2(i, j) * 32, 32, 32);
						//currentmap.Tiles[i, j].Append<Image>();

						if (i == 2 && j == 2)
						{
							currentmap.Tiles[i, j].Append<Image>();
							currentmap.Tiles[i, j].Append<Collision>();
						}
					}
				}
				/*
				Sprite guard = Sprite.New("Guard1", "NPC/guard", new Vector2(3, 3) * 32);
				guard.Append<NPC>();
				guard.Append<TownAI>();
				guard.Append<Collision>();
				guard.Append<Animated>().SetIdleFrames(0).SetDownFrames(0, 1).SetUpFrames(2, 3).SetRightFrames(5, 4).SetLeftFrames(6, 7);
				currentmap.NPCs[0] = guard;
				
			}*/
		}

		#endregion

		#region Load

		protected override void LoadContent()
		{
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			Camera = new Camera2D(GraphicsDevice.Viewport)
			{
				Zoom = Settings.WindowScale
			};
			LoadOurContent();
			LoadOurStates();
			LoadOurSaves();
		}

		private void LoadOurContent()
		{
			Xiq12 = Content.Load<SpriteFont>("Font/File");
		}

		private void LoadOurStates()
		{
			MenuUI.SetState(new UIMainMenu());
			//((UIMapMaker)MenuUI.CurrentState)
		}

		private void LoadOurSaves()
		{

		}

		#endregion

		#region Unload

		protected override void UnloadContent()
		{
			RootTag.Add("SETTINGS", Settings.Save());
			BitIO.ToFile(RootTag, "ff.dat", false);
			MediaCache.UnloadAll();
		}

		#endregion

		#region Update

		protected override void Update(GameTime time)
		{
			if (FrameWatcher.TotalFrames < 120 || !HasFocus)
			{
				return;
			}

			mouseState = Mouse.GetState(Window);
			kbState = Keyboard.GetState();

			MouseX = mouseState.X;
			MouseY = mouseState.Y;

			MouseLeftReleased = MouseLeft && mouseState.LeftButton == ButtonState.Released;
			MouseRightReleased = MouseRight && mouseState.RightButton == ButtonState.Released;
			MouseMiddleReleased = MouseMiddle && mouseState.MiddleButton == ButtonState.Released;

			Player.Update(time);
			currentmap.Update(time);
			Camera.Position = Player.Center;
			UpdateUI(time);

			MouseLeft = mouseState.LeftButton == ButtonState.Pressed;
			MouseRight = mouseState.RightButton == ButtonState.Pressed;
			MouseMiddle = mouseState.MiddleButton == ButtonState.Pressed;

			FrameWatcher.Update(time);

			base.Update(time);
		}

		private void UpdateUI(GameTime time)
		{
			if (UserInterface.ActiveInstance == null)
			{
				return;
			}

			UserInterface.ActiveInstance?.Update(time);
		}

		#endregion

		#region Draw

		private string loading = "Loading";
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			if (FrameWatcher.TotalFrames < 120)
			{
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
				Vector2 measure = Xiq12.MeasureString(loading) * 1.5f;
				SpriteBatch.DrawBorderedString(Xiq12, loading, ScreenRect.Center.ToVector2() - measure / 2f, Color.White, 1.5f);
				FrameWatcher.Draw(SpriteBatch);
				if (FrameWatcher.TotalFrames % 30 == 0)
				{
					loading += ".";

					if (loading.Contains("...."))
					{
						loading = "Loading";
					}
				}
				SpriteBatch.End();
				return;
			}

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Camera.Transform);
			currentmap.Draw(SpriteBatch);
			Player.Draw(SpriteBatch);
			SpriteBatch.End();

			SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
			DrawUI(SpriteBatch);
			SpriteBatch.DrawBorderedString(Xiq12, DEBUG_MEDIA_TEXT.ToString(), default, Color.White, 0.4f);
			//DEBUG_MISC_TEXT = Player.Velocity;
			SpriteBatch.DrawBorderedString(Xiq12, DEBUG_MISC_TEXT.ToString(), Vector2.UnitY * 12f, Color.White, 0.4f);
			FrameWatcher.Draw(SpriteBatch);
			SpriteBatch.End();

			base.Draw(gameTime);

			ResetElapsedTime();
		}

		private void DrawUI(SpriteBatch batch)
		{
			if (UserInterface.ActiveInstance == null)
			{
				return;
			}

			UserInterface.ActiveInstance?.Draw(batch);
		}

		#endregion

		#region Handlers

		#endregion
	}
}
