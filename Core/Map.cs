using BitBiter.IO;
using VisualBoy.Core.Modules.NPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection;

namespace VisualBoy.Core
{
	[DefaultMember("Empty")]
	public struct Map
	{
		public static readonly Map Empty = Create("", 0, 0);
		public Sprite[,] Tiles;
		public Sprite[] NPCs;
		public string Name;
		public int Width, Height;

		public static Map Load(string name)
		{
			SharpTag tag = BitIO.FromFile(Path.Combine("Maps", name), false);

			Map ret = new Map()
			{
				Name = tag.GetString("mapname"),
				Width = tag.GetInt("mapwidth"),
				Height = tag.GetInt("mapheight")
			};

			ret.Tiles = new Sprite[ret.Width, ret.Height];

			for (int i = 0; i < ret.Width; i++)
			{
				for (int j = 0; j < ret.Height; j++)
				{
					ret.Tiles[i, j] = Sprite.Load(tag.GetSharpTag($"maptile[{i},{j}]"));
				}
			}

			return ret;
		}

		public static Map Create(string name, int width, int height)
		{
			Map ret = new Map()
			{
				Name = name,
				Width = width,
				Height = height,
				Tiles = new Sprite[width, height],
				NPCs = new Sprite[255]
			};

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					ret.Tiles[i, j] = Sprite.Empty;
				}
			}

			for (int i = 0; i < 255; i++)
			{
				ret.NPCs[i] = Sprite.Empty;
			}

			return ret;
		}

		public static void Save(Map map)
		{
			SharpTag ret = new SharpTag()
			{
				{ "mapname", map.Name },
				{ "mapwidth", map.Width },
				{ "mapheight", map.Height }
			};

			if (map.Tiles == null)
			{
				map.Tiles = new Sprite[map.Width, map.Height];
			}

			for (int i = 0; i < map.Width; i++)
			{
				for (int j = 0; j < map.Height; j++)
				{
					ret.Add($"maptile[{i},{j}]", map.Tiles[i, j].Save());
				}
			}

			BitIO.ToFile(ret, Path.Combine("Maps", map.Name), false);
		}

		public void SpawnNPC(Sprite obj)
		{
			if (!obj.Contains<NPC>())
			{
				throw new ArgumentException($"Sprite does not contain the NPC module: {obj.Name}");
			}

			for (int i = 0; i < 255; i++)
			{
				if (!NPCs[i].Active)
				{
					NPCs[i] = obj;
					return;
				}
			}

			throw new Exception($"Not enough room for NPC '{obj.Name}'");
		}

		public void Update(GameTime time)
		{
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					Tiles[i, j].Update(time);
				}
			}

			if (NPCs != null)
			{
				for (int i = 0; i < 255; i++)
				{
					NPCs[i].Update(time);
				}
			}
		}

		public void Draw(SpriteBatch batch)
		{
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					Tiles[i, j].Draw(batch);
				}
			}

			if (NPCs != null)
			{
				for (int i = 0; i < 255; i++)
				{
					NPCs[i].Draw(batch);
				}
			}
		}

		public override int GetHashCode()
		{
			return Width.GetHashCode() * Height.GetHashCode() + Name.GetHashCode() + Tiles.GetHashCode();
		}

		public override string ToString()
		{
			return $"MAP {{ {Name}: {Width}x{Height} }}";
		}

		public override bool Equals(object obj)
		{
			return obj is Map map && map == this;
		}

		public static bool operator ==(Map left, Map right)
		{
			if (left.Name == right.Name && left.Width == right.Width && left.Height == right.Height)
			{
				return left.Tiles == right.Tiles;
			}

			return false;
		}

		public static bool operator !=(Map left, Map right)
		{
			return !(left == right);
		}
	}
}
