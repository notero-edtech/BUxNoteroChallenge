using System.IO;

namespace ForieroEditor
{
    public static class Paths
	{
		public static string Assets => Path.Combine(Directory.GetCurrentDirectory(), "Assets");
		public static string Packages => Path.Combine(Directory.GetCurrentDirectory(), "Packages");
		public static string Library => Path.Combine(Directory.GetCurrentDirectory(), "Library");
		public static string ProjectSettings => Path.Combine(Directory.GetCurrentDirectory(), "ProjectSettings");
		public static string Temp => Path.Combine(Directory.GetCurrentDirectory(), "Temp");
	}
}
