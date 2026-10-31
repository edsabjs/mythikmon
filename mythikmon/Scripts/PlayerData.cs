using Godot;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace Mythikmon;

public class PlayerData
{
    public string PlayerName { get; set; } = "Trainer";
    public Vector2 Position { get; set; } = new(480, 270);
    public List<string> CapturedCreatureNames { get; set; } = new();
    public int Talismans { get; set; } = 5;

    public void Save()
    {
        string path = ProjectSettings.GlobalizePath("user://mythikmon_save.json");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static PlayerData Load()
    {
        string path = ProjectSettings.GlobalizePath("user://mythikmon_save.json");
        if (!File.Exists(path)) return new PlayerData();
        try { return JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(path)) ?? new PlayerData(); }
        catch { return new PlayerData(); }
    }
}
