using Managers;
using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class GameplayState : IState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerController _playerController;
        private readonly PlayerInput _playerInput;

        public GameplayState(GameStateMachine gameStateMachine, PlayerController playerController, PlayerInput playerInput)
        {
            _gameStateMachine = gameStateMachine;
            _playerInput = playerInput;
            _playerController = playerController;
        }

        public void Enter()
        {
            _playerInput.SwitchCurrentActionMap("Player");

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            EnemyManager.Instance.BeginFiring();
        }

        public void Tick()
        {
            if (EnemyManager.Instance.GetActiveEnemiesCount() == 0)
            {
                EnemyManager.Instance.SpawnEnemies();
                EnemyManager.Instance.BeginFiring();
            }
            
            if (_playerInput.actions["Pause"].WasPerformedThisFrame())
                _gameStateMachine.ChangeState(GameStateMachine.PauseState);

            EnemyManager.Instance.MoveEnemies();
            
            if (_playerController.CurrentLives <= 0)
            {
                _gameStateMachine.ChangeState(GameStateMachine.FinishState);
            }
        }

        public void Exit()
        {
        }
    }
}