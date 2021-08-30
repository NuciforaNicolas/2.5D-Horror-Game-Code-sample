using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Character_Scripts.MonsterScripts.MonsterStates
{
    public class AlertState : FsmState
    {
        private readonly Monster _monster;
        
        private static readonly int Y = Animator.StringToHash("Y");
        private readonly NavMeshAgent _agent;
        private readonly Animator _anim;
        private float _timer;
        private float _timeToPatrol;
        private Vector3 _transitionOptions;

        public AlertState(GameObject player, GameObject npc, Animator anim, NavMeshAgent agent, float timeToPatrol)
        {
            Player = player;
            Npc = npc;
            _monster = npc.GetComponent<Monster>();
            _anim = anim;
            _agent = agent;
            //_transitionOptions = null;
            _timeToPatrol = timeToPatrol;

            StateId = StateId.Alert;
        }

        public override void Act()
        {
            // Attiva animazioni o fa altro
            // DebugManager.Log("Sono allarmato");
        }

        /// <summary>
        /// </summary>
        public override void Reason()
        {
            _timer += Time.deltaTime;
            if (_timer >= _timeToPatrol)
                _monster.SetTransition(Transition.FindPlayer, _transitionOptions);
        }

        public override void DoBeforeEntering(object options)
        {
            DebugManager.Log("Sto entrando allo stato alert");
            _agent.isStopped = true;
            _anim.SetFloat(Y, 0);
            if (options != null)
            {
                if (options is Vector3 pos) _transitionOptions = pos;
            }

            //_timeToPatrol = 2;
            _timer = 0;

            if(options is Vector3 vector3){
                _anim.gameObject.transform.LookAt(vector3);
            }
            else{
                // TODO Controllare questa sezione di codice. Options probabilmente non sarà mai un dictionary
                if (options is Dictionary<string, object> dictionary)
                    if (dictionary.ContainsKey("found_tag"))
                    {
                        _anim.gameObject.transform.LookAt((Vector3) dictionary["found_position"]);
                       // _timeToPatrol = 1; 
                    }
            }

            // TODO vedere cosa mettere
        }

        public override void DoBeforeLeaving(object options)
        {
            DebugManager.Log("Sto uscendo dallo stato alert");
            // _agent.isStopped = true;     // TODO vedere cosa mettere
        }
    }
}