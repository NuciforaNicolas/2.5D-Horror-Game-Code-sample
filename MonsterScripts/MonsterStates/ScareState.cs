using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class ScareState : FsmState
    {
        private readonly Monster _monster;
	    
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        private static readonly int Y = Animator.StringToHash("Y");
        private readonly GameObject[] _scaredWayPoints;
        private Vector3 _currentScaredWayPoint;
        private float _speed;
        
        public ScareState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, GameObject[] scaredWayPoints, float speed)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            _scaredWayPoints = scaredWayPoints;
            _speed = speed;

            StateId = StateId.Scared; //TODO Modificare lo StateId
        }
        
        public override void Act()
        {
            
        }

        public override void Reason()
        {
            float dist = Vector3.Distance(Npc.transform.position, _currentScaredWayPoint);
            if (dist <= 3f){
                _monster.SetTransition(Transition.StopWalking);
            }
        }

        public override void DoBeforeEntering(object options)
        {
            _agent.isStopped = false;
            _agent.speed = _speed; // Probabilmente da cambiare con lo scaling giusto
            _anim.SetFloat(Y, 2);
            _currentScaredWayPoint = _scaredWayPoints[Random.Range(0, _scaredWayPoints.Length)].transform.position;
            _agent.SetDestination(_currentScaredWayPoint);
        }

        public override void DoBeforeLeaving(object options)
        {
            _agent.isStopped = true;
        }
    }
}