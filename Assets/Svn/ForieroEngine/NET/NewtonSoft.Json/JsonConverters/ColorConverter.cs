using UnityEngine;

namespace ForieroEngine.Newtonsoft.JsonConverters{
	
	/// <summary>
	/// Custom <c>Newtonsoft.Json.JsonConverter</c> for <c>UnityEngine.Color</c>.
	/// </summary>
	public class ColorConverter : PartialConverter<Color>{

		/// <summary>
		/// Get the property names include <c>r</c>, <c>g</c>, <c>b</c>, <c>a</c>.
		/// </summary>
		/// <returns>The property names.</returns>
		protected override string[] GetPropertyNames(){
			return new []{"r", "g", "b", "a"};
		}

	}

}
