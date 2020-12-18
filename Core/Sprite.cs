using BitBiter.IO;
using VisualBoy.Core.Extensions;
using VisualBoy.Core.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace VisualBoy.Core
{
	public struct Sprite
	{
		public int INSTANCE_HASH { get; private set; }

		public static Sprite Empty => new Sprite();

		public string Name;
		public string Texture;
		internal string TexturePath;
		public bool CheckPosition;
		public Point TilePosition;
		public Vector2 Position;
		public Vector2 Center => new Vector2(Position.X + Width / 2, Position.Y + Height / 2);
		public Vector2 Velocity;
		public int Width, Height;
		public bool Active;
		public bool ModulesActive { get; private set; }
		public Module[] Modules { get; private set; }
		public bool InBattle;

		public static Sprite New(string name, string texture, Vector2 position, int width = 32, int height = 32)
		{
			Sprite ret = new Sprite()
			{
				INSTANCE_HASH = Main.rand.Int32(0, int.MaxValue),
				Name = name,
				TexturePath = texture,
				Position = position,
				TilePosition = position.ToTileCoordinates(),
				Width = width,
				Height = height,
				Modules = new Module[0],
				Active = true
			};

			if (!string.IsNullOrEmpty(texture))
			{
				MediaCache.LoadTexture(texture, out ret.Texture);
			}

			return ret;
		}

		public T Append<T>() where T : Module
		{
			Append(out T instance);
			return instance;
		}

		public bool Append<T>(out T instance) where T : Module
		{
			if (Contains<T>())
			{
				instance = GetModule<T>();
				return false;
			}

			for (int i = 0; i < Modules.Length; i++)
			{
				if (Modules[i] == null)
				{
					Modules[i] = Activator.CreateInstance<T>();
					Modules[i].Load(ref this);
					instance = (T)Modules[i];
					return true;
				}
			}

			Module[] temp = Modules;
			Array.Resize(ref temp, Modules.Length + 1);
			Modules = temp;
			Modules[^1] = Activator.CreateInstance<T>();
			Modules[^1].Load(ref this);
			instance = (T)Modules[^1];
			return true;
		}

		public bool Remove<T>() where T : Module
		{
			if (!Contains<T>())
			{
				return false;
			}

			for (int i = 0; i < Modules.Length; i++)
			{
				if (Modules[i].GetType() == typeof(T))
				{
					Modules[i].Deactivate(ref this);
					Modules[i].Unload(ref this);
					Modules[i] = null;
					return true;
				}
			}

			return false;
		}

		public T GetModule<T>() where T : Module
		{
			foreach (Module module in Modules)
			{
				if (module == null)
				{
					continue;
				}

				if (module.GetType() == typeof(T))
				{
					return (T)module;
				}
			}

			return default;
		}

		public bool Contains<T>() where T : Module
		{
			if (Modules == null)
			{
				return false;
			}

			foreach (Module mod in Modules)
			{
				if (mod != null && mod.GetType() == typeof(T))
				{
					return true;
				}
			}

			return false;
		}

		public void Update(GameTime time)
		{
			if (!CanRun())
			{
				return;
			}

			Velocity = Vector2.Zero;

			foreach (Module module in Modules)
			{
#pragma warning disable IDE0059 // Unnecessary assignment of a value
				string modulename = module?.ToString();
#pragma warning restore IDE0059 // Unnecessary assignment of a value
				if ((module?.PreUpdate(time, ref this)).GetValueOrDefault())
				{
					module?.Update(time, ref this);
				}
			}

			if (Velocity != default)
			{
				if (float.IsNaN(Velocity.X) || float.IsInfinity(Velocity.X))
				{
					Velocity.X = 0f;
				}

				if (float.IsNaN(Velocity.Y) || float.IsInfinity(Velocity.Y))
				{
					Velocity.Y = 0f;
				}

				Position += Velocity;
			}
			else if (CheckPosition)
			{
				Position.RoundToTile();
				TilePosition = Position.ToTileCoordinates();
				CheckPosition = false;
			}

			foreach (Module module in Modules)
			{
				module?.PostUpdate(time, ref this);
			}

			//Main.DEBUG_MISC_TEXT = $"Position: {Position} ||| Velocity: {Velocity}";
		}

		public void Draw(SpriteBatch batch)
		{
			if (!CanRun())
			{
				return;
			}

			foreach (Module module in Modules)
			{
				if ((module?.PreDraw(batch, ref this)).GetValueOrDefault())
				{
					module?.Draw(batch, ref this);
				}
			}

			foreach (Module module in Modules)
			{
				module?.PostDraw(batch, ref this);
			}
		}

		private bool CanRun()
		{
			if (!Active)
			{
				if (ModulesActive)
				{
					foreach (Module module in Modules)
					{
						module.Deactivate(ref this);
					}

					ModulesActive = false;
				}

				return false;
			}

			if (!ModulesActive)
			{
				foreach (Module module in Modules)
				{
					module.Activate(ref this);
				}

				ModulesActive = true;
			}

			return true;
		}

		public static Sprite Load(SharpTag tag)
		{
			Sprite ret = new Sprite()
			{
				Name = tag.GetString("spritename"),
				TexturePath = tag.GetString("spritetexture"),
				Position = tag.GetVector2("spritepos"),
				Width = tag.GetInt("spritewidth"),
				Height = tag.GetInt("spriteheight"),
				Active = tag.GetBool("spriteactive"),
				Modules = new Module[tag.GetInt("spritemodlen")]
			};

			MediaCache.LoadTexture(ret.TexturePath, out ret.Texture);

			for (int i = 0; i < ret.Modules.Length; i++)
			{
				ret.Modules[i] = tag.GetModule($"spritemod{i}"); // probably doesn't work
			}

			return ret;
		}

		public SharpTag Save()
		{
			SharpTag ret = new SharpTag
			{
				{ "spritename", Name },
				{ "spritetexture", TexturePath },
				{ "spritepos", Position },
				{ "spritewidth", Width },
				{ "spriteheight", Height },
				{ "spriteactive", Active },
				{ "spritemodlen", Modules.Length }
			};

			for (int i = 0; i < Modules.Length; i++)
			{
				ret.Add($"spritemod{i}", Modules[i]);
			}

			return ret;
		}

		public bool IsNextTo(Sprite other)
		{
			Point diff = TilePosition - other.TilePosition;

			if (diff.X == 1 || diff.X == -1)
			{
				return diff.Y == 0;
			}

			if (diff.Y == 1 || diff.Y == -1)
			{
				return diff.X == 0;
			}

			return false;
		}

		public bool MovingToward(Sprite other)
		{
			Point diff = other.TilePosition - TilePosition;

			if (diff.Y == 0)
			{
				return (diff.X == 1 && Velocity.X > 0f) || (diff.X == -1 && Velocity.X < 0f);
			}
			else if (diff.X == 0)
			{
				return (diff.Y == 1 && Velocity.Y > 0f) || (diff.Y == -1 && Velocity.Y < 0f);
			}

			return false;
		}

		public static bool operator ==(Sprite left, Sprite right)
		{
			return left.INSTANCE_HASH == right.INSTANCE_HASH;
		}

		public static bool operator !=(Sprite left, Sprite right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is Sprite sprite && sprite == this;
		}

		public override int GetHashCode()
		{
			return INSTANCE_HASH.GetHashCode();
		}

		public override string ToString()
		{
			return $"SPRITE {{ {Name}, {Position}, {INSTANCE_HASH} }}";
		}
	}
}
