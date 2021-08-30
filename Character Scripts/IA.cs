using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Character_Scripts
{
    public class IA : Character
    {
        protected GameObject player;
        private Transform playerHead;
        public float angleGround;
        [SerializeField] float headOffset, checkPlayerOnSightDistance;
        [SerializeField] Transform npcHead;
        
        //[SerializeField] protected Transform[] path;
        protected FsmSystem Fsm;

        private bool hasFSM;

        protected override void Start()
        {
            base.Start();

            SetUpPlayer();

            hasFSM = true;
            MakeFSM();
        }

        /// <summary>
        /// Trova il riferimento ai componenti del player
        /// </summary>
        private void SetUpPlayer()
        {
            player = GameObject.FindWithTag("Player");

            if (player is null)
            {
                Debug.LogError(gameObject.name + ": Player not found");
                return;
            }

            playerHead = SearchTransformWithName(player.transform, "mixamorig:Head");

            if (playerHead is null)
            {
                Debug.LogError(gameObject.name + ": Player HEAD not found");
                return;
            }
        }

        /// <summary>
        /// Cerca ricorsivamente all'interno del transform e dei figli per trovare l'oggetto col nome dato
        /// </summary>
        /// <param name="t">Il transform di partenza</param>
        /// <param name="name">nome dell'oggeto da cercare</param>
        private Transform SearchTransformWithName(Transform t, string name)
        {
            if (t.name == name)
                return t;
            
            for (int i = 0; i < t.childCount; i++)
            {
                Transform tempT = SearchTransformWithName(t.GetChild(i), name);
                if (tempT)
                {
                    return tempT;
                }
            }

            return null;
        }

        protected void FixedUpdate()
        {
            if (hasFSM)
            {
                Fsm.CurrentState.Reason();
                Fsm.CurrentState.Act();
            }
        }

        public void SetTransition(Transition t, object options = null)
        {
            Fsm.PerformTransition(t, options);
            DebugManager.Instance.SetState(Fsm.CurrentStateId.ToString());
        }

        /// <summary>
        /// Metodo ereditato, in cui i figli instanziano una FSM. 
        /// </summary>
        protected virtual void MakeFSM()
        {
            // Se i figli non fanno l'override del metodo, vuol dire che non utilizzano FSM
            hasFSM = false;
        }
        
        public static object CheckRayCollisionWithTag(IEnumerable<Ray> rays, string tag)
        {
            foreach (var ray in rays)
                if (Physics.Raycast(ray, out var rayHit) && rayHit.collider.gameObject.CompareTag(tag))
                {
                    var options = new Dictionary<string, object>
                    {
                        {"found_tag", rayHit.collider.tag},
                        {"found_position", rayHit.point}
                    };
                    return options;
                }

            return null;
        }

        public static implicit operator IA(GameObject v)
        {
            return v.GetComponent<IA>();
        }
        
        private static bool CheckEntityCollision(Vector3 playerPosition, float cosAngleGround, float cosAngleUp,
            Vector3 currentPosition, Vector3 forwardVector, float maximumDistanceSquared)
        {
            // Debug.Assert(maximumDistanceSquared > 0);
            // Debug.Assert(cosAngleUp * cosAngleUp > 1 - angleGround * angleGround);

            var distanceOfTarget = playerPosition - currentPosition;
            if (distanceOfTarget.sqrMagnitude > maximumDistanceSquared) return false;
            var normalizedTargetDirection = distanceOfTarget.normalized;

            // THIS MIGHT BE OPTIONAL
            //if (playerPosition.y < currentPosition.y ||
            //    Vector3.Dot(normalizedTargetDirection, Vector3.up) > cosAngleUp) return false;
            //Debug.Log("after other check");

            return Vector3.Dot(normalizedTargetDirection, forwardVector) >
                   cosAngleGround; // angle < desiredAngle implies that cosangle > cosDesiredAngle
        }
        
        public bool CheckPlayerOnSight(float cosAngleUp = 0.82f)
        {
            float cosAngleGround = Mathf.Cos(angleGround * Mathf.Deg2Rad);
            //Il mostro è troppo alto quindi ho preso una transfom più bassa

            Vector3 npcHeadTrans = npcHead.position - Vector3.up * headOffset - transform.forward * headOffset;

            var isOnSight = CheckEntityCollision(playerHead.position, cosAngleGround, cosAngleUp, npcHeadTrans,
                transform.forward, checkPlayerOnSightDistance);

            if (!isOnSight) return false;
            
            Debug.DrawRay(npcHeadTrans, (playerHead.position - npcHeadTrans), Color.green, 1.0f);
            
            //if (Physics.Raycast(npcHeadTrans.position, (playerHeadTrans.position - npcHeadTrans.position).normalized,
            //        out var hitInfo, (playerHeadTrans.position - npcHeadTrans.position).magnitude))
            //    Debug.Log(hitInfo.collider.gameObject.name);

            if (Physics.Raycast(npcHeadTrans, (playerHead.position - npcHeadTrans).normalized,
                    out var hitInfo, (playerHead.position - npcHeadTrans).magnitude, 1 << 8 | 1 << 17) &&
                hitInfo.collider.CompareTag("PlayerBody")){
                //Debug.Break();
                return true;
            }
            //Debug.Log(hitInfo.collider.gameObject.name);

            return false;
        }
    }

}