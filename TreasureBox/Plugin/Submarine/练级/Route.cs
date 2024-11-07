using System.Collections.Generic;

namespace TreasureBox.Plugin.Submarine.练级;

public class Route
{
    /// <summary>
    /// 
    /// </summary>
    public List<CollectionItem> Collection { get; set; }
}
public class CollectionItem
{
    /// <summary>
    /// 
    /// </summary>
    public int Distance { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Exp { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int ExpPerMinute { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int MapID { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Range { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int Rank { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<int> Route { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public List<RouteDetailsItem> RouteDetails { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int SectorsTotal { get; set; }
}
public class RouteDetailsItem
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }
}