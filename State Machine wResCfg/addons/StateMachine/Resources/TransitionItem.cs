// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)

using Godot;

namespace StateMachine.Resources
{
	[GlobalClass, Icon("res://addons/StateMachine/Icons/SM-TI-000.svg")]
	public partial class TransitionItem : Resource
	{
		[Export] public StateRES FromState;
		[Export] public StateRES ToState;
		[Export] public ConditionUsage[] Conditions;
	}
}