# Mythikmon - Godot 4 C# Starter Project

A small playable prototype for a 2D top-down creature-collecting RPG inspired by Philippine mythology.

## Requirements
- Godot 4 .NET edition (not the standard/non-.NET edition)
- .NET 8 SDK

## Open and run
1. Extract the ZIP file.
2. Open Godot 4 .NET.
3. Import the `project.godot` file inside the `Mythikmon` folder.
4. Let Godot restore/build the C# project.
5. Press F6 or F5 to run.

## Controls
- WASD or Arrow Keys: Move
- Enter: Start game / continue from some screens
- E: Save while exploring
- C: Open creature collection
- Esc: Return to menu / close collection
- Battle: 1 Attack, 2 Use Talisman, 3 Run, Enter to continue after a battle

## Included prototype features
- Main menu
- Top-down village exploration
- Random wild creature encounters while moving
- Turn-based battle actions
- Talisman binding system
- Creature collection screen
- Local JSON save data
- Five starter mythical creature designs

## Important notes
- This is a starter prototype, not a finished commercial game.
- The graphics are original simple pixel-art placeholders made for this prototype. Replace or improve them as the project grows.
- The battle and bind mechanics are intentionally basic and should be expanded and tested by the group.
- `Creature` is a shared model class. The next OOP improvement is to create creature subclasses (for example `Tikbalang : Creature`) that override unique abilities, and to introduce interfaces for battle actions.

## OOP examples included
- **Abstraction:** `Creature` is an abstract class with the abstract `GetSpecialMove()` method.
- **Inheritance:** `TikbalangCreature`, `ManananggalCreature`, `BakunawaCreature`, `KapreCreature`, and `SigbinCreature` inherit from `Creature`.
- **Overriding / polymorphism:** each creature overrides `GetSpecialMove()` and `CalculateDamage()`.
- **Encapsulation:** creature and player data use properties with controlled methods for damage, healing, and saving.
- **Collections:** the game stores creature objects in `List<Creature>`.
- **Enums:** `CreatureType` categorizes creature types.


## Talisman mechanic
The player starts with five talismans. In battle, press 2 to use one. The chance of binding a wild creature increases as its HP gets lower. A successful bind adds that creature to the party.
