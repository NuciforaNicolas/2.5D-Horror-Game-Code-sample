using System;
using System.Collections;
using System.Collections.Generic;
using Character_Scripts.Interactions;
using Character_Scripts.Skills;
using ExtensionMethods;
using Inventory;
using Managers;
using ProjectileMotion;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Door.DoorTypes;

namespace Character_Scripts
{
    public class Player : Character
    {
        [SerializeField] private float regenDelay;
        [SerializeField] private float regenTime;
        [SerializeField] private GameObject noisePrefab;    // TODO: utilizzarlo per simulare i passi
        [SerializeField] private float timeToStep;
        [SerializeField] private float timeToClimb;
        [SerializeField] private float angleCoverCheck;
        [SerializeField] private float radiusCoverLength;

        //[SerializeField] private float timeToClimb;
        private bool _canMove;  // Turn to false to prevent player movement
        private bool _waitForAnimStateToMove;    // If true we have to wait for a given animation before moving again
        private string _waitingAnimStateName;    // Name of the animation to be waited
        
        public bool isRegen;
        public bool isCrouch;
        public bool isNoise;
        public bool isCover;
        public bool tempCover;
        [SerializeField] private Transform coverObject, climbablePlatform;
        private Camera _camera;
        //private float climbTimer;
        private List<Interactable> _interactables;
        private Skill _skill;
        private BoxCollider _bxc;

        [SerializeField] private Inventory.Inventory inventory;
        public Inventory.Inventory Inventory => inventory;
        
        private ProjectileShot _projectileShot;

        private bool isAiming;
        private bool _isDragging;
        private DraggableObject _draggable;

        private bool _isClimbing;
        
        private bool _isCrawling;
        private Vector3 _forceMovementDirection;

        private CapsuleCollider _baseCollider;   // Base Collider, used normally
        private Collider _crawlCollider; // Collider used when crawling
        
        // Rig controller
        private Rig _handsRig;
        private Transform _leftHandRigTarget;
        private Transform _rightHandRigTarget;

        public Rig HandsRig => _handsRig;
        public Transform LeftHandRigTarget => _leftHandRigTarget;
        public Transform RightHandRigTarget => _rightHandRigTarget;
        
        // Animator hashes
        private readonly int animDragging = Animator.StringToHash("Dragging");
        private readonly int animCrawling = Animator.StringToHash("CrawlDown");

        protected void Awake()
        {
            _interactables = new List<Interactable>();

            _baseCollider = GetComponent<CapsuleCollider>();
            
            // Set references to components and transforms
            _handsRig = GetComponentInChildren<Rig>();
            _leftHandRigTarget = transform.FindRecursiveChildrenByTag("LeftHandRigTarget");
            _rightHandRigTarget = transform.FindRecursiveChildrenByTag("RightHandRigTarget");

            _crawlCollider = transform.FindChildrenByTag("CrawlCollider")?.GetComponent<Collider>();
            
            _projectileShot = GetComponentInChildren<ProjectileShot>();
            
            _skill = GetComponent<Skill>();
            _bxc = transform.GetChild(0).GetComponent<BoxCollider>();
            
            // Verify integrity
            VerifyReferences();
        }

        protected override void Start()
        {
            _camera = Camera.main;
            
            base.Start();

            LoadGameCompleted();

            _canMove = true;
            _waitForAnimStateToMove = false;

            isAiming = false;
            _isDragging = false;
            _isCrawling = false;
            _isClimbing = false;

            tempCover = false;
            
            inventory.StartReset();
        }

