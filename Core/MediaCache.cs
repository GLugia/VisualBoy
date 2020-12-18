using BitBiter.Extensions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace VisualBoy.Core
{
	public static class MediaCache
	{
		private static GraphicsDevice Graphics;
		private static IDictionary<string, Texture2D> textures;
		private static IDictionary<string, DynamicSoundEffectInstance> songs; // @TODO manage sound????
		private static string CurrentSong;
		private static IDictionary<string, SoundEffect> sounds;

		internal static void Init(GraphicsDevice graphics)
		{
			Graphics = graphics;
			textures = new Dictionary<string, Texture2D>();
			songs = new Dictionary<string, DynamicSoundEffectInstance>();
			CurrentSong = "";
			sounds = new Dictionary<string, SoundEffect>();
		}

		#region Textures

		public static void LoadTexture(string filename, out string key)
		{
			if (string.IsNullOrEmpty(filename))
			{
				throw new NullReferenceException("File name cannot be null");
			}

			if (textures.TryGetValue(filename.ToLowerInvariant(), out Texture2D _))
			{
				key = filename.ToLowerInvariant();
				return;
			}

			Stopwatch sw = Stopwatch.StartNew();
			string path = "";

			foreach (string file in Directory.EnumerateFiles(Main.AssetPath, filename + ".*"))
			{
				if (file != null)
				{
					path = file;
					break;
				}
			}

			if (path == "")
			{
				foreach (string folder in Directory.EnumerateDirectories(Main.AssetPath))
				{
					foreach (string file in Directory.EnumerateFiles(folder, filename + ".*"))
					{
						if (file != null)
						{
							path = file;
							break;
						}
					}
				}
			}

			if (path == "")
			{
				throw new FileNotFoundException($"Failed to find file in {Main.AssetPath}", filename);
			}

			key = filename.ToLowerInvariant();
			sw.Stop();

			if (textures.TryAdd(key, Texture2D.FromFile(Graphics, path)))
			{
				Main.DEBUG_MEDIA_TEXT = $"Loaded '{key}' from '{path}' in {sw.Elapsed.TotalMilliseconds}ms";
			}
			else
			{
				Main.DEBUG_MEDIA_TEXT = $"Failed to load '{key}' from '{path}' ({sw.Elapsed.TotalMilliseconds}ms)";
			}
		}

		public static void AddTexture(Texture2D tex)
		{
			tex.Name = tex.Name.ToLowerInvariant();
			textures.TryAdd(tex.Name, tex);
		}

		public static Texture2D GetTexture(string name)
		{
			name = name.ToLowerInvariant();

			if (textures.TryGetValue(name, out Texture2D tex))
			{
				return tex;
			}

			throw new KeyNotFoundException(name);
		}

		public static void UnloadTextures()
		{
			textures.Clear();
		}

		public static void UnloadTexture(string name)
		{
			textures.TryRemove(name);
		}

		#endregion

		#region Songs

		public static void LoadSong(string path)
		{
			string asm = Main.instance.GetType().Assembly.Location;
			string name = path;

			int lastsep = path.LastIndexOf(Path.DirectorySeparatorChar);
			if (lastsep != -1)
			{
				name = name.Substring(lastsep, name.Length);
			}

			int lastdot = name.LastIndexOf('.');
			if (lastdot != -1)
			{
				name = name.Substring(0, lastdot);
			}

			DynamicSoundEffectInstance song = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo)
			{
				IsLooped = true
			};

			songs.TryAdd(name.ToLowerInvariant(), song);
		}

		public static void UnloadSongs()
		{
			songs.Clear();
		}

		public static void UnloadSong(string name)
		{
			songs.TryRemove(name);
		}

		public static void PlaySong(string name)
		{
			name = name.ToLowerInvariant();

			StopSong();

			if (songs.TryGetValue(name, out DynamicSoundEffectInstance song) && song != null)
			{
				if (song.State == (SoundState.Playing | SoundState.Paused))
				{
					song.Stop();
				}

				CurrentSong = name;
				song.Play();
			}
		}

		public static void PauseSong()
		{
			if (songs.TryGetValue(CurrentSong, out DynamicSoundEffectInstance song) && song != null && song.State == SoundState.Playing)
			{
				song.Pause();
			}
		}

		public static void StopSong()
		{
			if (songs.TryGetValue(CurrentSong, out DynamicSoundEffectInstance song) && song != null && song.State == (SoundState.Playing | SoundState.Paused))
			{
				song.Stop();
			}
		}

		#endregion

		#region Sounds

		public static void LoadSound(string path)
		{
			string asm = Main.instance.GetType().Assembly.Location;
			string name = path;

			int lastsep = path.LastIndexOf(Path.DirectorySeparatorChar);
			if (lastsep != -1)
			{
				name = name.Substring(lastsep, name.Length);
			}

			int lastdot = name.LastIndexOf('.');
			if (lastdot != -1)
			{
				name = name.Substring(0, lastdot);
			}

			sounds.TryAdd(name.ToLowerInvariant(), SoundEffect.FromFile(Path.Combine(asm, path)));
		}

		public static void UnloadSounds()
		{
			sounds.Clear();
		}

		public static void UnloadSound(string name)
		{
			sounds.TryRemove(name);
		}

		public static void PlaySound(string name)
		{
			if (sounds.TryGetValue(name.ToLowerInvariant(), out SoundEffect sound) && sound != null)
			{
				sound.CreateInstance().Play();
			}
		}

		#endregion

		public static void UnloadAll()
		{
			UnloadTextures();
			UnloadSongs();
			UnloadSounds();
		}
	}
}
