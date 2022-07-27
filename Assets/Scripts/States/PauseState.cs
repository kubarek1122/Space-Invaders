using System;
using Managers;
using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class PauseState : IState
    {
        public static event Action<bool> OnPause;

        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerInput _playerInput;

        public PauseState(GameStateMachine gameStateMachine, PlayerInput playerInput)
        {
            _gameStateMachine = gameStateMachine;
            _playerInput = playerInput;
        }

        public void Enter()
        {
            UIManager.OnResume += Resume;
            UIManager.OnGameExit += GameExit;
            
            OnPause?.Invoke(true);

            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _playerInput.SwitchCurrentActionMap("UI");
        }

        public void Tick()
        {
            if(_playerInput.actions["Cancel"].WasPerformedThisFrame())
                _gameStateMachine.ChangeState(GameStateMachine.GameplayState);
        }

        public void Exit()
        {
            UIManager.OnResume -= Resume;
            UIManager.OnGameExit -= GameExit;
            
            OnPause?.Invoke(false);
            
            Time.timeScale = 1.0f;
        }

        private void Resume()
        {
            _gameStateMachine.ChangeState(GameStateMachine.GameplayState);
        }

        private void GameExit()
        {
            EnemyManager.Instance.DespawnEnemies();
            _gameStateMachine.ChangeState(GameStateMachine.IntroState);
        }
    }
}