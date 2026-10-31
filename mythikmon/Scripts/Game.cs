using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mythikmon;

public partial class Game : Node2D
{
    private enum Screen { Menu, Explore, Battle, Collection }
    private Screen screen = Screen.Menu;
    private readonly Random random = new();
    private readonly List<Creature> party = new();
    private readonly List<Creature> wildPool = new();
    private Creature? activeCreature;
    private Creature? wildCreature;
    private PlayerData player = new();
    private Vector2 playerPosition = new(480, 270);
    private Vector2 playerDirection = Vector2.Down;
    private string message = "Welcome to the world of Mythikmon!";
    private string battleMessage = "A wild creature appeared!";
    private int encounterCooldown;
    private Texture2D? playerTexture;
    private Texture2D? logoTexture;
    private Texture2D? talismanTexture;
    private readonly Dictionary<string, Texture2D> creatureTextures = new();

    public override void _Ready()
    {
        playerTexture = LoadTexture("res://Assets/player.png");
        logoTexture = LoadTexture("res://Assets/logo.png");
        talismanTexture = LoadTexture("res://Assets/talisman.png");
        wildPool.Add(new TikbalangCreature());
        wildPool.Add(new ManananggalCreature());
        wildPool.Add(new BakunawaCreature());
        wildPool.Add(new KapreCreature());
        wildPool.Add(new SigbinCreature());
        foreach (var c in wildPool) creatureTextures[c.Name] = LoadTexture(c.SpritePath)!;
        party.Add(new TikbalangCreature(5));
        activeCreature = party[0];
        player = PlayerData.Load();
        playerPosition = player.Position;
        QueueRedraw();
    }

    private Texture2D? LoadTexture(string path)
    {
        if (!ResourceLoader.Exists(path)) return null;
        return GD.Load<Texture2D>(path);
    }

    public override void _Process(double delta)
    {
        if (screen != Screen.Explore) return;
        Vector2 move = Vector2.Zero;
        if (Input.IsKeyPressed(Key.W) || Input.IsKeyPressed(Key.Up)) move.Y -= 1;
        if (Input.IsKeyPressed(Key.S) || Input.IsKeyPressed(Key.Down)) move.Y += 1;
        if (Input.IsKeyPressed(Key.A) || Input.IsKeyPressed(Key.Left)) move.X -= 1;
        if (Input.IsKeyPressed(Key.D) || Input.IsKeyPressed(Key.Right)) move.X += 1;
        if (move != Vector2.Zero)
        {
            playerDirection = move.Normalized();
            playerPosition += playerDirection * 150f * (float)delta;
            playerPosition.X = Mathf.Clamp(playerPosition.X, 32, 928);
            playerPosition.Y = Mathf.Clamp(playerPosition.Y, 80, 500);
            encounterCooldown++;
            if (encounterCooldown > 55 && random.NextDouble() < 0.018)
            {
                encounterCooldown = 0;
                StartBattle();
            }
            QueueRedraw();
        }
    }

    public override void _UnhandledInput(InputEvent e)
    {
        if (e is not InputEventKey key || !key.Pressed || key.Echo) return;
        if (screen == Screen.Menu)
        {
            if (key.Keycode == Key.Enter || key.Keycode == Key.N) { screen = Screen.Explore; message = "Explore the grass to encounter mythical creatures. Press E to save."; }
            else if (key.Keycode == Key.C) screen = Screen.Collection;
            else if (key.Keycode == Key.Escape) GetTree().Quit();
        }
        else if (screen == Screen.Explore)
        {
            if (key.Keycode == Key.E) { player.Position = playerPosition; player.Save(); message = "Game saved! Keep exploring."; }
            if (key.Keycode == Key.C) screen = Screen.Collection;
            if (key.Keycode == Key.Escape) screen = Screen.Menu;
        }
        else if (screen == Screen.Battle)
        {
            if (key.Keycode == Key.Key1) Attack();
            else if (key.Keycode == Key.Key2) UseTalisman();
            else if (key.Keycode == Key.Key3) { screen = Screen.Explore; battleMessage = "You escaped safely."; }
            else if (key.Keycode == Key.Enter && (wildCreature == null || wildCreature.IsFainted())) screen = Screen.Explore;
        }
        else if (screen == Screen.Collection && (key.Keycode == Key.Escape || key.Keycode == Key.Enter)) screen = Screen.Explore;
        QueueRedraw();
    }

    private void StartBattle()
    {
        wildCreature = Clone(wildPool[random.Next(wildPool.Count)]);
        screen = Screen.Battle;
        battleMessage = $"A wild {wildCreature.Name} appeared!";
    }

    private Creature Clone(Creature c) => c.Name switch
    {
        "Tikbalang" => new TikbalangCreature(c.Level),
        "Manananggal" => new ManananggalCreature(c.Level),
        "Bakunawa" => new BakunawaCreature(c.Level),
        "Kapre" => new KapreCreature(c.Level),
        "Sigbin" => new SigbinCreature(c.Level),
        _ => new TikbalangCreature(c.Level)
    };

