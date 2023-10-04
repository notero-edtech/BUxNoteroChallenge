using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


namespace ForieroEditor.Extensions
{
	public static partial class ForieroEditorExtensions
	{
		public static bool IndexInRange<T>(this IList<T> list, int index)
		{
			if (list == null) return false;
			if (index >= 0 && index < list.Count) return true;
			return false;
		}
	}
}
