using UnityEditor.SettingsManagement;

namespace UnityEditor.Polybrush
{
    static class PolybrushSettings
    {
        // Derive the settings key from the package folder this code actually ships in,
        // so the fork does not keep writing under the upstream package id.
        //
        // PackageInfo is not referenced by this assembly definition (CS0117), so the id
        // is taken from the resolved package directory instead.
        internal static string k_PackageName
        {
            get
            {
                string dir = PolyEditorUtility.RootFolder;

                if (string.IsNullOrEmpty(dir))
                    return "com.community.polybrush";

                int slash = dir.LastIndexOf('/');
                return slash >= 0 ? dir.Substring(slash + 1) : dir;
            }
        }

        static Settings s_Instance;

        internal static Settings instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new Settings(k_PackageName);
                }

                return s_Instance;
            }
        }

        public static void Save()
        {
            instance.Save();
        }

        public static void Set<T>(string key, T value, SettingsScope scope = SettingsScope.Project)
        {
            instance.Set<T>(key, value, scope);
        }

        public static T Get<T>(string key, SettingsScope scope = SettingsScope.Project, T fallback = default(T))
        {
            return instance.Get<T>(key, scope, fallback);
        }

        public static bool ContainsKey<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            return instance.ContainsKey<T>(key, scope);
        }

        public static void Delete<T>(string key, SettingsScope scope = SettingsScope.Project)
        {
            instance.DeleteKey<T>(key, scope);
        }
    }
}
