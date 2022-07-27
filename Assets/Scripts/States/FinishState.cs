using System;
using Managers;
using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class FinishState : IState
    {
        public static event Action<int> OnFinish;
        
        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerController _playerController;
        private readonly PlayerInput _playerInput;

        public FinishState(GameStateMachine gameStateMachine, PlayerController playerController, PlayerInput playerInput)
        {
            _gameStateMachine = gameStateMachine;
            _playerController = playerController;
            _playerInput = playerInput;
        }

        public void Enter()
        {
            UIManager.OnGameExit += GameExit;
            
            OnFinish?.Invoke(_playerController.CurrentScore);

            EnemyManager.Instance.DespawnEnemies();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _playerInput.SwitchCurrentActionMap("UI");
        }

        public void Tick()
        {
        }

        public void Exit()
        {
            UIManager.OnGameExit -= GameExit;
        }

        private void GameExit()
        {
            _gameStateMachine.ChangeState(GameStateMachine.IntroState);
        }
    }
}