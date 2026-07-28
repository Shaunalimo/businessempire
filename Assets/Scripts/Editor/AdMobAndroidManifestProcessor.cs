#if UNITY_ANDROID
using System.IO;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;
using BusinessEmpire.Core;

namespace BusinessEmpire.Editor
{
    /// <summary>
    /// Injects AdMob App ID into AndroidManifest.xml at build time.
    /// </summary>
    public class AdMobAndroidManifestProcessor : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifestPath = Path.Combine(path, "src/main/AndroidManifest.xml");
            if (!File.Exists(manifestPath)) return;

            string manifest = File.ReadAllText(manifestPath);
            const string metaTag = "com.google.android.gms.ads.APPLICATION_ID";

            if (manifest.Contains(metaTag)) return;

            string insertion =
                $"\n    <meta-data android:name=\"{metaTag}\"" +
                $" android:value=\"{GameConstants.AdMobAppIdAndroid}\"/>";

            manifest = manifest.Replace("<application", "<application" + insertion);
            File.WriteAllText(manifestPath, manifest);
            Debug.Log("[Business Empire] AdMob App ID injected into AndroidManifest.");
        }
    }
}
#endif
