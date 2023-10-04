using System.IO;
using ForieroEditor.Extensions;
using UnityEditor;

namespace ForieroEditor.Menu
{
	public static partial class MenuItems
	{
		[MenuItem("Foriero/GitHub/Get Azure", false, -900)] public static void GetAzure() => GitHubInternal.GetAzure();
		[MenuItem("Foriero/GitHub/Get SimpleJSON", false, -900)] public static void GetSimpleJSON() => GitHubInternal.GetSimpleJSON();
		[MenuItem("Foriero/GitHub/Playmaker/Get TextMeshPro Actions", false, -900)] public static void GetTextMeshProPlaymakerActions() => GitHubInternal.GetTextMeshProPlaymakerActions();
		[MenuItem("Foriero/GitHub/Get RuntimeSpriteSheetsGenerator", false, -900)] public static void GetRuntimeSpriteSheetsGenerator() => GitHubInternal.GetRuntimeSpriteSheetsGenerator();

		const string spineMenuPath = "Foriero/GitHub/";
		const string spineRuntime = spineMenuPath + "Get Spine Runtime/";
		const string spineTimeline = spineMenuPath + "Get Spine Timeline/";
		const string spineUniversalRenderPipeline = spineMenuPath + "Get Spine URP/";
		const string spineLightWeightRenderPipeline = spineMenuPath + "Get Spine LWRP/";
		const string spineRuntimeExamples = spineMenuPath + "Get Spine Runtime Examples/";
		
		const string spine38 = "3.8";
		const string spine38git = "branches/3.8";
		[MenuItem(spineRuntime + spine38, false, -900)] public static void GetSpineRuntime38() => GitHubInternal.GetSpineRuntime(spine38git);
		[MenuItem(spineTimeline + spine38, false, -900)] public static void GetSpineTimeline38() => GitHubInternal.GetSpineTimeline(spine38git);
		[MenuItem(spineUniversalRenderPipeline + spine38, false, -900)] public static void GetSpineUniversalRenderPipeline38() => GitHubInternal.GetSpineUniversalRenderPipeline(spine38git);
		[MenuItem(spineLightWeightRenderPipeline + spine38, false, -900)] public static void GetSpineLightWeightRenderPipeline38() => GitHubInternal.GetSpineLightWeightRenderPipeline(spine38git);
		[MenuItem(spineRuntimeExamples + spine38, false, -900)] public static void GetSpineRuntimeExamples38() => GitHubInternal.GetSpineRuntimeExamples(spine38git);

		const bool spine40Beta = false;
		const string spine40 = "4.0" + (spine40Beta ? "-beta" : "");
		const string spine40git = "branches/4.0" + (spine40Beta ? "-beta" : "");
		[MenuItem(spineRuntime + spine40, false, -900)] public static void GetSpineRuntime40() => GitHubInternal.GetSpineRuntime(spine40git);
		[MenuItem(spineTimeline + spine40, false, -900)] public static void GetSpineTimeline40() => GitHubInternal.GetSpineTimeline(spine40git);
		[MenuItem(spineUniversalRenderPipeline + spine40, false, -900)] public static void GetSpineUniversalRenderPipeline40() => GitHubInternal.GetSpineUniversalRenderPipeline(spine40git);
		[MenuItem(spineLightWeightRenderPipeline + spine40, false, -900)] public static void GetSpineLightWeightRenderPipeline40() => GitHubInternal.GetSpineLightWeightRenderPipeline(spine40git);
		[MenuItem(spineRuntimeExamples + spine40, false, -900)] public static void GetSpineRuntimeExamples40() => GitHubInternal.GetSpineRuntimeExamples(spine40git);
		
		const bool spine41Beta = false;
		const string spine41 = "4.1" + (spine41Beta ? "-beta" : "");
		const string spine41git = "branches/4.1" + (spine41Beta ? "-beta" : "");
		[MenuItem(spineRuntime + spine41, false, -900)] public static void GetSpineRuntime41() => GitHubInternal.GetSpineRuntime(spine41git);
		[MenuItem(spineTimeline + spine41, false, -900)] public static void GetSpineTimeline41() => GitHubInternal.GetSpineTimeline(spine41git);
		[MenuItem(spineUniversalRenderPipeline + spine41, false, -900)] public static void GetSpineUniversalRenderPipeline41() => GitHubInternal.GetSpineUniversalRenderPipeline(spine41git);
		[MenuItem(spineLightWeightRenderPipeline + spine41, false, -900)] public static void GetSpineLightWeightRenderPipeline41() => GitHubInternal.GetSpineLightWeightRenderPipeline(spine41git);
		[MenuItem(spineRuntimeExamples + spine41, false, -900)] public static void GetSpineRuntimeExamples41() => GitHubInternal.GetSpineRuntimeExamples(spine41git);

