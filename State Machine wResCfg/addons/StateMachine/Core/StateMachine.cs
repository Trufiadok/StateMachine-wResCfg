// Copyright: 2020, Unity Open Project #1: Chop Chop
// Unity and UOP1 community
// License: Apache-2.0

// Modified by Trufiadok (trufiadokgamedev@gmail.com)


using Godot;

namespace StateMachine
{
	public partial class StateMachine : Node
	{
		[Export] private Resources.TransitionTableRES _transitionTableRES = default;

		// public DemoStateMachine demoStateMachine { get; private set; }

		// private readonly Dictionary<Type, Component> _cachedComponents = new Dictionary<Type, Component>();
		internal State _currentState;

        public string _currentStateName { get { return _currentState._originRES.ResourceName; } }
		[Export] string currentStateName;

		public override void _Ready() 
		{
			// demoStateMachine = FindNode<DemoStateMachine>("DemoStateMachine", true, false);
			_currentState = _transitionTableRES.GetInitialState(this);
			_currentState.OnStateEnter();
		}

        // public new bool TryGetComponent<T>(out T component) where T : Component
        // {
        // 	var type = typeof(T);
        // 	if (!_cachedComponents.TryGetValue(type, out var value))
        // 	{
        // 		if (base.TryGetComponent<T>(out component))
        // 			_cachedComponents.Add(type, component);

        // 		return component != null;
        // 	}

        // 	component = (T)value;
        // 	return true;
        // }

        // public T GetOrAddComponent<T>() where T : Component
        // {
        // 	if (!TryGetComponent<T>(out var component))
        // 	{
        // 		component = gameObject.AddComponent<T>();
        // 		_cachedComponents.Add(typeof(T), component);
        // 	}

        // 	return component;
        // }

        // public new T GetComponent<T>() where T : Component
        // {
        // 	return TryGetComponent(out T component)
        // 		? component : throw new InvalidOperationException($"{typeof(T).Name} not found in {name}.");
        // }

        public override void _Process(double delta) 
		{
			if (_currentState.TryGetTransition(out var transitionState))
				Transition(transitionState);

			_currentState.OnUpdate();

            currentStateName = _currentStateName;
		}

		private void Transition(State transitionState)
		{
			_currentState.OnStateExit();
			_currentState = transitionState;
			_currentState.OnStateEnter();
		}

		// public T FindNode<T>(string nodeName, bool recursive = true, bool owned = true) where T : class
		// {
		// 	T nodeObject;

		// 	nodeObject = GetTree().Root.FindChild(nodeName, recursive, owned) as T;

		// 	return nodeObject;
		// }
	}
}
