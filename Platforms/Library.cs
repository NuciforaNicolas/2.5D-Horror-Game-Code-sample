using Puzzles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Library : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ShelfPuzzle.isPuzzleCompleted())
        {
            PuzzlesManager.Instance.ClosePuzzle();
            anim.SetBool("isPuzzleCompleted", true);
            this.enabled = false;
        }
    }
}
