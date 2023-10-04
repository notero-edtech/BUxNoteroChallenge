using UnityEngine;

namespace ForieroEngine.Newtonsoft.JsonConverters{
	
	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.Rect</c>.
	/// </summary>
	public class RectConverter : PartialConverter<Rect>{

		/// <summary>
		/// Get the property names include <c>x</c>, <c>y</c>, <c>width</c>, <c>height</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames(){
			return new []{"x", "y", "width", "height"};
		}

	}

	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.RectInt</c>.
	/// </summary>
	public class RectIntConverter : PartialConverter<RectInt>
	{

		/// <summary>
		/// Get the property names include <c>x</c>, <c>y</c>, <c>width</c>, <c>height</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames()
		{
			return new[] { "x", "y", "width", "height" };
		}

	}
}
