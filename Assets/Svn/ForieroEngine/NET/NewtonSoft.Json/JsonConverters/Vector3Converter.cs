using UnityEngine;

namespace ForieroEngine.Newtonsoft.JsonConverters{

	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.Vector3</c>.
	/// </summary>
	public class Vector3Converter : PartialConverter<Vector3>{

		/// <summary>
		/// Get the property names include <c>x</c>, <c>y</c>, <c>z</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames(){
			return new []{"x", "y", "z"};
		}

	}

	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.Vector3Int</c>.
	/// </summary>
	public class Vector3IntConverter : PartialConverter<Vector3Int>
	{

		/// <summary>
		/// Get the property names include <c>x</c>, <c>y</c>, <c>z</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames()
		{
			return new[] { "x", "y", "z" };
		}

	}

}
