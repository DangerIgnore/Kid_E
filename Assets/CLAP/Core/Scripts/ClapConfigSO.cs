using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Clap
{
    [CreateAssetMenu(fileName = "ClapConfiguration", menuName = "Clap/Configuration", order = 1)]
    [System.Serializable]
    public class ClapConfigSO : ScriptableObject
    {

        public const string k_MyCustomSettingsPath = "Assets/Clap/Core/Resources/Core/ClapConfiguration.asset"; //needs to be same path as below

        internal static ClapConfigSO GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<ClapConfigSO>(k_MyCustomSettingsPath);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<ClapConfigSO>();
                settings.m_resourcePath = "C:/Users/Micah/CLAP/Demos/resources/";
                settings.m_dllFolderPath = @"C:/Users/Micah/CLAP/Demos/CLAPTAP/x64/Release";
                settings.m_datasetExportPath = "C:/Users/Micah/Desktop/ClapDataSetExports/";
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        

        public TrackingMethod TRACKING_METHOD = TrackingMethod.LEAP_MOTION;


        [SerializeField] private string m_resourcePath = "C:/Users/Micah/CLAP/Demos/resources/";
        [SerializeField] private string m_dllFolderPath = @"C:/Users/Micah/CLAP/Demos/CLAPTAP/x64/Release";
        [SerializeField] private string m_datasetExportPath = "C:/Users/Micah/Desktop/ClapDataSetExports/";

        public string ResourcePath
        {
            get
            {
                return m_resourcePath;
            }

            set
            {
                m_resourcePath = value;
            }
        }

        public string DllFolderPath
        {
            get
            {
                return m_dllFolderPath;
            }

            set
            {
                m_dllFolderPath = value;
            }
        }

        public string DatasetExportPath
        {
            get
            {
                return m_datasetExportPath;
            }

            set
            {
                m_datasetExportPath = value;
            }
        }

        /// <summary>
        /// Returns the path to the currentDllFolder;
        /// </summary>
        /// <returns></returns>
        public string GetDllFolder()
        {
            return DllFolderPath;
            //return dllFoldersDictionary[CLAP_DLL_FOLDER];
        }


        //When adding a new location, add it to the enum and to the dictionary.
        public enum dllFolders { Mickeal, Mickeal_Debug, Dani, Dani_Debug, Dani_Casa, Mine, Dani_Laptop, Micah_Office }
        /*    Dictionary<dllFolders, string> dllFoldersDictionary = new Dictionary<dllFolders, string>()
            {
                { dllFolders.Mickeal, @"C:/Users/Mickeal/Documents/clap_tap/Demos/CLAPTAP/x64/Release"},
                { dllFolders.Mickeal_Debug,@"C:/Users/Mickeal/Documents/clap_tap/Demos/CLAPTAP/x64/Debug"},
                { dllFolders.Dani,  @"C:/Users/Dani/PhD/Wearhap/Tactile Render/CLAP_TAP/Demos/CLAPTAP/x64/Release"},
                { dllFolders.Dani_Debug, @"C:/Users/Dani/PhD/Wearhap/Tactile Render/CLAP_TAP/Demos/CLAPTAP/x64/Debug"},
                { dllFolders.Dani_Casa, @"C:/Users/Dani/PhD/CLAP_TAP/Demos/CLAPTAP/x64/Release"},
                { dllFolders.Mine,@"C:/Users/mine/Documents/clap_tap/Demos/CLAPTAP/x64/Release"},
                { dllFolders.Dani_Laptop, @"D:/Projects/CLAPTAP/Demos/CLAPTAP/x64/Release"},    
                { dllFolders.Micah_Office, @"C:/Users/Micah/CLAP/Demos/CLAPTAP/x64/Release"},
            };
            */

        //Possible Values
        public enum TrackingMethod
        {
            LEAP_MOTION = 0,
            b = 1,
            c = 2
        }

    }


    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class MyCustomSettingsIMGUIRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/Clap", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Clap",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = ClapConfigSO.GetSerializedSettings();
                    EditorGUILayout.PropertyField(settings.FindProperty("m_resourcePath"), new GUIContent("Resource Folder Path"));
                    EditorGUILayout.PropertyField(settings.FindProperty("m_dllFolderPath"), new GUIContent("DLL Folder Path"));
                    EditorGUILayout.PropertyField(settings.FindProperty("m_datasetExportPath"), new GUIContent("Dataset Export Path"));

                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Number", "Some String" })
            };

            return provider;
        }
    }

    /*
    // Create MyCustomSettingsProvider by deriving from SettingsProvider:
    class ClapConfigSO_Provider : SettingsProvider
    {
        private SerializedObject m_CustomSettings;

        class Styles
        {
            public static GUIContent number = new GUIContent("My Number");
            public static GUIContent someString = new GUIContent("Some string");
        }

        public const string k_MyCustomSettingsPath = "Assets/Clap/Core/Editor/ClapConfiguration.asset";  //needs to be same path as above
        public ClapConfigSO_Provider(string path, SettingsScope scope = SettingsScope.User)
            : base(path, scope) { }

        public static bool IsSettingsAvailable()
        {
            return File.Exists(k_MyCustomSettingsPath);
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            // This function is called when the user clicks on the MyCustom element in the Settings window.
            m_CustomSettings = ClapConfigSO_Editor.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            // Use IMGUI to display UI:
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_Number"), Styles.number);
            EditorGUILayout.PropertyField(m_CustomSettings.FindProperty("m_SomeString"), Styles.someString);
        }

        // Register the SettingsProvider
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new ClapConfigSO_Provider("Project/ClapConfigSO_Editor", SettingsScope.Project);

                // Automatically extract all keywords from the Styles.
                provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
                return provider;
            }

            // Settings Asset doesn't exist yet; no need to display anything in the Settings window.
            return null;
        }
    }
    */
}