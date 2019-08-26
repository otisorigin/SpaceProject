using System;

public class Player
{
    public Player(String name, Unit[] unitGroup)
    {
        Name = name;
        UnitGroup = unitGroup;
    }
    
    public String Name { get; set; }
    public Unit[] UnitGroup { get; set; }
}