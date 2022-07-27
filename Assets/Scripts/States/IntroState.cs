using System;
using Managers;
using StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace States
{
    public class IntroState : IState
    {
        public static event Action<bool> OnIntro;

        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerInput _playerInput;

        public IntroState(GameStateMachine gameStateMachine, PlayerInput playerInput)
        {
            _gameStateMachine = gameStateMachine;
            _playerInput = playerInput;
        }

        public void Enter()
        {
            UIManager.OnPlay += Play;
            
            OnIntro?.Invoke(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            _playerInput.SwitchCurrentActionMap("UI");
        }

        public void Tick()
        {
        }

        public void Exit()
        {
            UIManager.OnPlay -= Play;
            
            OnIntro?.Invoke(false);
        }

        private void Play()
        {
            _gameStateMachine.ChangeState(GameStateMachine.PrepareInvasionState);
        }
    }
}