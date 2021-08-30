using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyNumber : MonoBehaviour
{
    [SerializeField] char number;

    private void OnMouseDown()
    {
        KeyPadPuzzle.Instance.insertNumber(number);
    }
}
