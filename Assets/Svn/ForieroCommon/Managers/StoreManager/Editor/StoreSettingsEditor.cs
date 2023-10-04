using ForieroEngine.Purchasing;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StoreSettings))]
public class StoreSettingsEditor : Editor
{
    StoreSettings settings;

    Store.StoreEnum storeOverride;

    void OnEnable()
    {
        settings = target as StoreSettings;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(settings);
        }        
        EditorGUILayout.EndHorizontal();

        settings.initialize = EditorGUILayout.Toggle("Initialize", settings.initialize);

        settings.useCatalogProvider = EditorGUILayout.Toggle("Use Unity Cloud", settings.useCatalogProvider);        
        EditorGUILayout.BeginHorizontal();
        settings.projectWWW = EditorGUILayout.TextField("Project Website", settings.projectWWW, GUILayout.ExpandWidth(true));
        if(GUILayout.Button("O", GUILayout.Width(20)))
        {
            Application.OpenURL(settings.projectWWW);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        settings.appWWW = EditorGUILayout.TextField("Application Website", settings.appWWW, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("O", GUILayout.Width(20)))
        {
            Application.OpenURL(settings.appWWW);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        settings.appName = EditorGUILayout.TextField("Application Name", settings.appName, GUILayout.ExpandWidth(true));
        if (GUILayout.Button("P", GUILayout.Width(20)))
        {
            EditorGUIUtility.PingObject(settings);
        }
        EditorGUILayout.EndHorizontal();

#if FORIERO_INAPP
        settings.fakeStoreUIMode = (UnityEngine.Purchasing.FakeStoreUIMode)EditorGUILayout.EnumPopup("Fake Store UI Mode", settings.fakeStoreUIMode);
        settings.appStore = (UnityEngine.Purchasing.AppStore)EditorGUILayout.EnumPopup("App Store", settings.appStore);
#endif

        GUILayout.Box("", GUILayout.Height(5), GUILayout.ExpandWidth(true));
        settings.amazon.foldout = EditorGUILayout.Foldout(settings.amazon.foldout, "Amazon : " + settings.amazon.bundleId);
        if (settings.amazon.foldout)
        {
            EditorGUI.indentLevel++;
            settings.amazon.bundleId = EditorGUILayout.TextField("Bundle ID", settings.amazon.bundleId);
            EditorGUILayout.LabelField("Public Key");
            EditorStyles.textField.wordWrap = true;
            settings.amazon.publicKey = EditorGUILayout.TextArea(settings.amazon.publicKey);
            EditorGUI.indentLevel--;
        }

        settings.samsung.foldout = EditorGUILayout.Foldout(settings.samsung.foldout, "Samsung : " + settings.samsung.bundleId);
        if (settings.samsung.foldout)
        {
            EditorGUI.indentLevel++;
            settings.samsung.bundleId = EditorGUILayout.TextField("Bundle ID", settings.samsung.bundleId);
            EditorGUILayout.LabelField("Public Key");
            EditorStyles.textField.wordWrap = true;
            settings.samsung.publicKey = EditorGUILayout.TextArea(settings.samsung.publicKey);
            EditorGUI.indentLevel--;
        }

        settings.google.foldout = EditorGUILayout.Foldout(settings.google.foldout, "Google : " + settings.google.bundleId);
        if (settings.google.foldout)
        {
            EditorGUI.indentLevel++;
            settings.google.bundleId = EditorGUILayout.TextField("Bundle ID", settings.google.bundleId);
            EditorGUILayout.LabelField("Public Key");
            EditorStyles.textField.wordWrap = true;
            settings.google.publicKey = EditorGUILayout.TextArea(settings.google.publicKey);
            EditorGUI.indentLevel--;
        }

        settings.ios.foldout = EditorGUILayout.Foldout(settings.ios.foldout, "iOS : " + settings.ios.bundleId);
        if (settings.ios.foldout)
        {
            EditorGUI.indentLevel++;
            settings.ios.bundleId = EditorGUILayout.TextField("Bundle ID", settings.ios.bundleId);
            settings.ios.SKU = EditorGUILayout.TextField("SKU", settings.ios.SKU);
            settings.ios.appleId = EditorGUILayout.TextField("Apple ID", settings.ios.appleId);
            EditorGUI.indentLevel--;
        }

        settings.osx.foldout = EditorGUILayout.Foldout(settings.osx.foldout, "OSX : " + settings.osx.bundleId);
        if (settings.osx.foldout)
        {
            EditorGUI.indentLevel++;
            settings.osx.bundleId = EditorGUILayout.TextField("Bundle ID", settings.osx.bundleId);
            settings.osx.SKU = EditorGUILayout.TextField("SKU", settings.osx.SKU);
            settings.osx.appleId = EditorGUILayout.TextField("Apple ID", settings.osx.appleId);
            EditorGUI.indentLevel--;
        }

        settings.wsa.foldout = EditorGUILayout.Foldout(settings.wsa.foldout, "WSA : " + settings.wsa.storeId);
        if (settings.wsa.foldout)
        {
            EditorGUI.indentLevel++;
            settings.wsa.storeId = EditorGUILayout.TextField("WSA Store ID", settings.wsa.storeId);
            settings.wsa.packageIdentityName = EditorGUILayout.TextField("WSA Package Identity Name", settings.wsa.packageIdentityName);
            settings.wsa.packageFamilyName = EditorGUILayout.TextField("WSA Package Family Name", settings.wsa.packageFamilyName);
            EditorGUI.indentLevel--;
        }

        settings.udp.foldout = EditorGUILayout.Foldout(settings.udp.foldout, "UDP : " + settings.udp.bundleId);
        if (settings.udp.foldout)
        {
            EditorGUI.indentLevel++;
            settings.udp.bundleId = EditorGUILayout.TextField("Bundle ID", settings.udp.bundleId);
            EditorGUILayout.LabelField("Client Id");
            EditorStyles.textField.wordWrap = true;
            settings.udp.cliendId = EditorGUILayout.TextArea(settings.udp.cliendId);
            EditorGUI.indentLevel--;
        }

        if (GUILayout.Button("Add"))
        {
            settings.purchaseItems.Add(new StoreSettings.PurchaseItem());
            EditorUtility.SetDirty(settings);
        }

        StoreSettings.PurchaseItem deletePurchaseItem = null;

        foreach (StoreSettings.PurchaseItem purchaseItem in settings.purchaseItems)
        {
            EditorGUILayout.BeginHorizontal();
            purchaseItem.foldout = EditorGUILayout.Foldout(purchaseItem.foldout, purchaseItem.name);
            GUILayout.FlexibleSpace();
            GUILayout.Label(purchaseItem.purchaseType.ToString());
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                if (EditorUtility.DisplayDialog("Delete PurchaseItem!", "Do you want to delete : " + purchaseItem.name + " ?", "Yes", "No"))
                {
                    deletePurchaseItem = purchaseItem;
                }
            }
            EditorGUILayout.EndHorizontal();

            if (purchaseItem.foldout)
            {
                purchaseItem.purchaseType = (Store.PurchaseEnum)EditorGUILayout.EnumPopup("Purchase Type", purchaseItem.purchaseType);
                purchaseItem.name = EditorGUILayout.TextField("Name", purchaseItem.name);
                purchaseItem.description = EditorGUILayout.TextField("Description", purchaseItem.description);
                purchaseItem.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.prependBundleId);
                purchaseItem.id = EditorGUILayout.TextField("ID", purchaseItem.id);

                GUILayout.Box("Amazon", GUILayout.ExpandWidth(true));
                purchaseItem.amazon.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.amazon.overrideId);
                purchaseItem.amazon.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.amazon.prependBundleId);
                purchaseItem.amazon.id = EditorGUILayout.TextField("Amazon Id", purchaseItem.amazon.id);