        /// <summary>
        /// Check variables, alert if false 
        /// </summary>
        private void VerifyReferences()
        {
            if (_bxc is null)
            {
                Debug.LogError("Player: Bxc not set!");
            }

            if (_handsRig is null)
            {
                Debug.LogError("Player: Hands rig not set!");
            }

            if (_leftHandRigTarget is null)
            {
                Debug.LogError("Player: Left hand rig target not set!");
            }
            
            if(_rightHandRigTarget is null)
            {
                Debug.LogError("Player: Right hand rig target not set!");
            }

            if (_baseCollider is null)
            {
                Debug.LogError("Player: Base Collider not set!");
            }

            if (_crawlCollider is null)
            {
                Debug.LogError("Player: Crawl Collider not set!");
            }
        }

        private void Update()
        {
            // Check aim input
            if (InputManager.Instance.AimPressed && inventory.HasThrowableObjects)
            {
                if (!isAiming)
                {
                    isAiming = true;
                    _projectileShot.Aim(true);
                }
            }
            else
            {
                if (isAiming)
                {
                    isAiming = false;
                    _projectileShot.Aim(false);
                }
            }

            if (isCover)
            {
                transform.LookAt((coverObject.position - transform.position) * 3);
            }

            if (_waitForAnimStateToMove)
            {
                WaitForAnimBeforeMove();
            }
        }

        private void FixedUpdate()
        {
            // Move logic
            if (_canMove)
            {
                Move();
            }
        }

        private void Move()
        {
            var horizontal = _isDragging ? 0f : InputManager.Instance.HorizontalDirection;;
            var vertical = InputManager.Instance.VerticalDirection;

            Anim.SetFloat(animX, horizontal);
            Anim.SetFloat(animZ, vertical);

            Vector3 dir;
            
            if (_isCrawling)
            {
                /*
                 * TODO: Al momento !qualsiasi! movimento in input farà muovere il player lungo lo stesso vettore
                 * Fare in modo che solo il movimento nella stessa direzione del movimento forzato sia considerato
                 */
                
                dir = _forceMovementDirection * (new Vector2(horizontal, vertical).magnitude);
            }
            else
            {
                Vector3 baseForward = Vector3.forward;

                // Se il player sta interagendo con un oggetto "draggable":
                if (_isDragging)
                {
                    // - Modifica il forward in world space, affinchè il movimento sia avanti / indietro
                    baseForward = transform.forward;
                }

                //this is the direction in the world space we want to move:
                dir = baseForward * vertical + Vector3.right * horizontal;
            }

            // If dragging objects, don't turn
            if (!_isDragging && !_isCrawling)
            {
                Vector3 direction = Vector3.right * (Time.fixedDeltaTime * (dir.x * speed)) +
                                Vector3.forward * (Time.fixedDeltaTime * (dir.z * speed));
                transform.LookAt(transform.position + direction, Vector3.up);
            }
            
            Rb.MovePosition(transform.position + dir * (Time.fixedDeltaTime * speed) );
            
            // Debug.Log(_isDragging + " " + transform.position);
            
            if(!isCrouch && !(dir.x == 0 && dir.z == 0 ) && !isNoise) StartCoroutine(nameof(MakeStepNoise));
        }

        #region UNITY Callbacks

        private void OnEnable()
        {
            InputManager.Instance.Climbing += Climb;
            InputManager.Instance.Interacting += Interact;
            InputManager.Instance.UsingSkill += UseSkill;
            InputManager.Instance.ThrowingObject += ThrowObject;
            InputManager.Instance.Attacking += Attack;
            InputManager.Instance.Crouching += Crouch;
            SaveManager.Instance.LoadCompleted += LoadGameCompleted;
            InputManager.Instance.Saving += Saving;
            InputManager.Instance.TakeCover += TakeCover;
        }
        
        private void OnDisable()
        {
            if (InputManager.Instance == null) return;
            InputManager.Instance.Climbing -= Climb;
            InputManager.Instance.Interacting -= Interact;
            InputManager.Instance.UsingSkill -= UseSkill;
            InputManager.Instance.ThrowingObject -= ThrowObject;
            InputManager.Instance.Attacking -= Attack;
            InputManager.Instance.Crouching -= Crouch;
            try
            {
                SaveManager.Instance.LoadCompleted -= LoadGameCompleted;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }

            InputManager.Instance.Saving -= Saving;
            InputManager.Instance.TakeCover -= TakeCover;
        }
        
