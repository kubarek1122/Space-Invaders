using StateMachine;
using States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class GameStateMachine : StateMachineMB
    {
        public static IntroState IntroState { get; private set; }
        public static PrepareInvasionState PrepareInvasionState { get; private set; }
        public static GameplayState GameplayState { get; private set; }
        public static PauseState PauseState { get; private set; }
        public static FinishState FinishState { get; private set; }

        [SerializeField] private GameObject player;

        private void Awake()
        {
            var playerController = player.GetComponent<PlayerController>();
            var playerInput = player.GetComponent<PlayerInput>();
        
            IntroState = new IntroState(this, playerInput);
            PrepareInvasionState = new PrepareInvasionState(this, playerController, playerInput);
            GameplayState = new GameplayState(this, playerController, playerInput);
            PauseState = new PauseState(this, playerInput);
            FinishState = new FinishState(this, playerController, playerInput);
        }

        private void Start()
        {
            ChangeState(IntroState);
        }
    }
}