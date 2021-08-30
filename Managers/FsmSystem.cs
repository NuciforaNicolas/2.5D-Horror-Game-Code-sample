using System.Collections.Generic;
using System.Linq;
using Character_Scripts;
using UnityEngine;

namespace Managers
{
    public enum StateId
    {
        NullStateId = 0,
        Idle        = 1,
        Walking     = 2,
        Alert       = 3,
        Patrolling  = 4,
        Chasing     = 5,
        Attacking   = 6,
        Distracted  = 7,
        Hit         = 8,
        Scared      = 9,
        StrMng_Calm = 10,
        StrMng_Jump = 11,
        StrMng_Run = 12,
        StrMng_ObjFall = 13,
        StrMng_Lightning = 14,
    }

    public enum Transition
    {
        NullTransition   = 0,
        StartWalking     = 1,
        StopWalking      = 2,
        FindPlayer       = 3,
        GoChasing        = 4,
        AttackPlayer     = 5,
        BecomeAlerted    = 6,
        ReceiveDamage    = 7,
        BecomeScared     = 8,
        BecomeDistracted = 9,
        StrMng_BeCalm = 10,
        StrMng_Jumping = 11,
        StrMng_Running = 12,
        StrMng_ObjFalling = 13,
        StrMng_LightningStrike = 14,
    }

    public abstract class FsmState
    {
        protected Dictionary<Transition, StateId> Map = new Dictionary<Transition, StateId>();
        protected StateId StateId;
        public StateId Id => StateId;

        public HashSet<Transition> BlockedTransitions { get; } = new HashSet<Transition>();

        protected Player Player;
        protected IA Npc;

        public void AddTransition(Transition trans, StateId id)
        {
            if (trans == Transition.NullTransition)
            {
                DebugManager.LogError("Finite State Machine error: NullTransition is not allowed for a real transition!");
                return;
            }

            if (id == StateId.NullStateId)
            {
                DebugManager.LogError("Finite State Machine error: NullStateId is not allowed for a real id!");
                return;
            }

            if (Map.ContainsKey(trans))
            {
                DebugManager.LogError("Finite State Machine error: State " + StateId + " already has a transition " +
                               trans);
                return;
            }

            Map.Add(trans, id);
        }

        public void DeleteTransition(Transition trans)
        {
            if (trans == Transition.NullTransition)
            {
                DebugManager.LogError("Finite State Machine error: NullTransition is not allowed!");
                return;
            }

            if (Map.ContainsKey(trans))
            {
                Map.Remove(trans);
                return;
            }

            DebugManager.LogError("Finite State Machine error: Transition " + trans + "passed to " + StateId +
                           " was not on transition list!");
        }

        public StateId GetOutputState(Transition trans)
        {
            return Map.ContainsKey(trans) ? Map[trans] : StateId.NullStateId;
        }

        public virtual void DoBeforeEntering(object options)
        {
        }

        public virtual void DoBeforeLeaving(object options)
        {
        }

        public abstract void Reason();

        public abstract void Act();
    }

    public class FsmSystem
    {
        private readonly List<FsmState> _states;

        public FsmSystem()
        {
            _states = new List<FsmState>();
        }

        public StateId CurrentStateId { get; private set; }

        public FsmState CurrentState { get; private set; }

        public void AddState(FsmState s)
        {
            if (s == null)
            {
                DebugManager.LogError("Finite State Machine error: Null reference not allowed!");
                return;
            }

            if (_states.Count == 0)
            {
                _states.Add(s);
                CurrentState = s;
                CurrentStateId = s.Id;
                return;
            }

            foreach (var state in _states.Where(state => state.Id == s.Id))
            {
                DebugManager.LogError("Finite State Machine error: Impossible to add state " + state +
                               " because it has already been added!");
                return;
            }

            _states.Add(s);
        }

        public void DeleteState(StateId id)
        {
            if (id == StateId.NullStateId)
            {
                DebugManager.LogError("Finite State Machine error: NullStateId not allowed!");
                return;
            }

            foreach (var state in _states.Where(state => state.Id == id))
            {
                _states.Remove(state);
                return;
            }

            DebugManager.LogError("Finite State Machine error: Impossible to delete state " + id +
                           ". It wasn't on the list of states");
        }

        public void PerformTransition(Transition trans, object options)
        {
            if (trans == Transition.NullTransition)
            {
                DebugManager.LogError("Finite State Machine error: NullTransition is not allowed!");
                return;
            }

            var id = CurrentState.GetOutputState(trans);
            if (id == StateId.NullStateId)
            {
                if (!CurrentState.BlockedTransitions.Contains(trans))
                {
                    DebugManager.LogError("Finite State Machine error: State" + CurrentStateId +
                               " doesn't not have a target state for transition " + trans);
                }
                
                return;
            }

            CurrentStateId = id;
            foreach (var state in _states.Where(state => state.Id == CurrentStateId))
            {
                CurrentState.DoBeforeLeaving(options);
                CurrentState = state;
                CurrentState.DoBeforeEntering(options);
                break;
            }
        }
    }
}