        private void OnDrawGizmosSelected()
        {
            var position = transform.position;
            Gizmos.DrawWireSphere(position, radiusCoverLength);
            var forward = transform.forward;
            Gizmos.DrawRay(position, Quaternion.AngleAxis(angleCoverCheck * 0.5f, Vector3.up) * forward * 10f);
            Gizmos.DrawRay(position, Quaternion.AngleAxis(-angleCoverCheck * 0.5f, Vector3.up) * forward * 10f);
        }

        #endregion

        private void Saving()
        {
            SaveManager.Instance.Save.JsonInventory = inventory.SaveJsonData();
        }

        private void LoadGameCompleted()
        {
            if (inventory is null)
            {
                Debug.LogWarning("Player LoadGameCompleted(): inventory not set!");
                return;
            }
            
            inventory.LoadJsonData(SaveManager.Instance.Save.JsonInventory);
        }
        
        private void OnApplicationQuit()
        {
            if (inventory is null)
            {
                Debug.LogWarning("Player OnApplicationQuit(): inventory not set!");
                return;
            }
            
            inventory.Clear();
        }

        /// <summary>
        /// Lancia un oggetto dall'inventario
        /// </summary>
        private void ThrowObject()
        {
            if (!isAiming) return;
            
            // Pick the first throwable item, and pass it to the projectileShot
            Throwable item = inventory.FirstOfType(ItemType.Throwable) as Throwable;
            inventory.RemoveItem(item, 1);
            _projectileShot.Throw(item);
        }

        private void Attack()
        {
            var mousePoint = _camera.ScreenPointToRay(Input.mousePosition);
            var monsterLayer = 1 << 14;

            if (Physics.Raycast(mousePoint, out var hitInfo, Mathf.Infinity, monsterLayer))
                hitInfo.collider.GetComponent<Monster>().Hit();
        }
        
        private IEnumerator MakeStepNoise(){
            Instantiate(noisePrefab, transform.position, Quaternion.identity);
            isNoise=true;
            yield return new WaitForSeconds(timeToStep);
            isNoise=false;
        }
        
        public void UseSkill()
        {
            if (_skill is null) return;
            
            _skill.UseSkill();
        }

        public override void Hit()
        {
            base.Hit();

            UIManager.Instance.EnableHit();

            if (health > 0)
            {
                if (!isRegen)
                    isRegen = true;
                else
                    StopCoroutine(nameof(Regen));

                StartCoroutine(nameof(Regen));
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }

        private IEnumerator Regen()
        {
            yield return new WaitForSeconds(regenDelay);
            while (health < maxHealth)
            {
                if (health > 0)
                    health++;
                else
                    break;
                yield return new WaitForSeconds(regenTime);
            }

            isRegen = false;
        }

        /// <summary>
        /// Stop player movemnt until an animator state is reached
        /// </summary>
        /// <param name="stateName"></param>
        private void StopMovementTillAnim(string stateName)
        {
            _canMove = false;
            _waitingAnimStateName = stateName;
            _waitForAnimStateToMove = true;
        }
        
        /// <summary>
        /// Check whenever we reached the right animation before moving again
        /// </summary>
        private void WaitForAnimBeforeMove()
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName(_waitingAnimStateName))
            {
                _canMove = true;
                
                _waitForAnimStateToMove = false;
            }
        }

        #region Crouch

