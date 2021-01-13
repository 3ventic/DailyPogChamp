using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DailyPogChamp;
using EvtSource;
using Newtonsoft.Json;

const string savePath = "/tmp/pogchamp";
var l = new object();

var lastPogChamp = long.Parse((await File.ReadAllTextAsync(savePath)).Trim('\r', '\n', ' '));

var url = Environment.GetEnvironmentVariable("URL");

if (string.IsNullOrEmpty(url))
{
    Console.WriteLine("env URL must not be empty");
    Environment.Exit(1);
}

using var evt = new EventSourceReader(new Uri(url));
evt.Disconnected += (_, _) =>
{
    Console.WriteLine("stream disconnected");
    Environment.Exit(2);
};

evt.MessageReceived += async (_, args) =>
{
    var msg = JsonConvert.DeserializeObject<Privmsg>(args.Message);
    var pogChampIndex = msg.Message.IndexOf("PogChamp", StringComparison.Ordinal);
    if (pogChampIndex <= -1 || !msg.Tags.ContainsKey("emotes")) return;
    try
    {
        var emotes = msg.Tags["emotes"].Split('/', StringSplitOptions.RemoveEmptyEntries).Select(e =>
        {
            var parts = e.Split(':');
            if (parts.Length != 2) return null;
            return new Emote
            {
                Id = long.Parse(parts[0]),
                StartIndices = parts[1].Split(',').Select(s => int.Parse(s.Split('-')[0]))
            };
        });
        var pogchamp = emotes.FirstOrDefault(e => e != null && e.StartIndices.Contains(pogChampIndex));
        lock (l)
        {
            if (pogchamp == default || pogchamp.Id <= lastPogChamp) return;
            lastPogChamp = pogchamp.Id;
            File.WriteAllText(savePath, lastPogChamp.ToString());
            Console.WriteLine($"new pogchamp {lastPogChamp}");
        }

        var message = new DiscordWebHookMessage(lastPogChamp);
        using var response = await message.ExecuteAsync();
        response?.EnsureSuccessStatusCode();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"error parsing emotes: {ex}");
    }
};

evt.Start();
Console.WriteLine("started");

await Task.Delay(-1);
