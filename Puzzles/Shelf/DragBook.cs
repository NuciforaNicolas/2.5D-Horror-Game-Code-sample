using System;
using UnityEngine;

namespace Puzzles.Shelf
{
    public class DragBook : MonoBehaviour
    {
        [SerializeField] float offSetZ, originalZ, boundary, center, dragSpeed;
        [SerializeField] Slot slot;
        public bool bookTaken, isInRightPlace;
        Vector3 lastMousePos;

        public enum BookNumber
        {
            I = 1, II = 2, III = 3, IV = 4, V = 5, VI = 6, VII = 7, VIII = 8
        }
        public BookNumber bookNumber;

        private void Start()
        {
            slot.setHasBook(true);
        }

        void OnMouseDown()
        {
            if (!isInRightPlace)
            {
                if (CompareTag("KeyBook") && !GetComponent<KeyBook>().isUnlocked()) return;
                bookTaken = true;
                slot.setHasBook(false);
                Vector3 bookPos = transform.localPosition;
                Debug.Log("BookPos: " + bookPos);
                bookPos.z = bookPos.z + offSetZ;
                transform.localPosition = bookPos;
                lastMousePos = Input.mousePosition;
            }
        }

        private void OnMouseUp()
        {
            if (slot != null && bookTaken)
            {
                Vector3 newPos = slot.transform.localPosition;
                newPos.z = originalZ;
                transform.localPosition = newPos;
                bookTaken = false;
                slot.setHasBook(true);
                if ((int)bookNumber == (int)slot.slotNumber)
                {
                    isInRightPlace = true;
                    ShelfPuzzle.increaseBooksCounter();
                }
            }
        }

        void OnMouseDrag()
        {
            if (!isInRightPlace)
            {
                if (CompareTag("KeyBook") && !GetComponent<KeyBook>().isUnlocked()) return;
                Vector3 delta = Input.mousePosition - lastMousePos;
                Vector3 bookPos = transform.localPosition;
                bookPos.x += delta.x * dragSpeed * Time.deltaTime;
                if ((bookPos.x > -boundary) && (bookPos.x < boundary)) transform.localPosition = bookPos;
                lastMousePos = Input.mousePosition;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (bookTaken && other.CompareTag("Slot"))
            {
                if (!other.GetComponent<Slot>().getHasBook())
                {
                    Debug.Log("Slot found");
                    slot = other.GetComponent<Slot>();
                }
            }
        }
    }
}
