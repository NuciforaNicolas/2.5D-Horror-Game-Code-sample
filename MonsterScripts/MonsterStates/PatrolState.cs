using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class PatrolState : FsmState
    {
        private readonly Monster _monster;
        
        private static readonly int Y = Animator.StringToHash("Y");
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        private readonly float _rayLength;
        private readonly RoomWayPoint[] _rooms;
        private readonly Ray[] _rays;
        private bool _targetReached = true;
        private float _timer;
        private readonly float _minTimeToIdle;
        private readonly float _maxTimeToIdle;
        private float _timeToIdle;
        private float _speed;


        public PatrolState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, RoomWayPoint[] rooms, float rayLength, float minTimeToIdle, float maxTimeToIdle, float speed)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            _rayLength = rayLength;
            _rooms = rooms;
            _rays = new Ray[2];
            _minTimeToIdle = minTimeToIdle;
            _maxTimeToIdle = maxTimeToIdle;
            _speed = speed;

            StateId = StateId.Patrolling;
        }

        public override void Act()
        {
            /*if (wayPoint == null){
                wayPoint = npc.GetComponent<Monster>().alertObject.transform;
                _agent.SetDestination(wayPoint.position);
            }*/
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
                _agent.SetDestination(new Vector3(x, _agent.transform.position.y, z));//_monster.test.position);//Per test della visione
            }

            _targetReached = _agent.velocity.magnitude <= 0.1f;

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
        /// </summary>
        public override void Reason()
        {
            /*if (Input.GetMouseButtonDown(0))
            {
                Ray mousePoint = Camera.main.ScreenPointToRay(Input.mousePosition);

                DebugManager.Log("Ho premuto il tasto sinistro del mouse");

                int platformLayer = 1<<8;

                if (Physics.Raycast(mousePoint, out var hitInfo, Mathf.Infinity , platformLayer))
                {
                    var options = new Dictionary<string, object>
                    {
                        {"found_tag", hitInfo.collider.tag},
                        {"found_position", hitInfo.point}
                    };

                    DebugManager.Log("Sto passando allo stato distracted");

                    monster.SetTransition(Transition.BecomeDistracted, options);
                    return;
                }

            }*/

            _timer += Time.deltaTime;
            if (_timer >= _timeToIdle)
            {
                _monster.SetTransition(Transition.StopWalking);
                return;
            }

            if (Npc.CheckPlayerOnSight())
            {
                _monster.SetTransition(Transition.GoChasing);
                return;
            }

            var torchOnSight = IA.CheckRayCollisionWithTag(_rays, "Torch");
            if (torchOnSight != null)
            {
                _monster.SetTransition(Transition.GoChasing, torchOnSight);
                return;
            }

        }

        public override void DoBeforeEntering(object options)
        {
            DebugManager.Log("Sto entrando allo stato patrol");
            _agent.isStopped = false;
            _anim.SetFloat(Y, 2);
            _agent.speed = _speed; // Probabilmente da cambiare con lo scaling giusto
            _timeToIdle = Random.Range(_minTimeToIdle, _maxTimeToIdle);
            _timer = 0;

            _targetReached = true;

            if(options is Vector3 vector3){
                _agent.SetDestination(vector3);
                _targetReached = false;
            }
            else{

                if (!(options is Dictionary<string, object> dictionary)) return; // Return if options is Unknown

                if (!dictionary.ContainsKey("found_tag")) return;

                DebugManager.Log("Ho visto " + dictionary["found_tag"]); // Ex only if tag is known

                if (!dictionary.ContainsKey("found_position")) return;

                _agent.SetDestination((Vector3) dictionary["found_position"]); // Ex only if player position is known
                _targetReached = false;
            }
        }

        public override void DoBeforeLeaving(object options)
        {
            DebugManager.Log("Sto uscendo dallo stato patrol");
            // _agent.isStopped = true;     // TODO vedere cosa mettere
        }
    }
}