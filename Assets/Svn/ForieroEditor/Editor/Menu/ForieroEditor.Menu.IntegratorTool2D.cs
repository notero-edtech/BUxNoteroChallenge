using UnityEditor;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
	{
		[MenuItem ("Assets/Integrator Tool 2D/Recreate")]
		public static void IllustratorRecreate ()
		{
			IntegratorTool2DEditor.RecreateScene.GenerateHierarchy (AssetDatabase.GetAssetPath (Selection.objects [0]), "");
		}
	}
}