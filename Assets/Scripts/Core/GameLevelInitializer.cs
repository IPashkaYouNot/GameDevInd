using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GameLevelInitializer : MonoBehaviour
    {
        [SerializeField] private PlayerEntity _playerEntity;
        [SerializeField] private GameUIInputView _gameUIInputView;

        private ExternalDevicesInputReader _externalDevicesInputReader;
        private PlayerBrain _playerBrain;

        private bool _onPause;
        private void Awake()
        {
            _externalDevicesInputReader = new();
            _playerBrain = new(_playerEntity, new()
            {
                _gameUIInputView,
                _externalDevicesInputReader
            });
        }
        private void Update()
        {
            if (_onPause)
                return;
            _externalDevicesInputReader.OnUpdate();
        }

        private void FixedUpdate()
        {
            if (_onPause)
                return;
            _playerBrain.OnFixedUpdate();
        }
    }
}
