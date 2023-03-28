using Core.Services.Updater;
using StatsSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StatsSystem
{
    public class StatsController : IStatValueGiver, IDisposable
    {
        private readonly List<Stat> _currentStats;
        private readonly List<StatModificator> _currentModificators;

        public StatsController(List<Stat> currentStats)
        {
            _currentStats = currentStats;
            _currentModificators = new();
            ProjectUpdater.Instance.UpdateCalled += OnUpdate;
        }

        public float GetStatValue(StatType type) => 
            _currentStats.First(s => s.Type == type);

        public void ProccessModificator(StatModificator statModificator)
        {
            var statToChange = _currentStats.Find(stat => stat.Type == statModificator.Stat.Type);

            if (statToChange is null)
                return;

            var addedValue = 
                statModificator.StatModificatorType == StatModificatorType.Additive 
                    ? statToChange + statModificator.Stat
                    : statToChange * statModificator.Stat;

            statToChange.SetStatValue(statToChange + addedValue);

            if (statModificator.Duration < 0)
                return;

            if (_currentModificators.Contains(statModificator))
                _currentModificators.Remove(statModificator);
            else
            {
                var addedStat = new Stat(statModificator.Stat.Type, -addedValue);
                var tempModificator = 
                    new StatModificator
                    (
                        addedStat, 
                        StatModificatorType.Additive, 
                        statModificator.Duration, 
                        Time.time
                    );
                _currentModificators.Add(tempModificator);
            }
        }
        public void Dispose() => ProjectUpdater.Instance.UpdateCalled -= OnUpdate;

        private void OnUpdate()
        {
            if(_currentModificators.Count == 0)
                return;

            var expiredModificators = _currentModificators
                .Where(m => m.StartTime + m.Duration <= Time.time);

            foreach (var em in expiredModificators)
                ProccessModificator(em);
        }
    }
}