		[MenuItem("Foriero/GitHub/Get xNode", false, -900)] public static void GetxNode() => GitHubInternal.GetxNode();
		[MenuItem("Foriero/GitHub/Get DotNetZip", false, -900)] public static void GetDotNetZip() => GitHubInternal.GetDotNetZip();
		[MenuItem("Foriero/GitHub/Get SpriteShaders", false, -900)] public static void GetSpriteShaders() => GitHubInternal.GetSpriteShaders();
		[MenuItem("Foriero/GitHub/Get CSharpSynth", false, -900)] public static void GetCSharpSynth() => GitHubInternal.GetCSharpSynth();
		[MenuItem("Foriero/GitHub/Get UniRx", false, -900)] public static void GetUniRx() => GitHubInternal.GetUniRx();
		[MenuItem("Foriero/GitHub/Get UniTask", false, -900)] public static void GetUniTask() => GitHubInternal.GetUniTask();
		[MenuItem("Foriero/GitHub/Get LINQtoGameObject", false, -900)] public static void GetLINQToGameObject() => GitHubInternal.GetLINQToGameObject();

		[MenuItem("Foriero/GitHub/Fonts/Get FontAwesome", false, -900)] public static void GetFontAwesome() => GitHubInternal.GetFontAwesome();
		[MenuItem("Foriero/GitHub/Fonts/Get OpenDyslexic", false, -900)] public static void GetFontOpenDyslexic() => GitHubInternal.GetFontOpenDyslexic();

		[MenuItem("Foriero/GitHub/Get Steamworks NET", false, -900)] public static void GetSteamworksNET() => GitHubInternal.GetSteamworksNET();

		public static class GitHubInternal
		{
			public static void GetSteamworksNET()
            {
				string url1 = "https://github.com/rlabrecque/Steamworks.NET/trunk/Plugins/Steamworks.NET/";
				CommandLine.GitHub.GetRepositoryFiles(url1, "Assets/Git/Steamworks.NET/src/", (s) => AssetDatabase.Refresh());
				string url2 = "https://github.com/rlabrecque/Steamworks.NET/trunk/Editor/Steamworks.NET/";
				CommandLine.GitHub.GetRepositoryFiles(url2, "Assets/Git/Steamworks.NET/Editor/", (s) => AssetDatabase.Refresh());
				string url3 = "https://github.com/rlabrecque/Steamworks.NET-Example/trunk/Assets/Scripts/Steamworks.NET/SteamManager.cs";
				CommandLine.GitHub.GetRepositoryFiles(url3, "Assets/Git/Steamworks.NET/SteamManager.cs", (s) => AssetDatabase.Refresh());
			}

			public static void GetAzure()
			{
				string url = "https://github.com/Unity3dAzure/AppServices/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/Azure/AppServices/", (s) => AssetDatabase.Refresh());

				url = "https://github.com/Unity3dAzure/RESTClient/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/Azure/RESTClient/", (s) => AssetDatabase.Refresh());

				url = "https://github.com/Unity3dAzure/StorageServices/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/Azure/StorageServices/", (s) => AssetDatabase.Refresh());

