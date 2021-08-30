using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Puzzles;

public class KeyPadPuzzle : Singleton<KeyPadPuzzle>
{
    [SerializeField] int totalDigits;
    [SerializeField] string inputSequence, sequence;
    [SerializeField] int digitsCounter;
    [SerializeField] bool isPuzzleCompleted;
    [SerializeField] Text digitsText;
    // Start is called before the first frame update
    void Start()
    {
        //TO-DO: rendere la sequenza randomica?
        digitsCounter = 0;
        inputSequence = "";
        digitsText.text = "";
    }

    private void FixedUpdate()
    {
        if(digitsCounter == totalDigits)
        {
            if (checkSequence())
            {
                isPuzzleCompleted = true;
                PuzzlesManager.Instance.ClosePuzzle();
                Debug.Log("KeyPad Puzzle completed");
                this.enabled = false;
            }
        }
    }

    public void insertNumber(char number)
    {
        if(digitsCounter < totalDigits)
        {
            Debug.Log("Inserito numero: " + number);
            inputSequence += number;
            digitsText.text += number;
            digitsCounter++;
        }
    }

    private bool checkSequence()
    {
        if (sequence == inputSequence) return true;
        else
        {
            inputSequence = "";
            digitsText.text = "";
            digitsCounter = 0;
            return false;
        }
    }
}
