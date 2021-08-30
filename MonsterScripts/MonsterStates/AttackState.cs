using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class AttackState : FsmState
    {
        private readonly Monster _monster;
	    
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int AttackParam = Animator.StringToHash("Attack");
        private readonly float _minTimeToChase;
        private readonly float _maxTimeToChase;
        private float _timeToChasing;
        private float _timer;
        private readonly BoxCollider _hand;

        //private readonly float _timeToAttack = 0.6f; NON UTILIZZATI. DA RIMUOVERE?
        //private float _timerOfAttack;
        //private bool isAttacking;

        public AttackState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, BoxCollider hand, float minTimeToChase, float maxTimeToChase)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            _minTimeToChase = minTimeToChase;
            _maxTimeToChase = maxTimeToChase;
            _hand = hand;
            StateId = StateId.Attacking;
        }
        
        public override void Act()
        {
            /*_timerOfAttack += Time.deltaTime;
            if (!isAttacking && _timerOfAttack >= _timeToAttack)
            {
                player.GetComponent<Player>().Hit();
                isAttacking = true;
            }*/
        }

        public override void Reason()
        {
            _timer += Time.deltaTime;
            if(_timer >= _timeToChasing){
                _monster.SetTransition(Transition.GoChasing);
                return;
            }
        }

        public override void DoBeforeEntering(object options)
        {
            Debug.Log("Inside attack state");
            _timeToChasing = Random.Range(_minTimeToChase, _maxTimeToChase);
            _timer = 0;
           // _timerOfAttack = 0f; // NON UTILIZZATI. DA RIMUOVERE?
           // isAttacking = false;
            _agent.isStopped = true;
            _hand.enabled = true;
            _anim.SetTrigger(AttackParam);
            _anim.SetFloat(Y, 0);
            //TODO Aggiungere comportamento quando si entra nello stato
        }

        public override void DoBeforeLeaving(object options)
        {
            _hand.enabled = false;
        }
    }
}