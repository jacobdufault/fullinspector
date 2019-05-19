using UnityEditor;
using UnityEngine;

namespace FullInspector.BackupService {
    public class fiContextMenus {
        // We support backup for *all* components derived from BaseBehavior
        [MenuItem("CONTEXT/CommonBaseBehavior/\ud83d\udcbe Backup")]
        public static void BackupBaseBehavior(MenuCommand command) {
            TryBackup(command.context);
        }

        // We have to white-list other components, to verify that they work with
        // the default serializer. Some will cause it to break, etc. To get
        // better support in the future for more components, we might need to
        // introduction an abstraction into CreateBackup.

        [MenuItem("CONTEXT/Transform/\ud83d\udcbe Backup")]
        public static void BackupTransform(MenuCommand command) {
            TryBackup(command.context);
        }

        private static void TryBackup(Object o) {
            var component = o as Component;
            if (component != null) {
                fiBackupManager.CreateBackup(component);
            }
        }
    }
}