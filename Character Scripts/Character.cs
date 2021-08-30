using System.Collections;
using Interfaces;
using Managers;
using Managers.StressManager;
using UnityEngine;

namespace Character_Scripts
{
    public class Character : MonoBehaviour, IHittable
    {
        protected readonly int animX = Animator.StringToHash("X");
        protected readonly int animZ = Animator.StringToHash("Z");
        //protected static readonly int WalkPressed = Animator.StringToHash("WalkPressed");
        protected static readonly int CrouchPressed = Animator.StringToHash("CrouchPressed");
        private static readonly int Jump1 = Animator.StringToHash("Jump");

        [Header("Character Attributes")]
        [SerializeField] protected float force = 100f;
        [SerializeField] protected int health = 2;
        [SerializeField] protected bool isJumping;
        [SerializeField] protected int maxHealth = 2;
        protected float speed;
        //[SerializeField] protected float speedWalk = 1f;
        [SerializeField] protected float speedRunning = 2f;
        [SerializeField] protected float speedCrouch = 0.5f;
        [SerializeField] protected float speedDragging = 0.5f;
        [SerializeField] protected float speedCrawling = 0.2f;
        
        [SerializeField] private float timeToJump;

        protected Animator Anim;

        protected Rigidbody Rb;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Rb = GetComponent<Rigidbody>();
            health = maxHealth;
            Anim = GetComponent<Animator>();
            speed = speedRunning;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("platform"))
                isJumping = false;
        }

        public virtual void Hit()
        {
            health--;
            if (health <= 0)
                Die();
        }

        public void Climb()
        {
            if (isJumping)
                return;

            StressManager.Instance.SetTransition(Transition.StrMng_Jumping);

            isJumping = true;
            //Rb.AddForce(Vector3.up * force);

            if (!Anim) return;
            StartCoroutine(nameof(WaitToJump));
            Anim.SetTrigger(Jump1);
        }

        private IEnumerator WaitToJump()
        {
            yield return new WaitForSeconds(timeToJump);
            Rb.AddForce(Vector3.up * force);
        }

        protected virtual void Die()
        {
            //TODO Verrà attivata l'animazione della morte
        }
    }
}