using Newtonsoft.Json;
using UnityEngine;

namespace GameResources
{
    public class ResourceSaveLoader : ISaveLoader
    {
        public bool LoadData()
        {
            if (Repository.TryGetData("GameResources", out ResourceData data))
            {
                GameResources.Gold = data.Gold;
                GameResources.Food = data.Food;
                GameResources.PeopleSatisfaction = data.PeopleSatisfaction;
                GameResources.CastleStrength = data.CastleStrength;
                Debug.Log("Loaded resources data");
                return true;
            }
            return false;
        }

        public void LoadDefaultData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/GameResources/DefaultResources");
            if (textAsset == null)
            {
                Debug.LogError("Default resources file not found!");
                return;
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                var defaultData = JsonConvert.DeserializeObject<ResourceData>(textAsset.text, settings);
                GameResources.Gold = defaultData.Gold;
                GameResources.Food = defaultData.Food;
                GameResources.PeopleSatisfaction = defaultData.PeopleSatisfaction;
                GameResources.CastleStrength = defaultData.CastleStrength;    
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON error: {ex.Message}");
            }
        }

        public void SaveData()
        {
            var data = new ResourceData
            {
                Gold = GameResources.Gold,
                Food = GameResources.Food,
                PeopleSatisfaction = GameResources.PeopleSatisfaction,
                CastleStrength = GameResources.CastleStrength
            };
            Repository.SetData("GameResources", data);
            Debug.Log("Saved resources data");
        }
    }

    [System.Serializable]
    public class ResourceData
    {
        public int Gold;
        public int Food;
        public int PeopleSatisfaction;
        public int CastleStrength;
    }
}