				url = "https://github.com/Unity3dAzure/AzureFunctions/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/Azure/AzureFunctions/", (s) => AssetDatabase.Refresh());				
			}

			public static void GetLINQToGameObject()
            {
				string url = "https://github.com/neuecc/LINQ-to-GameObject-for-Unity/trunk/Assets/LINQtoGameObject";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/LINQToGameObject/", (s) => AssetDatabase.Refresh());
			}

			//[MenuItem("Foriero/GitHub/Get UI Extensions")]
			//public static void GetUIExtensions()
			//{
			//	string url = "https://bitbucket.org/UnityUIExtensions/unity-ui-extensions/src/master";
			//	CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/UIExtensions");
			//	AssetDatabase.Refresh();
			//}

			public static void GetSimpleJSON()
			{
				string url = "https://github.com/Bunny83/SimpleJSON/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/SimpleJSON", (s) => AssetDatabase.Refresh());				
			}
						
			public static void GetTextMeshProPlaymakerActions()
			{
				string url = "https://github.com/dumbgamedev/TextMeshPro_Playmaker/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/Playmaker/TextMeshPro", (s) => AssetDatabase.Refresh());				
			}
			
			public static void GetRuntimeSpriteSheetsGenerator()
			{
				string url = "https://github.com/DaVikingCode/UnityRuntimeSpriteSheetsGenerator/trunk/Assets/RuntimeSpriteSheetsGenerator/Scripts";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/RuntimeSpriteSheetsGenerator", (s) => AssetDatabase.Refresh());				
			}

			public static void GetSpineRuntime(string branch)
			{
				string spine_unity = "https://github.com/EsotericSoftware/spine-runtimes/" + branch + "/spine-unity/Assets/Spine/";
				string spine_src = "https://github.com/EsotericSoftware/spine-runtimes/" + branch + "/spine-csharp/src/";

				CommandLine.GitHub.GetRepositoryFiles(spine_unity, "Assets/Git/Spine", (s) => AssetDatabase.Refresh());
				CommandLine.GitHub.GetRepositoryFiles(spine_src, "Assets/Git/Spine/Runtime/spine-csharp/src", (s) => AssetDatabase.Refresh());				
			}

			public static void GetSpineTimeline(string branch)
			{
				string spine_timeline = "https://github.com/EsotericSoftware/spine-runtimes/" + branch + "/spine-unity/Modules/com.esotericsoftware.spine.timeline/";
				CommandLine.GitHub.GetRepositoryFiles(spine_timeline, "Assets/Git/SpineTimeline", (s) => AssetDatabase.Refresh());				
			}

			public static void GetSpineUniversalRenderPipeline(string branch)
			{
				string spine_timeline = "https://github.com/EsotericSoftware/spine-runtimes/" + branch + "/spine-unity/Modules/com.esotericsoftware.spine.urp-shaders/Shaders/";
				CommandLine.GitHub.GetRepositoryFiles(spine_timeline, "Assets/Git/SpineUniversalRenderPipeline/Shaders", (s) => AssetDatabase.Refresh());				
			}

			public static void GetSpineLightWeightRenderPipeline(string branch)
			{
				string spine_timeline = "https://github.com/EsotericSoftware/spine-runtimes/" + branch + "/spine-unity/Modules/com.esotericsoftware.spine.lwrp-shaders/Shaders/";
				CommandLine.GitHub.GetRepositoryFiles(spine_timeline, "Assets/Git/SpineLightWeightRenderPipeline/Shaders", (s) => AssetDatabase.Refresh());				
			}

			public static void GetSpineRuntimeExamples(string branch)
			{
				string spine_examples = "https://github.com/EsotericSoftware/spine-runtimes/" + branch + "/spine-unity/Assets/Spine Examples/";
				CommandLine.GitHub.GetRepositoryFiles(spine_examples, "Assets/Git/Spine Examples", (s) => AssetDatabase.Refresh());				
			}
							
			public static void GetxNode()
			{
				string url = "https://github.com/Siccity/xNode/trunk/Scripts";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/xNode", (s) => AssetDatabase.Refresh());				
			}

			//[MenuItem("Foriero/GitHub/Get url")]
			//public static void Geturl()
			//{
			//	string url = "https://github.com/Demigiant/url/trunk/_url.Assembly/bin";
			//	CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Demigiant/url");
			//	AssetDatabase.Refresh();
			//}

			public static void GetDotNetZip()
			{
				string url = "https://github.com/foriero/dotnetzip-for-unity/trunk/Binaries/Release";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/DotNetZip", (s) => AssetDatabase.Refresh());				
			}

			public static void GetSpriteShaders()
			{
				string url = "https://github.com/traggett/UnitySpriteShaders/trunk/SpriteShaders";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/SpriteShaders", (s) => AssetDatabase.Refresh());				
			}

			public static void GetCSharpSynth()
			{
				string url = "https://github.com/foriero/csharpsynthunity/trunk/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Git/CSharpSynth", (s) => AssetDatabase.Refresh());				
			}

			public static void GetUniRx()
			{
				string url = "https://github.com/neuecc/UniRx/trunk/Assets/Plugins/UniRx/";
				CommandLine.GitHub.GetRepositoryFiles(url, "Assets/Plugins/UniRx", (s) => AssetDatabase.Refresh());				
			}

			public static void GetUniTask()
			{
				string editor = "https://github.com/Cysharp/UniTask/trunk/src/UniTask/Assets/Plugins/UniTask/Editor/";
				CommandLine.GitHub.GetRepositoryFiles(editor, "Assets/Plugins/UniTask/Editor", (s) => AssetDatabase.Refresh());

				string runtime = "https://github.com/Cysharp/UniTask/trunk/src/UniTask/Assets/Plugins/UniTask/Runtime/";
				CommandLine.GitHub.GetRepositoryFiles(runtime, "Assets/Plugins/UniTask/Runtime", (s) => AssetDatabase.Refresh());				
			}

			public static void GetFontAwesome()
			{
				string font = "https://github.com/FortAwesome/Font-Awesome/trunk/fonts/fontawesome-webfont.ttf";
				string variables = "https://github.com/FortAwesome/Font-Awesome/trunk/less/variables.less";

				string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "FontAwesome.ttf", SearchOption.AllDirectories);

				string path = "Assets/Git/Fonts";

				if (files.Length != 0)
				{
					path = Path.GetDirectoryName(files[0]).RemoveProjectPath();
				}

				path = path.FixAssetsPath();

				CommandLine.GitHub.GetRepositoryFiles(font, path + "/FontAwesome.ttf", (s) => AssetDatabase.Refresh());
				CommandLine.GitHub.GetRepositoryFiles(variables, path + "/variables.less", (s) => AssetDatabase.Refresh());
			}

			public static void GetFontOpenDyslexic()
			{
				
				string font = "https://github.com/antijingoist/opendyslexic/trunk/compiled/OpenDyslexic-Regular.otf";
				
				string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "OpenDyslexic-Regular.otf", SearchOption.AllDirectories);

				string path = "Assets/Git/Fonts";

				if (files.Length != 0)
				{
					path = Path.GetDirectoryName(files[0]).RemoveProjectPath();
				}

				path = path.FixAssetsPath();

				CommandLine.GitHub.GetRepositoryFiles(font, path + "/OpenDyslexic-Regular.otf", (s) => AssetDatabase.Refresh());
			}
		}
	}
}