using Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class WalkState : FsmState
    {
        private readonly Monster _monster;
        
        private static readonly int Y = Animator.StringToHash("Y");
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        private readonly float _rayLength;
        private readonly RoomWayPoint[] _rooms;
        private readonly Ray[] _rays;
        private bool _targetReached = true;
        private float _timerIdle;
        private readonly float _minTimeToIdle;
        private readonly float _maxTimeToIdle;
        private float _timeToIdle;
        private float _speed;
        private List<GameObject> _spawnPoints; // ogni tot secondi, sceglie uno spawnpoint dove far teletrasportare il mostro in modo da far cambiare zona
        private float _minTimeToSpawn;
        private float _maxTimeToSpawn;
        private float _timeToSpawn;
        private float _timerSpawn;

        public WalkState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, RoomWayPoint[] rooms, float rayLength, float minTimeToIdle, float maxTimeToIdle, float speed, List<GameObject> spawnpoints, float minTimeToSpawn, float maxTimeToSpawn)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            _rooms = rooms;
            _rayLength = rayLength;
            _rays = new Ray[2];
            _minTimeToIdle = minTimeToIdle;
            _maxTimeToIdle = maxTimeToIdle;
            _speed = speed;
            _spawnPoints = spawnpoints;
            _minTimeToSpawn = minTimeToSpawn;
            _maxTimeToSpawn = maxTimeToSpawn;

            StateId = StateId.Walking;
        }

        /// <summary>
        ///     Cammina
        /// </summary>
        public override void Act()
        {
            /*  Vector3 dest = Random.insideUnitSphere * _distance;
                dest = npc.transform.position + (dest - (dest.y * Vector3.up));
                Debug.Log(dest);
                _agent.SetDestination(dest);
                _timer = 0;
            */
            if (_targetReached)
            {
                var randX = Random.value;
                var randZ = Random.value;
                // Debug.Log("RandX: " + randX);
                //Debug.Log("RandZ: " + randZ);
                // float x = _wayPoints[0].transform.position.x * (1 - rand) + (_wayPoints[1].transform.position.x * rand);
                int room = Random.Range(0, _rooms.Length);
                var x = Mathf.Lerp(_rooms[room].waypoints[0].transform.position.x, _rooms[room].waypoints[1].transform.position.x, randX);
                var z = Mathf.Lerp(_rooms[room].waypoints[0].transform.position.z, _rooms[room].waypoints[2].transform.position.z, randZ);
                _agent.SetDestination(new Vector3(x, _agent.transform.position.y, z));
            }

            _targetReached = _agent.velocity.magnitude <= 0.1f;

            // raycast
            var forward = Npc.transform.forward;
            var leftDir = Quaternion.AngleAxis(Npc.angleGround, Vector3.up) * forward;
            var rightDir = Quaternion.AngleAxis(-Npc.angleGround, Vector3.up) * forward;

            var position = Npc.transform.position;
            DebugManager.ExecuteDebugMethod(() => Debug.DrawRay(position + new Vector3(0, 1, 0), leftDir * _rayLength, new Color(255, 0, 0)));
            DebugManager.ExecuteDebugMethod(() => Debug.DrawRay(position + new Vector3(0, 1, 0), rightDir * _rayLength, new Color(0, 255, 0)));
            
            _rays[0] = new Ray(position + new Vector3(0, 1, 0), leftDir * _rayLength);
            _rays[1] = new Ray(position + new Vector3(0, 1, 0), rightDir * _rayLength);

            // passati tot secondi, il mostro verrà teletrasportato in uno spawnpoint randomico in modo da far cambiare zona
            _timerSpawn += Time.deltaTime;
            if(_timerSpawn >= _timeToSpawn)
            {
                if (_spawnPoints.Count <= 0)
                {
                    Debug.Log("WALKSTATE: spawnpoint list empty");
                    _timerSpawn = 0;
                    return;
                }
                _agent.Warp(_spawnPoints[Random.Range(0, _spawnPoints.Count - 1)].transform.position);
                _timerSpawn = 0;
            }
        }

        /// <summary>
        ///     Il metodo permette il passaggio dallo stato walking allo stato idle
        /// </summary>
        public override void Reason()
        {
            _timerIdle += Time.deltaTime;
            if (_timerIdle >= _timeToIdle)
            {
                _monster.SetTransition(Transition.StopWalking);
                return;
            }

            if (Npc.CheckPlayerOnSight())
            {
                Debug.Log("Visto player");
                _monster.SetTransition(Transition.BecomeAlerted);
                return;
            }

            var torchOnSight = IA.CheckRayCollisionWithTag(_rays, "Torch");
            if (torchOnSight != null)
            {
                _monster.SetTransition(Transition.BecomeAlerted, torchOnSight);
                return;
            }
        }

        public override void DoBeforeEntering(object options)
        {
            // DebugManager.Log("Sto entrando allo stato walk");
            _agent.isStopped = false;
            _anim.SetFloat(Y, 1);
            _agent.speed = _speed; // Probabilmente da cambiare con lo scaling giusto
            _timeToIdle = Random.Range(_minTimeToIdle, _maxTimeToIdle);
            _timeToSpawn = Random.Range(_minTimeToSpawn, _maxTimeToSpawn);
            _timerIdle = 0;
            _timerSpawn = 0;
        }

        public override void DoBeforeLeaving(object options)
        {
            _agent.isStopped = true;
            // DebugManager.Log("Sto uscendo dallo stato walk");
        }
    }
}