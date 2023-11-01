// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)

using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace StateMachine.Resources
{
	[GlobalClass, Icon("res://addons/StateMachine/Icons/SM-TT-000.svg")]
	public partial class TransitionTableRES : Resource
	{
		[Export] private TransitionItem[] _transitions = default;

				/// <summary>
		/// Will get the initial state and instantiate all subsequent states, transitions, actions and conditions.
		/// </summary>
		internal State GetInitialState(StateMachine stateMachine)
		{
			var states = new List<State>();
			var transitions = new List<StateTransition>();
			var createdInstances = new Dictionary<Resource, object>();

			var fromStates = _transitions.GroupBy(transition => transition.FromState);

			foreach (var fromState in fromStates)
			{
				if (fromState.Key == null)
					throw new ArgumentNullException(nameof(fromState.Key), $"TransitionTable: {ResourceName}");

				var state = fromState.Key.GetState(stateMachine, createdInstances);
				states.Add(state);

				transitions.Clear();
				foreach (var transitionItem in fromState)
				{
					if (transitionItem.ToState == null)
						throw new ArgumentNullException(nameof(transitionItem.ToState), $"TransitionTable: {ResourceName}, From State: {fromState.Key.ResourceName}");

					var toState = transitionItem.ToState.GetState(stateMachine, createdInstances);
					ProcessConditionUsages(stateMachine, transitionItem.Conditions, createdInstances, out var conditions, out var resultGroups);
					transitions.Add(new StateTransition(toState, conditions, resultGroups));
				}

				state._transitions = transitions.ToArray();
			}

			return states.Count > 0 ? states[0]
				: throw new InvalidOperationException($"TransitionTable {ResourceName} is empty.");
		}

		private static void ProcessConditionUsages(
			StateMachine stateMachine,
			ConditionUsage[] conditionUsages,
			Dictionary<Resource, object> createdInstances,
			out StateCondition[] conditions,
			out int[] resultGroups)
		{
			int count = conditionUsages.Length;
			conditions = new StateCondition[count];
			for (int i = 0; i < count; i++)
				conditions[i] = conditionUsages[i].Condition.GetCondition(
					stateMachine, conditionUsages[i].ExpectedResult == ConditionUsage.Result.True, createdInstances);


			List<int> resultGroupsList = new List<int>();
			for (int i = 0; i < count; i++)
			{
				// GD.Print("conditionUsages[" + i.ToString() + "]");

				int idx = resultGroupsList.Count;
				resultGroupsList.Add(1);
				while (i < count - 1 && conditionUsages[i].Operator == ConditionUsage.LogicOperator.And)
				{
					// GD.Print(String.Format("i = {0}, operator: {1}", i, conditionUsages[i].Operator == ConditionUsage.LogicOperator.And ? "And" : "Or"));

					i++;
					resultGroupsList[idx]++;
				}
			}

			resultGroups = resultGroupsList.ToArray();
		}
	}
}
