using Managers;
using UnityEngine;

namespace Puzzles
{
    public class PuzzlesManager : Singleton<PuzzlesManager>
    {
        private Puzzle _currentPuzzle;
        
        private Transform _puzzleTargetPos;
        public Transform PuzzleTargetPos => _puzzleTargetPos;
        
        private Transform _cameraTransform;
        public Transform CameraTransform => _cameraTransform;
        
        protected override void Awake()
        {
            base.Awake();
            
            _cameraTransform = Camera.main.transform;
            
            // Find target transform, where to put puzzles
            // Find target transform, where to put puzzles
            foreach (Transform t in _cameraTransform)
            {
                if (t.CompareTag("PuzzlesTarget"))
                {
                    _puzzleTargetPos = t;
                }
            }
            if (_puzzleTargetPos is null)
            {
                Debug.LogError("Couldn't find puzzles target pos");
            }
        }

        /// <summary>
        /// Handle puzzle activation
        /// </summary>
        /// <param name="puzzle"></param>
        public void OpenPuzzle(Puzzle puzzle)
        {
            // Fire the event to input manager, to prevent other inputs
            InputManager.Instance.AddLockedInputAction(LockedInputActions);
            // Fire the event to ui manager, to show puzzles canvas
            UIManager.Instance.ShowPuzzlesCanvas();

            //if (puzzle.UIElementToActivate != null) puzzle.UIElementToActivate.SetActive(true);

            _currentPuzzle = puzzle;
        }

        private void LockedInputActions()
        {
            var inputManagerInstance = InputManager.Instance;
            if (Input.GetKeyDown(inputManagerInstance.CurrentControlBindings.Interact)) ClosePuzzle();
        }

        /// <summary>
        /// Handle puzzle deactivation
        /// </summary>
        public void ClosePuzzle()
        {
            _currentPuzzle.ClosePuzzle();
            //if (_currentPuzzle.UIElementToActivate != null) _currentPuzzle.UIElementToActivate.SetActive(false);

            // Tell input manager to reactivate inputs
            InputManager.Instance.ResetLockedInputAction();
            // Fire the event to ui manager, to hide puzzles canvas
            UIManager.Instance.HidePuzzlesCanvas();
        }
    }
}