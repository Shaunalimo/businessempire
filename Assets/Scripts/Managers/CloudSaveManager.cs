using BusinessEmpire.Core;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Future cloud save integration point (Firebase, Play Games, etc.).
    /// Currently mirrors local save metadata for UI readiness.
    /// </summary>
    public class CloudSaveManager : SingletonMonoBehaviour<CloudSaveManager>
    {
        public bool IsCloudSaveAvailable { get; private set; }
        public bool IsSyncing { get; private set; }
        public string LastSyncStatus { get; private set; } = "Not configured";

        protected override void OnSingletonAwake()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // Initialize Firebase / Play Games cloud save here.
            IsCloudSaveAvailable = false;
#else
            IsCloudSaveAvailable = false;
#endif
        }

        /// <summary>Upload local save to cloud when backend is connected.</summary>
        public void UploadSave()
        {
            if (!IsCloudSaveAvailable)
            {
                LastSyncStatus = "Cloud save coming soon";
                return;
            }

            IsSyncing = true;
            // Future: serialize SaveManager.Instance.CurrentSave and upload.
            IsSyncing = false;
            LastSyncStatus = "Upload complete";
        }

        /// <summary>Download cloud save and merge with local.</summary>
        public void DownloadSave()
        {
            if (!IsCloudSaveAvailable)
            {
                LastSyncStatus = "Cloud save coming soon";
                return;
            }

            IsSyncing = true;
            IsSyncing = false;
            LastSyncStatus = "Download complete";
        }

        public void EnableCloudSave(bool enabled)
        {
            SaveManager.Instance.CurrentSave.cloudSaveEnabled = enabled;
        }
    }
}
