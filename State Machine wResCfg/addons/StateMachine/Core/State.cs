// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


using StateMachine.Resources;

namespace StateMachine
{
	/// <summary>
	/// An object representing a state.
	/// </summary>
	public class State
	{
		internal StateRES _originRES;
		internal StateMachine _stateMachine;
		internal StateTransition[] _transitions;
		internal StateAction[] _actions;

		internal State() { }

		/// <summary>
		/// Constructor of State class
		/// </summary>
		public State(
			StateRES originRES,
			StateMachine stateMachine,
			StateTransition[] transitions,
			StateAction[] actions)
		{
			_originRES = originRES;
			_stateMachine = stateMachine;
			_transitions = transitions;
			_actions = actions;
		}

		/// <summary>
		/// Called when entering the state.
		/// </summary>
		public void OnStateEnter()
		{
			void OnStateEnter(IStateComponent[] comps)
			{
				for (int i = 0; i < comps.Length; i++)
					comps[i].OnStateEnter();
			}
			OnStateEnter(_transitions);
			OnStateEnter(_actions);
		}

		/// <summary>
		/// Called when stay in the state.
		/// </summary>		
		public void OnUpdate()
		{
			for (int i = 0; i < _actions.Length; i++)
				_actions[i].OnUpdate();
		}

		/// <summary>
		/// Called when leaving the state.
		/// </summary>
		public void OnStateExit()
		{
			void OnStateExit(IStateComponent[] comps)
			{
				for (int i = 0; i < comps.Length; i++)
					comps[i].OnStateExit();
			}
			OnStateExit(_transitions);
			OnStateExit(_actions);
		}

		/// <summary>
		/// Checks all the conditions of the state.
		/// </summary>
		public bool TryGetTransition(out State state)
		{
			state = null;

			for (int i = 0; i < _transitions.Length; i++)
				if (_transitions[i].TryGetTransiton(out state))
					break;

			for (int i = 0; i < _transitions.Length; i++)
				_transitions[i].ClearConditionsCache();

			return state != null;
		}		
	}
}
