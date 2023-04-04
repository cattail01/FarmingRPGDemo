/// <summary>
/// 地皮参数细节类
/// </summary>
/// <remarks>
/// <para>可序列化的类</para>
/// <para>用于保存地皮信息</para>
/// </remarks>
[System.Serializable]
public class GridPropertyDetails
{
    public int GridX;
    public int GridY;

    public bool IsDiggable = false;
    public bool CanDropItem = false;
    public bool CanPlaceFurniture = false;
    public bool IsPath = false;
    public bool IsNpcObstacle = false;

    public int DaysSinceDug = -1;
    public int DaysSinceWatered = -1;
    public int SeedItemCode = -1;
    public int GrowthDays = -1;
    public int DaysSinceLastHarvest = -1;

    public GridPropertyDetails()
    {
    }
}