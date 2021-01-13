using System;
using System.IO;
using System.Linq;
using DailyPogChamp;
using EvtSource;
using Newtonsoft.Json;

const string savePath = "/tmp/pogchamp";

var lastPogChamp = await File.ReadAllTextAsync(savePath);

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
        var emotes = msg.Tags["emotes"].Split(',').Select(e =>
        {
            var parts = e.Split(':');
            var indices = parts[1].Split('-');
            return new Emote
            {
                Id = parts[0],
                FirstIndex = int.Parse(indices[0]),
            };
        });
        var pogchamp = emotes.First(e => e.FirstIndex == pogChampIndex);
        if (pogchamp.Id == lastPogChamp) return;
        lastPogChamp = pogchamp.Id;
        await File.WriteAllTextAsync(savePath, lastPogChamp);
        await new DiscordWebHookMessage(lastPogChamp).Execute();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"error parsing emotes: {ex}");
    }
};

evt.Start();
