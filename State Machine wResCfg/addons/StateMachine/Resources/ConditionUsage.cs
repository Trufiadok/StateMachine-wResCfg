// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


using Godot;

namespace StateMachine.Resources
{
	[GlobalClass, Icon("res://addons/StateMachine/Icons/SM-CU-000.svg")]
	public partial class ConditionUsage : Resource
	{
		public enum Result { True, False }
		public enum LogicOperator { And, Or }

		[Export] public Result ExpectedResult;
		[Export] public StateConditionRES Condition;
		[Export] public LogicOperator Operator;
	}
}