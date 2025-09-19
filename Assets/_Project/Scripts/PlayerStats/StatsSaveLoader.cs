using Newtonsoft.Json;
using UnityEngine;

namespace GamePlayerStats {
    public class StatsSaveLoader : ISaveLoader {
        public bool LoadData() {
            if (Repository.TryGetData("GameResources", out ResourceData data)) {
                PlayerStats.Strength = data.Strength;
                PlayerStats.Dex = data.Dex;
                PlayerStats.Percept = data.Percept;
                PlayerStats.Mystic = data.Mystic;
                Debug.Log("Loaded resources data");
                return true;
            }
            return false;
        }

        public void LoadDefaultData() {
            TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/PlayerStats/DefaultPlayerStats");
            if (textAsset == null) {
                Debug.LogError("Default resources file not found!");
                return;
            }

            try {
                var settings = new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                var defaultData = JsonConvert.DeserializeObject<ResourceData>(textAsset.text, settings);
                PlayerStats.Strength = defaultData.Strength;
                PlayerStats.Dex = defaultData.Dex;
                PlayerStats.Percept = defaultData.Percept;
                PlayerStats.Mystic = defaultData.Mystic;
            }
            catch (JsonException ex) {
                Debug.LogError($"JSON error: {ex.Message}");
            }
        }

        public void SaveData() {
            var data = new ResourceData {
                Strength = PlayerStats.Strength,
                Dex = PlayerStats.Dex,
                Percept = PlayerStats.Percept,
                Mystic = PlayerStats.Mystic
            };
            Repository.SetData("GameResources", data);
            Debug.Log("Saved resources data");
        }
    }

    [System.Serializable]
    public class ResourceData {
        public int Strength;
        public int Dex;
        public int Percept;
        public int Mystic;
    }
}