        private void Crouch(){
            // Crouch to stand
            if(isCrouch){
                isCrouch = false;
                Anim.SetBool(CrouchPressed, false);
                LeaveCover();

                speed = speedRunning;

                StopMovementTillAnim("Blend Tree");
                // Durante l'animazione non ti puoi muovere 
            }
            // Stand to crouch
            else{
                isCrouch = true;
                Anim.SetBool(CrouchPressed, true);
            
                if (tempCover)
                {
                    tempCover = false;
                    GetCover();
                }
                
                speed = speedCrouch;
                
                StopMovementTillAnim("Crouch Blend Tree");
            }
        }

        private void GetCover(){
            isCover = true;
            Anim.SetBool("IsCover", true);
            _bxc.enabled = false;
        }

        private void LeaveCover(){
            isCover = false;
            Anim.SetBool("IsCover", false);
            _bxc.enabled = true;
        }

        #endregion

        #region Collisions

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("HideSpot") && !isCover && isCrouch){
                GetCover();
                coverObject = other.transform;
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("HideSpot") && !isCrouch)
            {
                tempCover = true;
                coverObject = other.transform;
            }
            if(other.CompareTag("Door")){
                // Ogni chiave ha uno scopo diverso e bisogna differenziarli. Usare lo switch case per capire quale chiave dobbiamo usare
                Debug.Log("Ho intercettato la porta");
                Door door = other.GetComponent<Door>();
                Item key = null;
                switch(door.DoorType) {
                    case Normal : key = inventory.FirstOfType(ItemType.NormalKey) as NormalKey; break;
                    case Secret : key = inventory.FirstOfType(ItemType.SecretKey) as SecretKey; break;
                    //case Library: key = inventory.FirstOfType(ItemType.LibraryKey) as LibraryKey; break;
                }
                if(key is null) return;
                if (door.canOpenDoor())
                {
                    door.openDoor();
                    inventory.RemoveItem(key, 1);
                }
            }

            // if (other.CompareTag("ClimbablePlatform") && other.GetComponent<ClimbableObject>().isClimbable())
            // {
            //     InputManager.Instance.canClimb = true;
            //     climbablePlatform = other.transform;
            // }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("HideSpot") && isCover)
            {
                LeaveCover();
                coverObject = null;
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("HideSpot") && !isCrouch)
            {
                tempCover = false;
            }
            // if (other.CompareTag("ClimbablePlatform") && !other.GetComponent<ClimbableObject>().isClimbable())
            // {
            //     InputManager.Instance.canClimb = false;
            //     climbablePlatform = null;
            // }
        }

        #endregion

        #region Interactions

        /// <summary>
        /// Un oggettto interagibile può essere raccolto, lo teniamo in memoria, aspettando l'input dell'utente
        /// </summary>
        /// <param name="interactable">L'interactable che PUO' essere interagito</param>
        public void SetInteractable(Interactable interactable)
        {
            if (!_interactables.Contains(interactable))
                _interactables.Add(interactable);
        }

        /// <summary>
        /// Un oggettto precedentemente interagibile non lo è più. Rimuoviamolo
        /// </summary>
        /// <param name="interactable">L'interactable che NON PUO' PIU' essere interagito</param>
        public bool UnsetInteractable(Interactable interactable)
        {
            return _interactables.Remove(interactable);
        }

        /// <summary>
        /// Interact with the last activated interactable 
        /// </summary>
        public void Interact()
        {
            // Se il player aveva interagito con un oggetto "draggable", l'interazione lascerà invece l'oggetto
            if (_isDragging)
            {
                StopDragging();
            }
            
            if (_interactables.Count <= 0)
                return;

            _interactables[_interactables.Count - 1].Interact(this);
            _interactables.RemoveAt(_interactables.Count - 1);
        }

        /// <summary>
        /// Remove every reference to interactables
        /// </summary>
        public void ResetInteractables()
        {
            foreach (var i in _interactables)
            {
                i.SetVisible(false);
                i.CantInteract(false);
            }
            
            _interactables.Clear();
        }

        /// <summary>
        /// Il player ha interagito con un oggetto "draggable"
        /// </summary>
        /// <param name="obj"></param>
        public void StartDragging(DraggableObject obj)
        {
            _draggable = obj;
            _isDragging = true;
            
            Anim.SetBool(animDragging, true);
            speed = speedDragging;
        }
        
        /// <summary>
        /// Il player ha smesso di interagire con un oggetto "draggable"
        /// </summary>
        /// <param name="obj"></param>
        private void StopDragging()
        {
            _draggable.StopInteract();
            _draggable = null;
            _isDragging = false;
            
            Anim.SetBool(animDragging, false);
            speed = speedRunning;
        }

        /// <summary>
        /// Il player inizia a strisciare a terra
        /// </summary>
        public void StartCrawling(Vector3 direction)
        {
            Anim.SetBool(animCrawling, true);

            _isCrawling = true;

            speed = speedCrawling;

            _forceMovementDirection = direction;

            Rb.useGravity = false;
            // Disable base collider to pass walls
            _baseCollider.enabled = false;
            // Enable crawl collider to trigger specific interactions
            _crawlCollider.enabled = true;
            
            StopMovementTillAnim("LowCrawl");
        }
        
        /// <summary>
        /// Il player finisce si strisciare
        /// </summary>
        public void StopCrawling()
        {
            Anim.SetBool(animCrawling, false);

            _isCrawling = false;
            
            speed = speedRunning;
            
            Rb.useGravity = true;
            _baseCollider.enabled = true;
            _crawlCollider.enabled = false;
            
            StopMovementTillAnim("Blend Tree");
        }

        public void SetClimbable(Transform climbable)
        {
            if (_isClimbing)
            {
                return;
            }
            
            climbablePlatform = climbable;
        }
        
        private void Climb()
        {
            _isClimbing = true;
            
            StartCoroutine("StartClimb");
            
            InputManager.Instance.AddLockedInputAction(() => { });

            // transform.position = new Vector3(climbablePlatform.transform.position.x, climbablePlatform.transform.position.y + climbablePlatform.localScale.y / 2.0f, climbablePlatform.transform.position.z);
        }

        IEnumerator StartClimb()
        {
            float climbTimer = 0;

            Vector3 destPos = new Vector3(climbablePlatform.position.x, climbablePlatform.position.y + climbablePlatform.localScale.y / 2.0f, climbablePlatform.position.z);
            Vector3 startPos = transform.position;
            
            while (climbTimer < timeToClimb)
            {
                climbTimer += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, destPos, climbTimer / timeToClimb);
                yield return null;
            }

            InputManager.Instance.ResetLockedInputAction();

            _isClimbing = false;
            climbablePlatform = null;
        }

        private void TakeCover()
        {
            const int maxCovers = 5;
            var foundCovers = new Collider[maxCovers];
            var foundCount = Physics.OverlapSphereNonAlloc(transform.position, radiusCoverLength, foundCovers, LayerMask.GetMask("HideSpot"));
            Collider selectedCover = null;
            for (int i = 0; i < foundCount; i++)
            {
                var currentCover = foundCovers[i];
                if (!CheckCover(currentCover)) continue;
                selectedCover = currentCover;
                break;
            }

            if (!(selectedCover is null))
            {
                GoToCover();
            }

            bool CheckCover(Collider cover)
            {
                var startPoint = transform.position;
                var endPoint = cover.ClosestPointOnBounds(startPoint);
                endPoint = new Vector3(endPoint.x, startPoint.y, endPoint.z);
                var dir = endPoint - startPoint;
                Debug.DrawRay(startPoint, dir, Color.red, 1.0f);
                // TODO: Implement obstacle check
                return !(Vector3.Angle(transform.forward, dir) > angleCoverCheck * 0.5f);
            }

            void GoToCover()
            {
                // TODO: Implement Go-to-cover movement
            }
        }
        
        #endregion

        public static implicit operator Player(GameObject v)
        {
            return v.GetComponent<Player>();
        }
    }
}