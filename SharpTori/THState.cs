using System;

namespace SharpTori
{
    public class THState<T>
    {
        // I have no other choices
        public T State;
        private T _previousState;

        public THState()
        {
            _previousState = State = default;
        }

        /// <summary>
        /// Trigger an event when the function checking the state change returns true.
        /// </summary>
        /// <param name="callback">The function to check for the state change.</param>
        /// <returns>The result of the callback function.</returns>
        public bool Trigger(Func<T, T, bool> callback)
        {
            return callback(_previousState, State);
        }

        /// <summary>
        /// Update the state.
        /// </summary>
        public void Update()
        {
            _previousState = State;
        }
    }
}
