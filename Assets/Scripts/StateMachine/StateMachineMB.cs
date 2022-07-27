using UnityEngine;

namespace StateMachine
{
    public abstract class StateMachineMB : MonoBehaviour
    {
        private IState CurrentState { get; set; }

        private void Update()
        {
            CurrentState?.Tick();
        }
        
        /// <summary>
        /// Method used to change state of state machine
        /// </summary>
        /// <param name="newState">New state</param>
        public void ChangeState(IState newState)
        {
            CurrentState?.Exit();
            CurrentState = newState;
            CurrentState?.Enter();
        }
    }
}