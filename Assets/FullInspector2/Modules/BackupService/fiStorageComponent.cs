﻿using FullInspector.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.BackupService {
    /// <summary>
    /// The backup solution stores the backups inside of this storage component.
    /// There should always be two instances of the component -- one in the scene
    /// (managed by fiSceneManager), and one in a prefab (managed by
    /// fiPrefabManager). The prefab storage is used when Unity is in play mode
    /// (so that the data will persist) and when the data being backed up is
    /// targeting something that is not a scene (say, another prefab).
    /// </summary>
    [AddComponentMenu("")]
    public class fiStorageComponent : MonoBehaviour, fiIEditorOnlyTag {
        public List<string> SeenBaseBehaviors = new List<string>();

        /// <summary>
        /// Our backups.
        /// </summary>
        public List<fiSerializedObject> Objects = new List<fiSerializedObject>();

        /// <summary>
        /// Removes all backups that no longer have a target.
        /// </summary>
        public void RemoveInvalidBackups() {
            bool removedAny = false;
            int i = 0;
            while (i < Objects.Count) {
                if (Objects[i].Target.Target == null) {
                    Objects.RemoveAt(i);
                    removedAny = true;
                }
                else {
                    ++i;
                }
            }

            if (removedAny) {
                SetDirty();
            }
        }


        /// <summary>
        ///     Unified method to set the storage dirty. Works with both prefab and scene storages.
        /// </summary>
        public void SetDirty() {
            if (this == null) {
                return;
            }

            if (fiLateBindings.PrefabUtility.IsPrefab(this)) {
                fiLateBindings.EditorUtility.SetDirty(gameObject);
                fiLateBindings.AssetDatabase.SaveAssets();
                return;
            }

            if (!Application.isPlaying && gameObject.scene.IsValid()) {
                fiLateBindings.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
        }

    }
}
