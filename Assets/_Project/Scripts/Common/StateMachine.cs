using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

public class StateMachine
{
    public IState CurrentState => _currentState;
    private readonly Dictionary<Type, List<Transition>> _transitions;
    private readonly List<Transition> _anyTransitions;
    private IState _currentState;
    private List<Transition> _currentTransitions;
    private static List<Transition> EmptyTransitions;
    private bool _isActive;

    public StateMachine()
    {
        _transitions = new Dictionary<Type, List<Transition>>();
        _currentTransitions = new List<Transition>();
        _anyTransitions = new List<Transition>();
        EmptyTransitions = new List<Transition>(0);
    }

    public void Update()
    {
        if(_isActive == false) return;
        
        Transition transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        _currentState?.Update();
    }

    public void SetState(IState state)
    {
        if (state == _currentState) return;
        if (_transitions.Count == 0 )
            Debug.LogError($"Add transitions before");
        _currentState?.Exit();

        _currentState = state;
        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);

        _currentTransitions ??= EmptyTransitions;

        _currentState.Enter();
    }

    public StateMachine AddTransition(IState from, IState to, Func<bool> condition)
    {
        bool result = _transitions.TryGetValue(from.GetType(), out List<Transition> value);
        if (result == false)
        {
            value = new List<Transition>();

            Type type = from.GetType();
            _transitions[type] = value;
        }

        value.Add(new Transition(to, condition));
        return this;
    }

    public StateMachine AddAnyTransition(IState to, Func<bool> condition)
    {
        _anyTransitions.Add(new Transition(to, condition));
        return this;
    }

    private Transition GetTransition()
    {
        foreach (Transition transition in _anyTransitions)
            if (transition.Condition.Invoke())
                return transition;
        
        foreach (Transition transition in _currentTransitions)
            if (transition.Condition.Invoke())
                return transition;

        return null;
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }
}