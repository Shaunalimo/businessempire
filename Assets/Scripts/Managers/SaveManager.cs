using System.Collections.Generic;
using BusinessEmpire.Core;
using BusinessEmpire.Data;
using BusinessEmpire.Utils;
using UnityEngine;

namespace BusinessEmpire.Managers
{
    /// <summary>
    /// Handles automatic and manual save/load of game state to local persistent storage.
    /// Cloud save integration point is stubbed for future implementation.
    /// </summary>
    public class SaveManager : SingletonMonoBehaviour<SaveManager>
    {
        [SerializeField] private float autoSaveInterval = GameConstants.AutoSaveIntervalSeconds;

        private float _autoSaveTimer;
        private GameSaveData _currentSave;

        public GameSaveData CurrentSave => _currentSave;
        public bool HasSave => JsonFileHelper.Exists(GameConstants.SaveFileName);

        protected override void OnSingletonAwake()
        {
            LoadGame();
        }

        private void Update()
        {
            _autoSaveTimer += Time.unscaledDeltaTime;
            if (_autoSaveTimer >= autoSaveInterval)
            {
                _autoSaveTimer = 0f;
                SaveGame();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) SaveGame();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) SaveGame();
        }

        protected override void OnApplicationQuit()
        {
            SaveGame();
            base.OnApplicationQuit();
        }

        /// <summary>Create new save or load existing.</summary>
        public void LoadGame()
        {
            if (JsonFileHelper.TryLoad(GameConstants.SaveFileName, out GameSaveData loaded))
            {
                _currentSave = loaded;
                MigrateSaveIfNeeded(_currentSave);
            }
            else
            {
                _currentSave = CreateNewSave();
            }

            _currentSave.lastOnlineTimestamp = TimeHelper.UnixNow;
            GameEvents.RaiseGameLoaded();
        }

        public void SaveGame()
        {
            if (_currentSave == null) return;

            _currentSave.lastSaveTimestamp = TimeHelper.UnixNow;
            _currentSave.lastOnlineTimestamp = TimeHelper.UnixNow;

            if (JsonFileHelper.Save(_currentSave, GameConstants.SaveFileName))
            {
                GameEvents.RaiseGameSaved();
            }
        }

        public void DeleteSave()
        {
            string path = JsonFileHelper.GetPath(GameConstants.SaveFileName);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            _currentSave = CreateNewSave();
            GameEvents.RaiseGameLoaded();
        }

        private GameSaveData CreateNewSave()
        {
            var save = new GameSaveData
            {
                cash = GameConstants.StartingCash,
                premiumCurrency = 10,
                lastSaveTimestamp = TimeHelper.UnixNow,
                lastOnlineTimestamp = TimeHelper.UnixNow,
                settings = new GameSettingsData()
            };

            foreach (BusinessType type in System.Enum.GetValues(typeof(BusinessType)))
            {
                save.businesses.Add(new BusinessInstanceData(type));
            }

            return save;
        }

        private void MigrateSaveIfNeeded(GameSaveData save)
        {
            // Future version migrations go here.
            if (save.saveVersion < 1) save.saveVersion = 1;
        }
    }
}
