// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


namespace StateMachine
{
	interface IStateComponent
	{
		/// <summary>
		/// Called when entering the state.
		/// </summary>
		void OnStateEnter();

		/// <summary>
		/// Called when leaving the state.
		/// </summary>
		void OnStateExit();
	}
}
