using Godot;

namespace Mythikmon;

public enum CreatureType { Grass, Fire, Water, Dark, Flying, Dragon, Normal }

public abstract class Creature
{
    public string Name { get; set; }
    public CreatureType Type { get; set; }
    public int MaxHP { get; set; }
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Level { get; set; }
    public string Description { get; set; }
    public string SpritePath { get; set; }

    public Creature(string name, CreatureType type, int hp, int attack, int defense, int level, string description, string spritePath)
    {
        Name = name; Type = type; MaxHP = hp; HP = hp; Attack = attack; Defense = defense;
        Level = level; Description = description; SpritePath = spritePath;
    }

    public virtual int CalculateDamage() => Mathf.Max(1, Attack + Level / 2);
    public abstract string GetSpecialMove();
    public void TakeDamage(int amount) => HP = Mathf.Max(0, HP - Mathf.Max(0, amount));
    public void Heal() => HP = MaxHP;
    public bool IsFainted() => HP <= 0;
}
