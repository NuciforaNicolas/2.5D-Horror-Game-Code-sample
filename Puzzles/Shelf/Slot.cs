using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzles.Shelf
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] bool hasBook;

        public enum SlotNumber
        {
            I = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7, VIII = 8, IX = 9
        }
        public SlotNumber slotNumber;

        public void setHasBook(bool hasBook)
        {
            this.hasBook = hasBook;
        }

        public bool getHasBook()
        {
            return hasBook;
        }
    }
}

