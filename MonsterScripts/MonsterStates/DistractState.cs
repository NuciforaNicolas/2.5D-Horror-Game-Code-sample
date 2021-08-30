using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class DistractState : FsmState
    {
        private readonly Monster _monster;
	    
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        private static readonly int Y = Animator.StringToHash("Y");
        private static readonly int Grattata = Animator.StringToHash("Grattata"); // DA SOSTITUIRE CON L'ANIMAZIONE VERA E PROPRIA
        private readonly float _timeToPatrol;
        private float _timer;
        private bool _yetAnimated;

        public DistractState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, float timeToPatrol)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            
            _anim = anim;
            _agent = agent;
            _timeToPatrol = timeToPatrol;
            StateId = StateId.Distracted; //TODO Modificare lo StateId
        }
        
        public override void Act()
        {
           
        }

        public override void Reason()
        {
            if(Vector3.Distance(Npc.transform.position, _agent.destination) <= 0.1){
                if (!_yetAnimated)
                {
                    _anim.SetTrigger(Grattata);
                    _yetAnimated = true;
                }
                _timer += Time.deltaTime;
                if (_timer >= _timeToPatrol){
                    _monster.SetTransition(Transition.FindPlayer);
                    return;
                }
            }
        }

        public override void DoBeforeEntering(object options)
        {
            _timer = 0;
            _agent.isStopped = false;

            if (options is Vector3 position)
            {
                _agent.SetDestination(position);
                _anim.SetFloat(Y, 1);
                _agent.speed = 3.5f; // Probabilmente da cambiare con lo scaling giusto
                return;
            }

            if (!(options is Dictionary<string, object> dictionary)) return; // Return if options is Unknown

            if (!dictionary.ContainsKey("found_tag")) return;

            if (!dictionary.ContainsKey("found_position")) return;

            _agent.SetDestination((Vector3)dictionary["found_position"]);


        }

        public override void DoBeforeLeaving(object options)
        {
            _yetAnimated = false;
            //TODO Aggiungere comportamento prima di uscire dallo stato
        }
    }
}