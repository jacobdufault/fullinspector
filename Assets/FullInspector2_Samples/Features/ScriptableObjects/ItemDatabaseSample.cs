using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// This file demonstrates how you could have a item database setup where the database is defined
// inside of a scriptable object

namespace FullInspector.Samples.ItemDatabase {
    public class ItemDatabaseSample : BaseScriptableObject<FullSerializerSerializer> {
        // name -> item
        public Dictionary<string, IItem> Items;
    }

    public interface IItem {
    }

    public class ArmorItem : IItem {
        public float Armor;
    }

    public class HealthBoostItem : IItem {
        public float HealthIncrease;
        public float Cooldown;
    }

    public class WeaponItem : IItem {
        public enum WeaponType {
            Sword, Axe, Hammer
        }

        public WeaponType Type;
        public float AttackStrength;
    }

#if UNITY_EDITOR
    public class ItemDatabaseEditorIntegration {
        [MenuItem("Window/Full Inspector/Testing/Create ItemDatabase Scriptable Object")]
        public static void CreateAsset() {
            CreateAsset<ItemDatabaseSample>();
        }

        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static void CreateAsset<T>() where T : ScriptableObject {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "") {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
#endif
}