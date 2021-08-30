using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Managers
{
    // verified commit test
    public class InputManager : Singleton<InputManager>
    {
        /// <summary>
        ///     Get current player Horizontal Direction [-1;1]
        /// </summary>
        public float HorizontalDirection { get; private set; }

        /// <summary>
        ///     Get current player Vertical Direction [-1;1]
        /// </summary>
        public float VerticalDirection { get; private set; }

        /// <summary>
        ///     Is the player using the "Free Torch mode"?
        /// </summary>
        public bool FreeTorch { get; private set; }

        //public bool WalkingPressed { get; private set; }

        public bool AimPressed { get; private set; }

        public ControlBindings CurrentControlBindings { get; set; }

        public bool LockedInput { get; private set; }

        private List<Action> _lockedInputActions;
        private bool _lockedInputActionsClear;

        public bool canClimb { get; set; }
        
        protected void Start()
        {
            _lockedInputActions = new List<Action>();
            CurrentControlBindings = new ControlBindings
            {
                Torch = KeyCode.Q, ChargeBattery = KeyCode.R, Interact = KeyCode.F, Climb = KeyCode.Space,
                Skill = KeyCode.Mouse0, FreeTorch = KeyCode.LeftAlt, ThrowObject = KeyCode.Mouse0,
                Attack = KeyCode.Mouse1, Saving = KeyCode.F5, /*Walk = KeyCode.C,*/ Aim = KeyCode.E,
                OpenInventory = KeyCode.I, Crouch = KeyCode.C, TakeCover = KeyCode.B,
            }; // Test initialization
        }

        private void Update()
        {
            if (LockedInput)
            {
                foreach (var lockedInputAction in _lockedInputActions)
                {
                    lockedInputAction?.Invoke();
                }

                return;
            }

            if (_lockedInputActionsClear)
            {
                _lockedInputActions.Clear();
                _lockedInputActionsClear = false;
            }

            if (Input.GetKeyDown(CurrentControlBindings.OpenInventory))
            {
                InventoryToggle?.Invoke();
                return;
            }

            HorizontalDirection = Input.GetAxis("Horizontal");
            VerticalDirection = Input.GetAxis("Vertical");

            FreeTorch = Input.GetKey(CurrentControlBindings.FreeTorch);
            // WalkingPressed = Input.GetKey(CurrentControlBindings.Walk);
            AimPressed = Input.GetKey(CurrentControlBindings.Aim);
            FreeTorch = Input.GetKey(CurrentControlBindings.FreeTorch);
            
            if (Input.GetKeyDown(CurrentControlBindings.Torch)) TorchToggle?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.Interact)) Interacting?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.Skill)) UsingSkill?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.ThrowObject)) ThrowingObject?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.Attack)) Attacking?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.Crouch)) Crouching?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.Climb) && canClimb) Climbing?.Invoke();
            if (Input.GetKeyDown(CurrentControlBindings.Saving))
            {
                Saving?.Invoke();
                SaveManager.Instance.SaveGame();
            }
            if (Input.GetKeyDown(CurrentControlBindings.TakeCover)) TakeCover?.Invoke();

            // if (Input.GetKeyDown(CurrentControlBindings.Object1)) Object1Selected?.Invoke();
            // Other controls
        }

        /*
        /// <summary>
        /// Is the player using the torch?
        /// </summary>
        public bool UseTorch { get; private set; }
        */

        /// <summary>
        ///     Player pressed the Torch button
        /// </summary>
        public event Action TorchToggle;

        /// <summary>
        ///     Player is charging battery
        /// </summary>
        public event Action ChargingBattery;

        /// <summary>
        ///     Player is interacting
        /// </summary>
        public event Action Interacting;

        public event Action Climbing;

        /// <summary>
        ///     Player is using skill
        /// </summary>
        public event Action UsingSkill;

        /// <summary>
        ///     Player is throwing an object
        /// </summary>
        public event Action ThrowingObject;

        /// <summary>
        ///     Player is attacking
        /// </summary>
        public event Action Attacking;
        
        /// <summary>
        ///     Player pressed open inventory button
        /// </summary>
        public event Action InventoryToggle;

        public event Action Saving;

        public event Action Crouching;

        public event Action TakeCover;

        public void AddLockedInputAction(Action action)
        {
            LockedInput = true;
            HorizontalDirection = 0;
            VerticalDirection = 0;
            AimPressed = false;
            FreeTorch = false;
            _lockedInputActions.Add(action);
        }

        public void ResetLockedInputAction()
        {
            LockedInput = false;
            _lockedInputActionsClear = true;
        }

        public void ExecuteEvent(string actionName)
        {
            var selectedEvent = typeof(InputManager).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(info =>
                info.Name.ToLower().Contains(actionName.ToLower()) && info.FieldType == typeof(Action));
            (selectedEvent?.GetValue(this) as Action)?.Invoke();
        }
    }
}