                GUILayout.Box("Samsung", GUILayout.ExpandWidth(true));
                purchaseItem.samsung.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.samsung.overrideId);
                purchaseItem.samsung.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.samsung.prependBundleId);
                purchaseItem.samsung.id = EditorGUILayout.TextField("Samsung Id", purchaseItem.samsung.id);

                GUILayout.Box("Google", GUILayout.ExpandWidth(true));
                purchaseItem.google.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.google.overrideId);
                purchaseItem.google.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.google.prependBundleId);
                purchaseItem.google.id = EditorGUILayout.TextField("Google Id", purchaseItem.google.id);

                GUILayout.Box("iOS", GUILayout.ExpandWidth(true));
                purchaseItem.ios.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.ios.overrideId);
                purchaseItem.ios.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.ios.prependBundleId);
                purchaseItem.ios.id = EditorGUILayout.TextField("iOS Id", purchaseItem.ios.id);

                GUILayout.Box("OSX", GUILayout.ExpandWidth(true));
                purchaseItem.osx.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.osx.overrideId);
                purchaseItem.osx.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.osx.prependBundleId);
                purchaseItem.osx.id = EditorGUILayout.TextField("OSX Id", purchaseItem.osx.id);

                GUILayout.Box("WSA", GUILayout.ExpandWidth(true));
                purchaseItem.wsa.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.wsa.overrideId);
                purchaseItem.wsa.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.wsa.prependBundleId);
                purchaseItem.wsa.id = EditorGUILayout.TextField("WSA Id", purchaseItem.wsa.id);

                GUILayout.Box("UDP", GUILayout.ExpandWidth(true));
                purchaseItem.udp.overrideId = EditorGUILayout.Toggle("Override", purchaseItem.udp.overrideId);
                purchaseItem.udp.prependBundleId = EditorGUILayout.Toggle("Prepend Bundle Id", purchaseItem.udp.prependBundleId);
                purchaseItem.udp.id = EditorGUILayout.TextField("UDP Id", purchaseItem.udp.id);
            }
        }

        if (deletePurchaseItem != null)
        {
            settings.purchaseItems.Remove(deletePurchaseItem);
            EditorUtility.SetDirty(settings);
        }
    }
}
