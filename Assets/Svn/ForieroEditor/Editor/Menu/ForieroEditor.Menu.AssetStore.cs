using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.IO;
using System.Linq;

namespace ForieroEditor.Menu
{
	public static class MenuExtensions
	{
		public static string ToStoreId (this int id) { if (id == 0) { return "/search/query=publisher:352"; } else { return "/content/" + id.ToString (); } }
	}

	public static partial class MenuItems
	{
		private const string CopyrightFree = "/* Marek Ledvina © Foriero s.r.o. 2022, The MIT License */";
		private const string CopyrightCommercial = "/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */";

		[MenuItem("Assets/Foriero/Copyright/Add Commercial Notice")] public static void AddCommercialNotice() => AddCopyrightNotice(CopyrightCommercial);
		[MenuItem("Assets/Foriero/Copyright/Add Free Notice")] public static void AddFreeNotice() => AddCopyrightNotice(CopyrightFree);
		
		private static void AddCopyrightNotice(string notice)
		{
			void NoticeFile(string f)
			{
				var l = File.ReadAllLines(f).ToList();
				if (l.Count > 0 && l[0] != notice) l.Insert(0, notice);
				File.WriteAllLines(f, l.ToArray());
			}
			
			var objects = Selection.GetFiltered<Object>(SelectionMode.Assets);
			foreach (var o in objects)
			{
				if (o.IsFolder())
				{
					var d = AssetDatabase.GetAssetPath(o).GetFullPathFromAssetPath();
					var files = Directory.GetFiles(d, "*.cs", SearchOption.AllDirectories);
					foreach (var f in files) { if (File.Exists(f)) NoticeFile(f); }
				} else if (o.IsScript()) { NoticeFile(AssetDatabase.GetAssetPath(o).GetFullPathFromAssetPath()); }
			}
			AssetDatabase.Refresh();
		}
		
		[MenuItem("Foriero/Links/Asset Store/3rd/Easy Touch", false, AssetStoreInternal.menuOrderIndex)] public static void OpenEasyTouch() => AssetStore.Open(AssetStoreInternal.easyTouch.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/3rd/DOTween", false, AssetStoreInternal.menuOrderIndex)] public static void OpenDOTween() => AssetStore.Open(AssetStoreInternal.doTweenId.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/3rd/Playmaker", false, AssetStoreInternal.menuOrderIndex)] public static void OpenPlaymaker() => AssetStore.Open(AssetStoreInternal.playMaker.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Animals/Fishes 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenFishesVol1() => AssetStore.Open(AssetStoreInternal.fishesVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Backgrounds/Backgrounds 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenBackgroundsVol1() => AssetStore.Open(AssetStoreInternal.backgroundsVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Backgrounds/Backgrounds 2D Vol.2", false, AssetStoreInternal.menuOrderIndex)] public static void OpenBackgroundsVol2() => AssetStore.Open(AssetStoreInternal.backgroundsVol2.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Buildings/Village 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenVillageVol1() => AssetStore.Open(AssetStoreInternal.villageVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Characters/Children 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenChildrenVol1() => AssetStore.Open(AssetStoreInternal.childrenVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Characters/Animals 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenAnimalsVol1() => AssetStore.Open(AssetStoreInternal.animalsVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Gems/Gems 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenGemsVol1() => AssetStore.Open(AssetStoreInternal.gemsVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Gems/Gems 2D Vol.2", false, AssetStoreInternal.menuOrderIndex)] public static void OpenGemsVol2() => AssetStore.Open(AssetStoreInternal.gemsVol2.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/GUI/Cartoon PRO GUI", false, AssetStoreInternal.menuOrderIndex)] public static void OpenCartoonProGUI() => AssetStore.Open(AssetStoreInternal.cartoonProGUI.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/GUI/Wood PRO GUI", false, AssetStoreInternal.menuOrderIndex)] public static void OpenWoodProGUI() => AssetStore.Open(AssetStoreInternal.woodProGUI.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Parallaxes/Parallax 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)]	public static void OpenParallaxVol1() => AssetStore.Open(AssetStoreInternal.parallaxVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Plants/Flowers 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenFlowersVol1() => AssetStore.Open(AssetStoreInternal.flowersVol1.ToStoreId());		
		[MenuItem("Foriero/Links/Asset Store/Packages/Plants/Trees 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenTreesVol1() => AssetStore.Open(AssetStoreInternal.treesVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Plants/Trees 2D Vol.2", false, AssetStoreInternal.menuOrderIndex)] public static void OpenTreesVol2() => AssetStore.Open(AssetStoreInternal.treesVol2.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Platformers/Platformer 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenPlatformerVol1() => AssetStore.Open(AssetStoreInternal.platfomerVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Skies/Skies 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)]	public static void OpenSkiesVol1() => AssetStore.Open(AssetStoreInternal.skiesVol1.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Packages/Weapons/Guns 2D Vol.1", false, AssetStoreInternal.menuOrderIndex)] public static void OpenGunsVol1() => AssetStore.Open(AssetStoreInternal.gunsVol1.ToStoreId());
		//[MenuItem ("Foriero/Links/Asset Store/Packages/Weapons/Guns 2D Vol.2", false, AssetStoreInternal.menuOrderIndex)] public static void OpenGunsVol2 () => AssetStore.Open(AssetStoreInternal.gunsVol2.ToStoreId ());
		[MenuItem("Foriero/Links/Asset Store/Tools/Assembly Builder", false, AssetStoreInternal.menuOrderIndex)] public static void OpenAssemblyBuilder() => AssetStore.Open(AssetStoreInternal.assemblyBuilder.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Tools/Hard Links", false, AssetStoreInternal.menuOrderIndex)] public static void OpenHardLinks() => AssetStore.Open(AssetStoreInternal.hardLinks.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Tools/Illustrator Tool", false, AssetStoreInternal.menuOrderIndex)] public static void OpenIllustratorTool() => AssetStore.Open(AssetStoreInternal.illustratorTool.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Tools/Mac App Store - Signed", false, AssetStoreInternal.menuOrderIndex)] public static void OpenMacAppStore() => AssetStore.Open(AssetStoreInternal.macAppStore.ToStoreId());
		[MenuItem("Foriero/Links/Asset Store/Tools/Store Shots", false, AssetStoreInternal.menuOrderIndex)] public static void OpenStoreShots() =>	AssetStore.Open(AssetStoreInternal.storeShots.ToStoreId());

		static class AssetStoreInternal
		{
			public const int menuOrderIndex = -700;

			//-------//
			public static int doTweenId = 27676;
			public static int easyTouch = 3322;
			public static int playMaker = 368;
			//-------//

			public static int gunsVol1 = 54981;
			//static int gunsVol2 = 0;
			public static int treesVol1 = 51071;
			public static int treesVol2 = 52809;
			public static int flowersVol1 = 52810;
			public static int platfomerVol1 = 52817;
			public static int parallaxVol1 = 53230;
			public static int gemsVol1 = 28764;
			public static int gemsVol2 = 52813;
			public static int childrenVol1 = 52812;
			public static int animalsVol1 = 52811;
			public static int fishesVol1 = 53231;
			public static int villageVol1 = 53101;
			public static int cartoonProGUI = 28567;
			public static int woodProGUI = 28496;
			public static int skiesVol1 = 52818;
			public static int backgroundsVol1 = 51075;
			public static int backgroundsVol2 = 53785;

			public static int assemblyBuilder = 3212;
			public static int hardLinks = 55471;
			public static int illustratorTool = 25491;
			public static int macAppStore = 54970;
			public static int storeShots = 54971;			
		}
	}
}