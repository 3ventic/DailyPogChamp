using System.Collections.Generic;

namespace DailyPogChamp
{
    public class Emote
    {
        public string Id { get; init; } = string.Empty;
        public IEnumerable<int> StartIndices { get; init; } = new List<int>();
    }
}
