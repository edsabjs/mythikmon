namespace Mythikmon;

// Each creature inherits shared statistics and behavior from the abstract Creature class.
// The overridden methods give every mythical creature its own special move.
public class TikbalangCreature : Creature
{
    public TikbalangCreature(int level = 3) : base("Tikbalang", CreatureType.Grass, 34 + level, 9 + level, 5 + level / 2, level,
        "A horse-headed spirit known for confusing travelers.", "res://Assets/tikbalang.png") { }
    public override string GetSpecialMove() => "Forest Trick";
    public override int CalculateDamage() => base.CalculateDamage() + 2;
}

public class ManananggalCreature : Creature
{
    public ManananggalCreature(int level = 4) : base("Manananggal", CreatureType.Dark, 30 + level, 11 + level, 4 + level / 2, level,
        "A mysterious night creature with powerful wings.", "res://Assets/manananggal.png") { }
    public override string GetSpecialMove() => "Midnight Dive";
    public override int CalculateDamage() => base.CalculateDamage() + 3;
}

public class BakunawaCreature : Creature
{
    public BakunawaCreature(int level = 7) : base("Bakunawa", CreatureType.Dragon, 48 + level, 14 + level, 9 + level / 2, level,
        "A legendary serpent associated with the moon.", "res://Assets/bakunawa.png") { }
    public override string GetSpecialMove() => "Lunar Tide";
    public override int CalculateDamage() => base.CalculateDamage() + 5;
}

public class KapreCreature : Creature
{
    public KapreCreature(int level = 5) : base("Kapre", CreatureType.Fire, 42 + level, 10 + level, 10 + level / 2, level,
        "A giant guardian associated with old trees.", "res://Assets/kapre.png") { }
    public override string GetSpecialMove() => "Ember Smoke";
    public override int CalculateDamage() => base.CalculateDamage() + 2;
}

public class SigbinCreature : Creature
{
    public SigbinCreature(int level = 4) : base("Sigbin", CreatureType.Dark, 28 + level, 12 + level, 3 + level / 2, level,
        "A mysterious creature said to move through shadows.", "res://Assets/sigbin.png") { }
    public override string GetSpecialMove() => "Shadow Pounce";
    public override int CalculateDamage() => base.CalculateDamage() + 3;
}
