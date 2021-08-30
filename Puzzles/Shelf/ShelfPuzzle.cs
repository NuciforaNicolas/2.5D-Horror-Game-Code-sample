using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfPuzzle : MonoBehaviour
{
    private static int _rightBookCounter;
    [SerializeField] int _totalBooks;
    private static bool _isPuzzleCompleted;

    private void Awake()
    {
        _rightBookCounter = 0;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rightBookCounter == _totalBooks)
        {
            _isPuzzleCompleted = true;
            Debug.Log("Puzzle COmpleted");
        }
    }

    public static void increaseBooksCounter()
    {
        _rightBookCounter += 1;
        Debug.Log("Book Counter: " + _rightBookCounter);
    }

    public static bool isPuzzleCompleted()
    {
        return _isPuzzleCompleted;
    }
}
