namespace StateMachine
{
    public interface IState
    {
        /// <summary>
        /// Method called on entering state
        /// </summary>
        public void Enter();
        
        /// <summary>
        /// Method called every update
        /// </summary>
        public void Tick();
        
        /// <summary>
        /// Method called on exiting state
        /// </summary>
        public void Exit();
    }
}