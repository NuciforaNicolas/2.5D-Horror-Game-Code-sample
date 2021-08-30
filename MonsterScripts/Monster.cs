using System;
using System.Collections.Generic;
using Character_Scripts.MonsterScripts.MonsterStates;
using Managers;
using Managers.StressManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// TODO refactor ALL OF THIS SCRIPT in smaller and more manageable files!

namespace Character_Scripts
{
    public class Monster : IA
    {
        [Space()]
        [Header("Monster state attributes")]
        [SerializeField] private RoomWayPoint[] rooms;
        [SerializeField] private float rayLength;

        [Space()]
        [Header("Idle state")]
        [SerializeField] private float minTimeToWalk;
        [SerializeField] private float maxTimeToWalk;

        [Space()]
        [Header("Walk state")]
        [SerializeField] private float minTimeToIdle_Walk;
        [SerializeField] private float maxTimeToIdle_Walk;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float minTimeToSpawn;
        [SerializeField] private float maxTimeToSpawn;
        [SerializeField] private List<GameObject> spawnPoints;

        [Space()]
        [Header("Alert state")]
        [SerializeField] private float timeToPatrol_Alert;

        [Space()]
        [Header("Patrol state")]
        [SerializeField] private float minTimeToIdle_Patrol;
        [SerializeField] private float maxTimeToIdle_Patrol;
        [SerializeField] private float patrolSpeed;

        [Space()]
        [Header("Chase state")]
        [SerializeField] private float maxDistance;
        [SerializeField] private float distanceAttack;
        [SerializeField] private float minTimeToPatrol_Chase;
        [SerializeField] private float maxTimeToPatrol_Chase;
        [SerializeField] private float chaseSpeed;

        [Space()]
        [Header("Attack state")]
        [SerializeField] private BoxCollider hand;
        [SerializeField] private float minTimeToChase_Attack;
        [SerializeField] private float maxTimeToChase_Attack;

        [Space()]
        [Header("Distracted state")]
        [SerializeField] private float timeToPatrol_Distracted;

        [Space()]
        [Header("Scare state")]
        [SerializeField] private GameObject[] scaredWayPoints;
        [SerializeField] private float scaredSpeed;

        [Space()]
        [Header("Scared Probability")]
        [SerializeField] private float probabilityScaredTransition = .8f;    // Probability to transition to scared after hit.  value in [0, 1]
        [SerializeField] private float timeToPatrol_Hit = 1.5f;    // Time to transition in patrol after hit
        [SerializeField] private float decreaseScaredProbability = .1f;    // Value to remove from probabilityScaredTransition after his transition
        [SerializeField] private float increaseScaredProbability = .05f;    // Value to add to probabilityScaredTransition if transition is not triggered
        [SerializeField] private float increaseScaredProbTimer = 5;    // Seconds to increase scared probability
        [SerializeField] private float maxScaredProb = .8f;
        [SerializeField] private float minScaredProb = .3f;

        //[Space()]
        //[Header("test")]
        //[SerializeField] public Transform test;//per test della visione

        // @todo verificare se usare timer
        private float _timer;
        
        private NavMeshAgent _agent;

        protected override void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            base.Start();
        }

