using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace ForieroEditorInternal.Extensions
{
    public static class AddressablesHelper
    {
        public static AddressableAssetGroup GetGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return null;

            var s = AddressableAssetSettingsDefaultObject.Settings;
            return s.FindGroup(groupName);
        }

        public static void RemoveGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return;

            var s = AddressableAssetSettingsDefaultObject.Settings;

            if (!GroupExists(groupName)) return;

            s.RemoveGroup(s.FindGroup(groupName));
        }

        public static AddressableAssetGroup CreateGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                return null;

            var s = AddressableAssetSettingsDefaultObject.Settings;
            var group = s.CreateGroup(groupName, false, false, false, s.DefaultGroup.Schemas);

            s.SetDirty(AddressableAssetSettings.ModificationEvent.GroupAdded, group, true);

            return group;
        }

        public static bool GroupExists(string groupName)
        {
            var s = AddressableAssetSettingsDefaultObject.Settings;
            return s.FindGroup(groupName) != null;
        }
    }

    public static partial class ForieroEditorExtensions
	{
        public static AddressableAssetEntry CreateAssetEntryWithLabel<T>(this T source, string groupName, string label) where T : Object
        {
            var entry = CreateAssetEntry(source, groupName);
            if (source != null)
            {
                source.AddAddressableAssetLabel(label);
            }

            return entry;
        }

        public static AddressableAssetEntry CreateAssetEntry<T>(this T source, string groupName, string address) where T : Object
        {
            var entry = source.CreateAssetEntry(groupName);
            entry.address = address; 
            return entry;
        }

        public static AddressableAssetEntry CreateAssetEntry<T>(this T source, string groupName) where T : Object
        {
            if (source == null || string.IsNullOrEmpty(groupName) || !AssetDatabase.Contains(source))
                return null;

            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            var sourcePath = AssetDatabase.GetAssetPath(source);
            var sourceGuid = AssetDatabase.AssetPathToGUID(sourcePath);
            var group = !AddressablesHelper.GroupExists(groupName) ? AddressablesHelper.CreateGroup(groupName) : AddressablesHelper.GetGroup(groupName);

            var entry = addressableSettings.CreateOrMoveEntry(sourceGuid, group);
            entry.address = sourcePath;

            addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

            return entry;
        }

        public static AddressableAssetEntry CreateAssetEntry<T>(this T source) where T : Object
        {
            if (source == null || !AssetDatabase.Contains(source))
                return null;

            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            var sourcePath = AssetDatabase.GetAssetPath(source);
            var sourceGuid = AssetDatabase.AssetPathToGUID(sourcePath);
            var entry = addressableSettings.CreateOrMoveEntry(sourceGuid, addressableSettings.DefaultGroup);
            entry.address = sourcePath;

            addressableSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);

            return entry;
        }

        

        public static void RemoveAddressableAssetLabel(this Object source, string label)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return;

            var entry = source.GetAddressableAssetEntry();
            if (entry != null && entry.labels.Contains(label))
            {
                entry.labels.Remove(label);

                AddressableAssetSettingsDefaultObject.Settings.SetDirty(AddressableAssetSettings.ModificationEvent.LabelRemoved, entry, true);
            }
        }

        public static void AddAddressableAssetLabel(this Object source, string label)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return;

            var entry = source.GetAddressableAssetEntry();
            if (entry != null && !entry.labels.Contains(label))
            {
                entry.labels.Add(label);

                AddressableAssetSettingsDefaultObject.Settings.SetDirty(AddressableAssetSettings.ModificationEvent.LabelAdded, entry, true);
            }
        }

        public static void SetAddressableAssetAddress(this Object source, string address)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return;

            var entry = source.GetAddressableAssetEntry();
            if (entry != null)
            {
                entry.address = address;
            }
        }

        public static void SetAddressableAssetGroup(this Object source, string groupName)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return;

            var group = !AddressablesHelper.GroupExists(groupName) ? AddressablesHelper.CreateGroup(groupName) : AddressablesHelper.GetGroup(groupName);
            source.SetAddressableAssetGroup(group);
        }

        public static void SetAddressableAssetGroup(this Object source, AddressableAssetGroup group)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return;

            var entry = source.GetAddressableAssetEntry();
            if (entry != null && !source.IsInAddressableAssetGroup(group.Name))
            {
                entry.parentGroup = group;
            }
        }

        public static HashSet<string> GetAddressableAssetLabels(this Object source)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return null;

            var entry = source.GetAddressableAssetEntry();
            return entry?.labels;
        }

        public static string GetAddressableAssetPath(this Object source)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return string.Empty;

            var entry = source.GetAddressableAssetEntry();
            return entry != null ? entry.address : string.Empty;
        }

        public static bool IsInAddressableAssetGroup(this Object source, string groupName)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return false;

            var group = source.GetCurrentAddressableAssetGroup();
            return group != null && group.Name == groupName;
        }

        public static AddressableAssetGroup GetCurrentAddressableAssetGroup(this Object source)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return null;

            var entry = source.GetAddressableAssetEntry();
            return entry?.parentGroup;
        }

        public static AddressableAssetEntry GetAddressableAssetEntry(this Object source)
        {
            if (source == null || !AssetDatabase.Contains(source))
                return null;

            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
            var sourcePath = AssetDatabase.GetAssetPath(source);
            var sourceGuid = AssetDatabase.AssetPathToGUID(sourcePath);

            return addressableSettings.FindAssetEntry(sourceGuid);
        }
    }
}
