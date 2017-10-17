using Newtonsoft.Json;

namespace _42run.Gameplay
{
    public class ScoreObject
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }
    }
}
