using Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class ChasingState : FsmState
    {
        private readonly Monster _monster;
        
        private readonly Animator _anim;
        private readonly NavMeshAgent _agent;
        private readonly float _rayLength;
        private readonly Ray[] _rays;
        private readonly float _maxDistanceSquared;
        private bool _playerLost;
        private float _timer;
        private readonly float _minTimeToPatrol;
        private readonly float _maxTimeToPatrol;
        private float _timeToPatrol;
        private readonly float _distanceAttack;
        private static readonly int Y = Animator.StringToHash("Y");
        private float _speed;
        //private bool _yetAttack;
        //private float _timeToAttack;
        //private float _timerAttack;

        public ChasingState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, float rayLength, float maxDistance, float distanceAttack, float minTimeToPatrol, float maxTimeToPatrol, float speed)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();

            _playerLost = false;
            _anim = anim;
            _agent = agent;
            _rayLength = rayLength;
            _rays = new Ray[2];
            _maxDistanceSquared = maxDistance * maxDistance;
            _distanceAttack = distanceAttack;
            _minTimeToPatrol = minTimeToPatrol;
            _maxTimeToPatrol = maxTimeToPatrol;
            _speed = speed;

            StateId = StateId.Chasing;
        }


        public override void Act()
        {   
            if(!_playerLost){
                _agent.SetDestination(Player.transform.position);
            }

            var forward = Npc.transform.forward;
            var leftDir = Quaternion.AngleAxis(Npc.angleGround, Vector3.up) * forward;
            var rightDir = Quaternion.AngleAxis(-Npc.angleGround, Vector3.up) * forward;

            var position = Npc.transform.position;
            DebugManager.ExecuteDebugMethod(() => Debug.DrawRay(position + new Vector3(0, 1, 0), leftDir * _rayLength, new Color(255, 0, 0)));
            DebugManager.ExecuteDebugMethod(() => Debug.DrawRay(position + new Vector3(0, 1, 0), rightDir * _rayLength, new Color(0, 255, 0)));
            
            _rays[0] = new Ray(position + new Vector3(0, 1, 0), leftDir * _rayLength);
            _rays[1] = new Ray(position + new Vector3(0, 1, 0), rightDir * _rayLength);
        }

        public override void Reason()
        {
            var torch = IA.CheckRayCollisionWithTag(_rays, "Torch");

            if ((Vector3.SqrMagnitude(Player.transform.position - Npc.transform.position) >= _maxDistanceSquared) && (torch == null))
            {
                // Se ho già perso di vista il player, non devo riperderlo
                if (!_playerLost)
                {
                    _playerLost = true;
                    _monster.SetLastPlayerPosition();
                    _anim.SetFloat(Y, 2);
                    _agent.speed = _speed; // Probabilmente da cambiare con lo scaling giusto
                    DebugManager.Log("Ho perso di vista il player");
                }
            }
            else
            {
                // Se non l'ho perso di vista, non devo rivederlo
                if (_playerLost && (Npc.CheckPlayerOnSight() || (torch != null)))
                {
                    _playerLost = false;
                    _timer = 0;
                    _monster.UnsetLastPlayerPosition();
                    _anim.SetFloat(Y, 3);
                    _agent.speed = _speed; // Probabilmente da cambiare con lo scaling giusto
                    DebugManager.Log("L'ho rivisto");
                }  
            }

            if (Vector3.Distance(Player.transform.position, Npc.transform.position) <= _distanceAttack)
            {
                DebugManager.Log("Sto attaccando");
                _monster.SetTransition(Transition.AttackPlayer);
                return;
            }
            
            if (!_playerLost) return;

            // Se il mostro raggiunge la sua destinazione deve smettere di camminare 
            if(_agent.remainingDistance <= 2){
                _anim.SetFloat(Y, 0);
                _monster.UnsetLastPlayerPosition();
            }
                
            _timer += Time.deltaTime;

            if (_timer >= _timeToPatrol)
            {
                _monster.UnsetLastPlayerPosition();
                _monster.SetTransition(Transition.FindPlayer);
                return;
            }
        }

        public override void DoBeforeEntering(object options)
        {
            _timeToPatrol = Random.Range(_minTimeToPatrol, _maxTimeToPatrol);
            _timer = 0;
            _playerLost = false;
            _agent.isStopped = false;
            _anim.SetFloat(Y, 3);
            _agent.speed = _speed; // Probabilmente da cambiare con lo scaling giusto
            DebugManager.Log("Time to patrol: " + _timeToPatrol);
            //TODO Aggiungere comportamento quando si entra nello stato
        }

        public override void DoBeforeLeaving(object options)
        {
            DebugManager.Log("Torno allo stato patrol");
            //TODO Aggiungere comportamento prima di uscire dallo stato
        }
    }
}