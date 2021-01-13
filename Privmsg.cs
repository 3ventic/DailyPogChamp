using Newtonsoft.Json;
using System.Collections.Generic;

namespace DailyPogChamp
{
    public class Privmsg
    {
        [JsonIgnore] public Dictionary<string, string> Tags { get; } = new();

        [JsonProperty("command")] public string Command { get; set; } = "I_NONE";
        [JsonProperty("room")] public string Room { get; set; } = "";
        [JsonIgnore] public string Channel => string.IsNullOrEmpty(Room) ? Target : Room;
        [JsonProperty("nick")] public string User { get; set; } = "";
        [JsonProperty("target")] public string Target { get; set; } = "";
        [JsonProperty("body")] public string Message { get; set; } = "";

        [JsonIgnore] private string _tagString = "";
        [JsonProperty("tags")]
        public string TagString
        {
            get => _tagString;
            set
            {
                var tagsParts = value.Split(';');
                foreach (var tag in tagsParts)
                {
                    var eqIndex = tag.IndexOf('=');
                    if (eqIndex == -1)
                    {
                        Tags[tag] = "true";
                    }
                    else
                    {
                        // Unescape tag value
                        Tags[tag.Substring(0, eqIndex)] = tag.Substring(eqIndex + 1)
                            .Replace(@"\:", ";")
                            .Replace(@"\s", " ")
                            .Replace(@"\\", @"\")
                            .Replace(@"\r", "\r")
                            .Replace(@"\n", "\n");
                    }
                }
                _tagString = value;
            }
        }

        public override string ToString() => $"{_tagString} {Command} {Channel} {User}{(string.IsNullOrEmpty(Target) ? string.Empty : $" > {Target}")}: {Message}";
    }
}
