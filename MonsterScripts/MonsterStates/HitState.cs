using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class HitState : FsmState
    {
        private readonly Monster _monster;
        
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int Hit = Animator.StringToHash("Hit");
        
        private float _timer;
        private float _probabilityScaredTransition;
        private readonly float _timeNextTransition;
        
        //  private float _timerOfProbability;
        //  private readonly float _timeToPatrol;
        //  private readonly float _probabilityInterval;
        
        private Transition _nextTransition;

        public HitState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, float timeNextTransition)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            StateId = StateId.Hit; //TODO Modificare lo StateId
            _nextTransition = Transition.FindPlayer;

            _timeNextTransition = timeNextTransition;
        }

        public override void Act()
        {
            DebugManager.Log("Ma come ti permetti!");
            
        }

        public override void Reason()
        {
            //   _timerOfProbability += Time.deltaTime;
            _timer += Time.deltaTime;

            /* if (_timerOfProbability >= _probabilityInterval) // probability calculation every x seconds
                {
                    var probability = Random.Range(0.0f, 1.0f);
                    if (probability < _probabilityScaredTransition)
                    {
                        Debug.Log("Mi sono spaventato");
                        npc.GetComponent<Monster>().SetTransition(Transition.BecomeScared);
                        return;
                    }

                    _timerOfProbability = 0.0f;
                }*/

            if (_timer >= _timeNextTransition)
            {
                _monster.SetTransition(_nextTransition);
                return;
            }
        }

        public override void DoBeforeEntering(object options)
        {
            _timer = 0.0f;

            _probabilityScaredTransition = _monster.GetScaredProbability();
            
            if (Random.Range(0.0f, 1.0f) <= _probabilityScaredTransition){
                _nextTransition = Transition.BecomeScared;
                _monster.DecreaseScaredProbability();
            }
            else{
                _nextTransition = Transition.FindPlayer;
            }

            _anim.SetTrigger(Hit);
            _agent.isStopped = true;
            
            DebugManager.Log("Scared Probability: " + _probabilityScaredTransition);
        }

        public override void DoBeforeLeaving(object options)
        {
            
        }
    }
}