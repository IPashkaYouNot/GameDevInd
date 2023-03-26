using InputReader;
using System.Collections.Generic;

namespace Player
{
    public class PlayerSystem
    {
        private readonly PlayerEntity _playerEntity;
        private readonly PlayerBrain _playerBrain;

        public PlayerSystem(PlayerEntity playerEntity, List<IEntityInputSource> inputSources)
        {
            _playerEntity = playerEntity;
            _playerBrain = new(playerEntity, inputSources);
        }

    }
}
