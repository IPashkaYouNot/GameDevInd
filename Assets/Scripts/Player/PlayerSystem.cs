using InputReader;
using StatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerSystem : IDisposable
    {
        private readonly StatsController _statsController;
        private readonly PlayerEntity _playerEntity;
        private readonly PlayerBrain _playerBrain;
        private readonly List<IDisposable> _disposables;

        public PlayerSystem(PlayerEntity playerEntity, List<IEntityInputSource> inputSources)
        {
            _disposables = new();

            var statStorage = Resources.Load<StatsStorage>($"Player/{nameof(StatsStorage)}");
            var stats = statStorage.Stats
                .Select(s => s.Clone() as Stat)
                .ToList();
            _statsController = new(stats);
            _disposables.Add(_statsController);

            _playerEntity = playerEntity;
            _playerEntity.Initialize(_statsController);

            _playerBrain = new(playerEntity, inputSources);
            _disposables.Add(_playerBrain);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
