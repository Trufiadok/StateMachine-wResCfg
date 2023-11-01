// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


using System.Collections.Generic;
using Godot;

namespace StateMachine.Resources
{
	[GlobalClass, Icon("res://addons/StateMachine/Icons/SM-S-000.svg")]
	public partial class StateActionRES : Resource
	{
		/// <summary>
		/// Will create a new custom <see cref="StateAction"/> or return an existing one inside <paramref name="createdInstances"/>
		/// </summary>
		internal StateAction GetAction(StateMachine stateMachine, Dictionary<Resource, object> createdInstances)
		{
			if (createdInstances.TryGetValue(this, out var obj))
				return (StateAction)obj;

			var action = CreateAction();
			createdInstances.Add(this, action);
			action._originRES = this;
			action.Awake(stateMachine);
			return action;
		}
		protected virtual StateAction CreateAction()
		{
			throw new System.Exception("StateActionSO.CreateAction() - This is an abstract method of abstract class !!!");
		}
	}

	public partial class StateActionRES<T> : StateActionRES where T : StateAction, new()
	{
		protected override StateAction CreateAction() => new T();
	}
}