        protected override void MakeFSM()
        {
            var idle = new IdleState(player, gameObject, Anim, _agent, rayLength, minTimeToWalk, maxTimeToWalk);
            idle.AddTransition(Transition.StartWalking, StateId.Walking);
            idle.AddTransition(Transition.BecomeAlerted, StateId.Alert);
            idle.AddTransition(Transition.BecomeScared, StateId.Scared);
            idle.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            idle.AddTransition(Transition.BecomeDistracted, StateId.Distracted);

            var walk = new WalkState(player, gameObject, Anim, _agent, rooms, rayLength, minTimeToIdle_Walk, maxTimeToIdle_Walk, walkSpeed, spawnPoints, minTimeToSpawn, maxTimeToSpawn);
            walk.AddTransition(Transition.StopWalking, StateId.Idle);
            walk.AddTransition(Transition.BecomeAlerted, StateId.Alert);
            walk.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            walk.AddTransition(Transition.BecomeDistracted, StateId.Distracted);

            var alert = new AlertState(player, gameObject, Anim, _agent, timeToPatrol_Alert);
            alert.AddTransition(Transition.FindPlayer, StateId.Patrolling);
            alert.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            alert.AddTransition(Transition.BecomeDistracted, StateId.Distracted);

            var patrol = new PatrolState(player, gameObject, Anim, _agent, rooms, rayLength, minTimeToIdle_Patrol, maxTimeToIdle_Patrol, patrolSpeed);
            patrol.AddTransition(Transition.GoChasing, StateId.Chasing);
            patrol.AddTransition(Transition.StopWalking, StateId.Idle);
            patrol.AddTransition(Transition.BecomeDistracted, StateId.Distracted);
            patrol.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            patrol.AddTransition(Transition.FindPlayer, StateId.Patrolling);

            var chase = new ChasingState(player, gameObject, Anim, _agent, rayLength, maxDistance, distanceAttack, minTimeToPatrol_Chase, maxTimeToPatrol_Chase, chaseSpeed);
            chase.AddTransition(Transition.AttackPlayer, StateId.Attacking);
            chase.AddTransition(Transition.FindPlayer, StateId.Patrolling);
            chase.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            chase.AddTransition(Transition.BecomeDistracted, StateId.Distracted);

            var attack = new AttackState(player, gameObject, Anim, _agent, hand, minTimeToChase_Attack, maxTimeToChase_Attack);
            attack.AddTransition(Transition.GoChasing, StateId.Chasing);
            attack.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            attack.AddTransition(Transition.BecomeDistracted, StateId.Distracted);

            var distract = new DistractState(player, gameObject, Anim, _agent, timeToPatrol_Distracted);
            distract.AddTransition(Transition.FindPlayer, StateId.Patrolling);
            distract.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            distract.AddTransition(Transition.BecomeDistracted, StateId.Distracted);

            var hit = new HitState(player, gameObject, Anim, _agent, timeToPatrol_Hit);
            hit.AddTransition(Transition.FindPlayer, StateId.Patrolling);
            hit.AddTransition(Transition.GoChasing, StateId.Chasing);
            //hit.AddTransition(Transition.BecomeAlerted, StateId.Alert); // Can be deleted (?)
            hit.AddTransition(Transition.BecomeDistracted, StateId.Distracted);
            hit.AddTransition(Transition.ReceiveDamage, StateId.Hit);
            hit.AddTransition(Transition.BecomeScared, StateId.Scared);

            var scared = new ScareState(player, gameObject, Anim, _agent, scaredWayPoints, scaredSpeed);
            scared.AddTransition(Transition.StopWalking, StateId.Idle);
            scared.AddTransition(Transition.ReceiveDamage, StateId.Hit);


            Fsm = new FsmSystem();
            Fsm.AddState(idle);
            Fsm.AddState(walk);
            Fsm.AddState(alert);
            Fsm.AddState(patrol);
            Fsm.AddState(chase);
            Fsm.AddState(attack);
            Fsm.AddState(distract);
            Fsm.AddState(hit);
            Fsm.AddState(scared);
        }

        private void Update()
        {
            // Se la probabilità non ha ancora raggiunto il massimo, incrementiamo il timer, e aggiorniamo i valori
            if (probabilityScaredTransition < maxScaredProb)
            {
                _timer += Time.deltaTime;
                if (_timer >= increaseScaredProbTimer)
                {
                    _timer = 0;
                    IncreaseScaredProbability();
                }
            }
        }

        public override void Hit()
		{
            SetTransition(Transition.ReceiveDamage);
        }

		private void OnTriggerEnter(Collider other)
        {
            // Tag da aggiungere
            if (other.CompareTag("Noise"))
            {
                SetTransition(Transition.BecomeDistracted, other.transform.position);
                StressManager.Instance.SetTransition(Transition.StrMng_ObjFalling);
            }
            else if (other.CompareTag("StepNoise"))
            {
                if (Fsm.CurrentStateId.ToString() == "Idle" || Fsm.CurrentStateId.ToString() == "Walking")
                    SetTransition(Transition.BecomeAlerted, other.transform.position);
                else if (Fsm.CurrentStateId.ToString() == "Patrolling")
                    SetTransition(Transition.FindPlayer, other.transform.position);
            }
        }

        private void OnDrawGizmos()
        {
            if(_agent != null){
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(_agent.destination, new Vector3(1, 1, 1));
            }
        }

        public void DecreaseScaredProbability()
        {
            _timer = 0;
            probabilityScaredTransition -= decreaseScaredProbability;
            if (probabilityScaredTransition < minScaredProb)
                probabilityScaredTransition = minScaredProb;
        }
        public void IncreaseScaredProbability()
        {
            probabilityScaredTransition += increaseScaredProbability;
            if (probabilityScaredTransition > maxScaredProb)
                probabilityScaredTransition = maxScaredProb;
        }
        public float GetScaredProbability()
        {
            return probabilityScaredTransition;
        }

        /// <summary>
        /// Il player esce fuori dal campo visivo. Piazzo una shadow copy nella sua ultima posizione
        /// </summary>
        public void SetLastPlayerPosition()
        {
            // Abilito la shadow copy, per vedere qual'è l'ultima posizione vista dal mostro
            LastSeenManager.Instance.SetCopy(player.transform.position, player.transform.rotation);
        }

        /// <summary>
        /// Rimuovo la shadow copy
        /// </summary>
        public void UnsetLastPlayerPosition()
        {
            // Nascondo la shadow copy
            LastSeenManager.Instance.UnsetCopy();
        }
    }
}