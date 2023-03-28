using Core.Services.Updater;
using InputReader;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GameLevelInitializer : MonoBehaviour
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private GameUIInputView _gameUIInputView;

        private ExternalDevicesInputReader _externalDevicesInputReader;
        private PlayerSystem _playerSystem;
        private ProjectUpdater _projectUpdater;

        private List<IDisposable> _disposables;

        private bool _onPause;
        private void Awake()
        {
            _disposables = new();

            if (ProjectUpdater.Instance is null)
                _projectUpdater = new GameObject().AddComponent<ProjectUpdater>();
            else
                _projectUpdater = ProjectUpdater.Instance as ProjectUpdater;

            _externalDevicesInputReader = new();
            _disposables.Add(_externalDevicesInputReader);

            _playerSystem = new(_playerEntity, new()
            {
                _gameUIInputView,
                _externalDevicesInputReader
            });
            _disposables.Add(_playerSystem);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _projectUpdater.IsPaused = !_projectUpdater.IsPaused;
            }
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
