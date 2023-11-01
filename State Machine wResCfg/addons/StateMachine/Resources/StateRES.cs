// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


using System.Collections.Generic;
using Godot;

namespace StateMachine.Resources
{
	[GlobalClass, Icon("res://addons/StateMachine/Icons/SM-S-000.svg")]
	public partial class StateRES : Resource
	{
		[Export] private StateActionRES[] _actions = null;

		/// <summary>
		/// Will create a new state or return an existing one inside <paramref name="createdInstances"/>.
		/// </summary>
		internal State GetState(StateMachine stateMachine, Dictionary<Resource, object> createdInstances)
		{
			if (createdInstances.TryGetValue(this, out var obj))
				return (State)obj;

			var state = new State();
			createdInstances.Add(this, state);

			state._originRES = this;
			state._stateMachine = stateMachine;
			state._transitions = new StateTransition[0];
			state._actions = GetActions(_actions, stateMachine, createdInstances);

			return state;
		}

		private static StateAction[] GetActions(StateActionRES[] scriptableActions,
			StateMachine stateMachine, Dictionary<Resource, object> createdInstances)
		{
			int count = scriptableActions.Length;
			var actions = new StateAction[count];
			for (int i = 0; i < count; i++)
				actions[i] = scriptableActions[i].GetAction(stateMachine, createdInstances);

			return actions;
		}
	}
}
