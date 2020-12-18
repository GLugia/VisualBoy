using BitBiter.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VisualBoy.Core.Modules
{
	/// <summary>
	/// Used in the base <see cref="Sprite"/> class.<para/>
	/// Extending this class, then calling <see cref="Sprite.Append{T}"/> allows for anything to be done to that parent from within a module.<para/>
	/// <see cref="Sprite"/> also contains a method -- <see cref="Sprite.GetModule{T}"/>
	/// -- that allows modifying anything available within that module. <para/>
	/// Modules can also be removed using <see cref="Sprite.Remove{T}"/>
	/// </summary>
	public abstract class Module
	{
		/// <summary>
		/// For initializing and loading variables.
		/// Do not initialize variables in a constructor if it requires <see cref="Parent"/> as it will be null.
		/// </summary>
		public virtual void Load(ref Sprite parent) { }
		/// <summary>
		/// For nullifying and unloading variables.
		/// Do not null required variables in <see cref="Deactivate"/> unless you reinitialize them in <seealso cref="Activate"/>
		/// </summary>
		public virtual void Unload(ref Sprite parent) { }
		/// <summary>
		/// Called when this module is activated which only happens when the parent sprite is made active.
		/// </summary>
		/// <param name="parent"></param>
		public virtual void Activate(ref Sprite parent) { }
		/// <summary>
		/// Called when this module is deactivated which only happens when the parent sprite is made inactive.
		/// </summary>
		/// <param name="parent"></param>
		public virtual void Deactivate(ref Sprite parent) { }

		/// <summary>
		/// Use this method to determine if the sprite should update. This method is called before any other methods.
		/// Return false to keep <see cref="Update(GameTime, ref Sprite)"/> from being called. Returns true by default.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public virtual bool PreUpdate(GameTime time, ref Sprite parent) { return true; }
		/// <summary>
		/// Use this method to update the parent sprite or this module.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="parent"></param>
		public virtual void Update(GameTime time, ref Sprite parent) { }
		/// <summary>
		/// This method is called even if <see cref="PreUpdate(GameTime, ref Sprite)"/> returns false.
		/// It's also called just after position/velocity updates occur.
		/// </summary>
		/// <param name="time"></param>
		/// <param name="parent"></param>
		public virtual void PostUpdate(GameTime time, ref Sprite parent) { }

		/// <summary>
		/// Use this method to draw stuff behind the parent sprite.
		/// Return false to keep <see cref="Draw(SpriteBatch, ref Sprite)"/> from being called. Returns true by default.
		/// </summary>
		/// <param name="batch"></param>
		/// <param name="parent"></param>
		/// <returns></returns>
		public virtual bool PreDraw(SpriteBatch batch, ref Sprite parent) { return true; }
		/// <summary>
		/// Use this method to draw stuff WITH the parent sprite.
		/// Whether it's behind or in front of the sprite is determined by first to last in which the modules were appended.
		/// </summary>
		/// <param name="batch"></param>
		/// <param name="parent"></param>
		public virtual void Draw(SpriteBatch batch, ref Sprite parent) { }
		/// <summary>
		/// Use this method to draw stuff over the parent sprite.
		/// This method is called even if <see cref="PreDraw(SpriteBatch, ref Sprite)"/> returns false.
		/// </summary>
		/// <param name="batch"></param>
		/// <param name="parent"></param>
		public virtual void PostDraw(SpriteBatch batch, ref Sprite parent) { }

		/// <summary>
		/// Use this to load necessary variables from save data.
		/// </summary>
		/// <param name="tag"></param>
		public virtual void Load(SharpTag tag) { }
		/// <summary>
		/// Use this to save necessary variables to save data.
		/// </summary>
		/// <returns></returns>
		public virtual SharpTag Save() { return null; }

		/// <summary>
		/// Returns the name of the module.
		/// </summary>
		/// <returns></returns>
		public sealed override string ToString()
		{
			return GetType().Name;
		}

		public sealed override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
