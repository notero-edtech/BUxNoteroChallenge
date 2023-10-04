using UnityEngine;

namespace ForieroEngine.Newtonsoft.JsonConverters{

	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.Vector2</c>.
	/// </summary>
	public class Vector2Converter : PartialConverter<Vector2>{

		/// <summary>
		/// Get the property names include <c>x</c>, <c>y</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames(){
			return new []{"x", "y"};
		}

	}

	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.Vector2Int</c>.
	/// </summary>
	public class Vector2IntConverter : PartialConverter<Vector2Int>
	{

		/// <summary>
		/// Get the property names include <c>x</c>, <c>y</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames()
		{
			return new[] { "x", "y" };
		}

	}

}
