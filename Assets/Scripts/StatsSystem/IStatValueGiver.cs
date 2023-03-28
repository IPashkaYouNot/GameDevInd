using StatsSystem.Enums;

namespace StatsSystem
{
    public interface IStatValueGiver
    {
        float GetStatValue(StatType statType);
    }
}
