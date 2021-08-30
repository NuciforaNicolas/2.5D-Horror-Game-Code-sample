using System;
using Character_Scripts;
using UnityEngine;
using Character_Scripts.Interactions;
using Managers;

namespace Puzzles
{
    public class Puzzle : Interactable
    {
        [Space]
        [Header("Puzzle settings")]
        [SerializeField] private Transform puzzleContainer;

        // [Tooltip("Offset from camera position, where puzzle should place")] 
        // [SerializeField] private float cameraDist;

        private Transform _cameraTransform;
        private Vector3 _startPos, _startScale, _targetScale;
        private Transform _targetPos;
        private Quaternion _startRot;
        private Quaternion _targetRot;
        private float _tO;
        private float _tC;
        [SerializeField] float scalePuzzle;
        //[SerializeField] private GameObject uiElementToActivate = null;

        //public GameObject UIElementToActivate => uiElementToActivate;

        private void Start()
        {
            //puzzleContainer.localScale = Vector3.zero;

            // Retrieve camera transform from a manager, avoiding numerous calls on camera.main
            // TODO: this solution should not be implemented in the PUZZLES manager
            _cameraTransform = PuzzlesManager.Instance.CameraTransform;

            // Retrieve target position from the manager, avoiding numerous cycles through camera transform
            _targetPos = PuzzlesManager.Instance.PuzzleTargetPos;
            _targetScale = new Vector3(puzzleContainer.localScale.x + scalePuzzle, puzzleContainer.localScale.y + scalePuzzle, puzzleContainer.localScale.z + scalePuzzle);

            _startPos = puzzleContainer.position;
            _startRot = puzzleContainer.rotation;
            _startScale = puzzleContainer.localScale;
            
            // Calcolo qui la rotazione target, sarà sempre la stessa
            _targetRot = Quaternion.LookRotation(_cameraTransform.forward - _cameraTransform.forward * 2, Vector3.up);
            
            _tO = 1;
            _tC = 1;
        }

        public override void Interact(Player author = null)
        {
            base.Interact(author);
            
            PuzzlesManager.Instance.OpenPuzzle(this);

            _tO = 0;
        }

        private void Update()
        {
            // If tOpen < 1 than animate the puzzle toward the camera
            if (_tO < 1)
            {
                _tO += Time.deltaTime;

                // Calcolo adesso la target pos, la telecamera potrebbe ancora essere in movimento
                // Vector3 targetPos = _cameraTransform.position + _cameraTransform.forward * cameraDist;

                puzzleContainer.position = Vector3.Lerp(_startPos, _targetPos.position, _tO);
                puzzleContainer.localScale = Vector3.Lerp(_startScale, _targetScale, _tO);
                puzzleContainer.rotation = Quaternion.Lerp(_startRot, _targetRot, _tO);
            }
            
            // If tClose < 1 than animate the puzzle toward the original position
            if (_tC < 1)
            {
                _tC += Time.deltaTime;

                // Calcolo adesso la target pos, la telecamera potrebbe ancora essere in movimento
                // Vector3 targetPos = _cameraTransform.position + _cameraTransform.forward * cameraDist;
                
                puzzleContainer.position = Vector3.Lerp(_targetPos.position, _startPos, _tC);
                puzzleContainer.localScale = Vector3.Lerp(_targetScale, _startScale, _tC);
                puzzleContainer.rotation = Quaternion.Lerp(_targetRot, _startRot, _tC);
            }

            if (ShelfPuzzle.isPuzzleCompleted()) this.gameObject.SetActive(false);
        }

        /// <summary>
        /// Handle actions to close this puzzle
        /// </summary>
        public void ClosePuzzle()
        {
            _tC = 0;
        }
    }
}