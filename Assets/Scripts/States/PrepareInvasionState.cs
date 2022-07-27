using System;
using Managers;
using StateMachine;
using UnityEngine.InputSystem;

namespace States
{
    public class PrepareInvasionState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerController _playerController;
        private readonly PlayerInput _playerInput;

        public static event Action<bool> OnPrepare;

        public PrepareInvasionState(GameStateMachine gameStateMachine, PlayerController playerController, PlayerInput playerInput)
        {
            _gameStateMachine = gameStateMachine;
            _playerController = playerController;
            _playerInput = playerInput;
        }

        public void Enter()
        {
            OnPrepare?.Invoke(true);
            
            _playerController.ResetPlayer();
            
            EnemyManager.Instance.SpawnEnemies();
            EnemyManager.Instance.ResetBarriers();
        }

        public void Tick()
        {
            if (_playerInput.actions["Start"].WasPerformedThisFrame())
            {
                OnPrepare?.Invoke(false);
                _gameStateMachine.ChangeState(GameStateMachine.GameplayState);
            }
        }

        public void Exit()
        {
        }
    }
}