    private void Attack()
    {
        if (wildCreature == null || activeCreature == null || wildCreature.IsFainted()) return;
        int damage = Mathf.Max(1, activeCreature.CalculateDamage() - wildCreature.Defense / 3);
        wildCreature.TakeDamage(damage);
        if (wildCreature.IsFainted()) battleMessage = $"You defeated {wildCreature.Name}! Press ENTER to continue.";
        else
        {
            int enemyDamage = Mathf.Max(1, wildCreature.CalculateDamage() - activeCreature.Defense / 3);
            activeCreature.TakeDamage(enemyDamage);
            if (activeCreature.IsFainted()) { activeCreature.Heal(); battleMessage = "Your creature recovered. Press ENTER to leave the battle."; wildCreature = null; }
            else battleMessage = $"You dealt {damage} damage. {wildCreature.Name} hit back for {enemyDamage}!";
        }
    }

    private void UseTalisman()
    {
        if (wildCreature == null || wildCreature.IsFainted()) { battleMessage = "There is no wild creature to bind."; return; }
        if (player.Talismans <= 0) { battleMessage = "You are out of talismans!"; return; }
        player.Talismans--;
        int chance = 35 + (wildCreature.MaxHP - wildCreature.HP) * 100 / wildCreature.MaxHP;
        if (random.Next(100) < chance)
        {
            party.Add(Clone(wildCreature));
            player.CapturedCreatureNames.Add(wildCreature.Name);
            player.Save();
            battleMessage = $"{wildCreature.Name} was bound by the talisman! Press ENTER to continue.";
            wildCreature = null;
        }
        else
        {
            int hit = Mathf.Max(1, wildCreature.CalculateDamage() - (activeCreature?.Defense ?? 0) / 3);
            activeCreature?.TakeDamage(hit);
            battleMessage = $"The creature broke free! It dealt {hit} damage.";
        }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(0, 0, 960, 540), new Color("101d32"));
        if (screen == Screen.Menu) DrawMenu();
        else if (screen == Screen.Explore) DrawExplore();
        else if (screen == Screen.Battle) DrawBattle();
        else DrawCollection();
    }

    private void DrawMenu()
    {
        DrawRect(new Rect2(0, 0, 960, 540), new Color("244d66"));
        DrawRect(new Rect2(0, 350, 960, 190), new Color("316a4d"));
        for (int i = 0; i < 18; i++)
        {
            float x = (i * 83) % 960; float y = 360 + (i * 37) % 170;
            DrawCircle(new Vector2(x, y), 24, new Color("397d4e"));
        }
        if (logoTexture != null) DrawTextureRect(logoTexture, new Rect2(230, 55, 500, 120), false);
        DrawPanel(new Rect2(340, 225, 280, 210));
        DrawString(ThemeDB.FallbackFont, new Vector2(390, 270), "ENTER  -  NEW GAME", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawString(ThemeDB.FallbackFont, new Vector2(390, 310), "C      -  CREATURES", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawString(ThemeDB.FallbackFont, new Vector2(390, 350), "ESC    -  EXIT", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawString(ThemeDB.FallbackFont, new Vector2(300, 490), "A 2D ADVENTURE INSPIRED BY PHILIPPINE FOLKLORE", HorizontalAlignment.Left, -1, 18, Colors.White);
    }

    private void DrawExplore()
    {
        // Simple top-down village map
        DrawRect(new Rect2(0, 0, 960, 540), new Color("5c9b57"));
        for (int x = 0; x < 960; x += 64) for (int y = 55; y < 540; y += 64) DrawRect(new Rect2(x + 8, y + 8, 3, 3), new Color("9cc66b"));
        DrawRect(new Rect2(0, 245, 960, 92), new Color("c7a16b"));
        DrawRect(new Rect2(425, 55, 100, 485), new Color("c7a16b"));
        DrawRect(new Rect2(710, 55, 250, 130), new Color("3f94c5"));
        DrawRect(new Rect2(0, 55, 960, 4), new Color("254b3c"));
        DrawHouse(new Vector2(110, 105)); DrawHouse(new Vector2(690, 340)); DrawHouse(new Vector2(790, 100));
        for (int i = 0; i < 22; i++)
        {
            Vector2 p = new((i * 137 + 30) % 940 + 10, (i * 79 + 80) % 430 + 80);
            if (Mathf.Abs(p.X - 470) < 65 || Mathf.Abs(p.Y - 290) < 55 || (p.X > 700 && p.Y < 185)) continue;
            DrawTree(p);
        }
        if (playerTexture != null) DrawTextureRect(playerTexture, new Rect2(playerPosition - new Vector2(16, 16), new Vector2(32, 32)), false);
        DrawPanel(new Rect2(12, 10, 936, 48));
        DrawString(ThemeDB.FallbackFont, new Vector2(28, 40), "MYTHIKMON  |  WASD / ARROWS: MOVE   E: SAVE   C: COLLECTION   ESC: MENU", HorizontalAlignment.Left, -1, 17, Colors.White);
        DrawPanel(new Rect2(12, 478, 936, 50));
        DrawString(ThemeDB.FallbackFont, new Vector2(28, 508), message, HorizontalAlignment.Left, 900, 16, Colors.White);
    }

    private void DrawHouse(Vector2 p)
    {
        DrawRect(new Rect2(p.X, p.Y + 25, 100, 65), new Color("a66d48"));
        DrawRect(new Rect2(p.X - 8, p.Y, 116, 38), new Color("315d8c"));
        DrawRect(new Rect2(p.X + 42, p.Y + 52, 20, 38), new Color("65432f"));
        DrawRect(new Rect2(p.X + 10, p.Y + 43, 20, 18), new Color("a9d9e9"));
    }

    private void DrawTree(Vector2 p)
    {
        DrawRect(new Rect2(p.X - 4, p.Y + 8, 8, 22), new Color("10502e"));
        DrawCircle(p, 19, new Color("23643b")); DrawCircle(p + new Vector2(-5, -5), 12, new Color("3e8a4b"));
    }

    private void DrawBattle()
    {
        DrawRect(new Rect2(0, 0, 960, 540), new Color("8bb8a0"));
        DrawRect(new Rect2(0, 320, 960, 220), new Color("6c9b62"));
        DrawEllipse(new Vector2(250, 350), new Vector2(145, 40), new Color("9cc17c"));
        DrawEllipse(new Vector2(720, 210), new Vector2(145, 40), new Color("9cc17c"));
        DrawPanel(new Rect2(25, 30, 390, 100));
        DrawString(ThemeDB.FallbackFont, new Vector2(45, 65), wildCreature?.Name ?? "Wild Creature", HorizontalAlignment.Left, -1, 24, Colors.White);
        DrawString(ThemeDB.FallbackFont, new Vector2(45, 105), $"HP: {wildCreature?.HP ?? 0} / {wildCreature?.MaxHP ?? 0}", HorizontalAlignment.Left, -1, 18, Colors.White);
        if (wildCreature != null && creatureTextures.TryGetValue(wildCreature.Name, out var wt)) DrawTextureRect(wt, new Rect2(630, 105, 180, 180), false);
        if (activeCreature != null && creatureTextures.TryGetValue(activeCreature.Name, out var pt)) DrawTextureRect(pt, new Rect2(125, 285, 180, 180), false);
        DrawPanel(new Rect2(535, 330, 400, 90));
        DrawString(ThemeDB.FallbackFont, new Vector2(555, 365), $"{activeCreature?.Name}  Lv.{activeCreature?.Level}", HorizontalAlignment.Left, -1, 22, Colors.White);
        DrawString(ThemeDB.FallbackFont, new Vector2(555, 398), $"HP: {activeCreature?.HP} / {activeCreature?.MaxHP}", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawPanel(new Rect2(25, 440, 910, 85));
        DrawString(ThemeDB.FallbackFont, new Vector2(45, 470), battleMessage, HorizontalAlignment.Left, 850, 17, Colors.White);
        if (talismanTexture != null) DrawTextureRect(talismanTexture, new Rect2(45, 490, 24, 24), false);
        DrawString(ThemeDB.FallbackFont, new Vector2(80, 505), $"1: ATTACK     2: USE TALISMAN ({player.Talismans})     3: RUN     ENTER: CONTINUE", HorizontalAlignment.Left, -1, 17, Colors.White);
    }

    private void DrawCollection()
    {
        DrawRect(new Rect2(0, 0, 960, 540), new Color("1b2d48"));
        DrawPanel(new Rect2(45, 35, 870, 470));
        DrawString(ThemeDB.FallbackFont, new Vector2(75, 82), "CREATURE COLLECTION / MYTHIKDEX", HorizontalAlignment.Left, -1, 27, Colors.White);
        int y = 135;
        foreach (var c in party)
        {
            if (creatureTextures.TryGetValue(c.Name, out var tex)) DrawTextureRect(tex, new Rect2(75, y - 28, 58, 58), false);
            DrawString(ThemeDB.FallbackFont, new Vector2(155, y + 5), $"{c.Name}   |   {c.Type}   |   Lv. {c.Level}   |   HP {c.HP}/{c.MaxHP}", HorizontalAlignment.Left, -1, 19, Colors.White);
            y += 72;
        }
        DrawString(ThemeDB.FallbackFont, new Vector2(75, 470), "Press ESC or ENTER to return", HorizontalAlignment.Left, -1, 17, Colors.White);
    }

    private void DrawPanel(Rect2 rect)
    {
        DrawRect(rect, new Color("14243d"));
        DrawRect(rect, new Color("7ba5d8"), false, 3);
    }

    private void DrawEllipse(Vector2 center, Vector2 radius, Color color)
    {
        DrawSetTransform(center, 0, radius);
        DrawCircle(Vector2.Zero, 1, color);
        DrawSetTransform(Vector2.Zero, 0, Vector2.One);
    }
}
