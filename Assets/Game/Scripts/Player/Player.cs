using System;

public class Player
{
    public Player(string name, Unit[] unitGroup)
    {
        Name = name;
        UnitGroup = unitGroup;
    }
    
    public string Name { get; set; }
    public Unit[] UnitGroup { get; set; }
}