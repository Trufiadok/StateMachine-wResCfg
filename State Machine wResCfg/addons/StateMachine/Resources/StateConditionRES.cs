// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


using System.Collections.Generic;
using Godot;

namespace StateMachine.Resources
{
	[GlobalClass, Icon("res://addons/StateMachine/Icons/SM-C-000.svg")]
	public partial class StateConditionRES : Resource
	{
		/// <summary>
		/// Will create a new custom <see cref="Condition"/> or use an existing one inside <paramref name="createdInstances"/>.
		/// </summary>
		internal StateCondition GetCondition(StateMachine stateMachine, bool expectedResult, Dictionary<Resource, object> createdInstances)
		{
			if (!createdInstances.TryGetValue(this, out var obj))
			{
				var condition = CreateCondition();
				condition._originRES = this;
				createdInstances.Add(this, condition);
				condition.Awake(stateMachine);

				obj = condition;
			}

			return new StateCondition(stateMachine, (Condition)obj, expectedResult);
		}

		protected virtual Condition CreateCondition()
		{
			throw new System.Exception("StateConditionSO.CreateCondition() - This is an abstract method of abstract class !!!");
		}
	}

	// public abstract class StateConditionRES<T> : StateConditionRES where T : Condition, new()
	// {
	// 	protected override Condition CreateCondition() => new T();
	// }
}
