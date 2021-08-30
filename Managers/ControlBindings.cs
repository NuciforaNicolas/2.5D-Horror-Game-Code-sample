using System;
using Inventory;
using UnityEngine;

namespace Managers
{
    /// <summary>
    ///     Control Bindings data structure. <see cref="SaveManager" /> should save this structure onto the save file
    /// </summary>
    [Serializable]
    public class ControlBindings
    {
        /// <summary>
        ///     "Use Torch" keycode binding
        /// </summary>
        public KeyCode Torch { get; set; }

        /// <summary>
        ///     "Charge Torch battery" keycode binding
        /// </summary>
        public KeyCode ChargeBattery { get; set; }

        /// <summary>
        ///     "Interact with an item" keycode binding
        /// </summary>
        public KeyCode Interact { get; set; }

        /// <summary>
        ///     "Jump" keycode binding
        /// </summary>
        public KeyCode Climb { get; set; }

        /// <summary>
        ///     "Use your skill" keycode binding
        /// </summary>
        public KeyCode Skill { get; set; }

        /// <summary>
        ///     Whenever torch should follow mouse position keycode binding
        /// </summary>
        public KeyCode FreeTorch { get; set; }

        /// <summary>
        ///     "Throw an object" keycode binding
        /// </summary>
        public KeyCode ThrowObject { get; set; }

        /// <summary>
        ///     "Attack" keycode binding
        /// </summary>
        public KeyCode Attack { get; set; }

        /// <summary>
        ///     "Walk" keycode binding
        /// </summary>
        public KeyCode Walk { get; set; }

        /// <summary>
        ///     "Aim" keycode binding
        /// </summary>
        public KeyCode Aim { get; set; }

        /// <summary>
        ///     "OpenInventory" keycode binding
        /// </summary>
        public KeyCode OpenInventory { get; set; }

        // Other controls
        public KeyCode Saving { get; set; }

        public KeyCode Crouch { get; set; }

        public KeyCode TakeCover { get; set; }

        //public KeyCode Object1 { get; set; }
    }
}