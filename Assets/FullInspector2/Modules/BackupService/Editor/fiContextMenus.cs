using UnityEditor;
using UnityEngine;

namespace FullInspector.BackupService {
    public class fiContextMenus {
        // We support backup for *all* components derived from BaseBehavior
        [MenuItem("CONTEXT/CommonBaseBehavior/Backup")]
        public static void BackupBaseBehavior(MenuCommand command) {
            if (command.context is Component) {
                var component = (Component)command.context;
                fiBackupManager.CreateBackup(component);
            }
        }

        // We have to white-list other components, to verify that they work with the default
        // serializer. Some will cause it to break, etc. To get better support in the future for
        // more components, we might need to introduction an abstraction into CreateBackup.

        [MenuItem("CONTEXT/Transform/Backup")]
        public static void BackupTransform(MenuCommand command) {
            if (command.context is Component) {
                var component = (Component)command.context;
                fiBackupManager.CreateBackup(component);
            }
        }
    }
}