using System.Collections.Generic;

namespace DailyPogChamp
{
    public class Emote
    {
        public long Id { get; init; }
        public IEnumerable<int> StartIndices { get; init; } = new List<int>();
    }
}
