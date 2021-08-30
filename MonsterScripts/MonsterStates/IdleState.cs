using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class IdleState : FsmState
    {
        private readonly Monster _monster;
        
        private static readonly int Y = Animator.StringToHash("Y");
        private readonly Animator _anim;
        private readonly NavMeshAgent _agent;
        private readonly float _rayLength;
        private readonly Ray[] _rays;
        private float _timer;
        private readonly float _minTimeToWalk;
        private readonly float _maxTimeToWalk;
        private float _timeToWalk;


        public IdleState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, float rayLength, float minTimeToWalk, float maxTimeToWalk)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            _rayLength = rayLength;
            _rays = new Ray[2];

            _minTimeToWalk = minTimeToWalk;
            _maxTimeToWalk = maxTimeToWalk;
            _timeToWalk = Random.Range(_minTimeToWalk, _maxTimeToWalk);

            StateId = StateId.Idle;
        }

        /// <summary>
        ///     Sta in idle
        /// </summary>
        public override void Act()
        {
            var forward = Npc.transform.forward;
            var leftDir = Quaternion.AngleAxis(Npc.angleGround, Vector3.up) * forward;
            var rightDir = Quaternion.AngleAxis(-Npc.angleGround, Vector3.up) * forward;

            var position = Npc.transform.position;
            DebugManager.ExecuteDebugMethod(() => Debug.DrawRay(position + new Vector3(0, 1, 0), leftDir * _rayLength, new Color(255, 0, 0)));
            DebugManager.ExecuteDebugMethod(() => Debug.DrawRay(position + new Vector3(0, 1, 0), rightDir * _rayLength, new Color(0, 255, 0)));
            
            _rays[0] = new Ray(position + new Vector3(0, 1, 0), leftDir * _rayLength); 
            _rays[1] = new Ray(position + new Vector3(0, 1, 0), rightDir * _rayLength); 
        }

        /// <summary>
        ///     Il metodo permette il passaggio dallo stato idle allo stato walk in un certo intervallo di tempo randomico
        /// </summary>
        public override void Reason()
        {
            _timer += Time.deltaTime;
            if (_timer >= _timeToWalk)
            {
                _monster.SetTransition(Transition.StartWalking);
                return;
            }

            if(Npc.CheckPlayerOnSight()){
                _monster.SetTransition(Transition.BecomeAlerted);
                return;
            }

            var torchOnSight = IA.CheckRayCollisionWithTag(_rays, "Torch");
            if (torchOnSight != null){
                _monster.SetTransition(Transition.BecomeAlerted, torchOnSight);
                return;
            }
        }

        public override void DoBeforeEntering(object options)
        {
            // DebugManager.Log("Sto entrando allo stato idle");
            _agent.isStopped = true;
            _anim.SetFloat(Y, 0);
            _agent.speed = 3.5f; // Probabilmente da cambiare con lo scaling giusto
            _timeToWalk = Random.Range(_minTimeToWalk, _maxTimeToWalk);
            _timer = 0;
        }

        public override void DoBeforeLeaving(object options)
        {
            // DebugManager.Log("Sto uscendo dallo stato idle");
        }
    }
}