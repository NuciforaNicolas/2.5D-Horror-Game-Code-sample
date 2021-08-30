using System;
using System.Collections;
using System.Collections.Generic;
using Character_Scripts;
using Managers;
using UnityEngine;

public class ClimbableObject : MonoBehaviour
{
    [SerializeField] bool canClimb;

    private void Awake()
    {
        canClimb = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerBody"))
        {
            InputManager.Instance.canClimb = true;
            other.GetComponentInParent<Player>().SetClimbable(transform);
            canClimb = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("PlayerBody")){
            InputManager.Instance.canClimb = false;
            other.GetComponentInParent<Player>().SetClimbable(null);
            canClimb = false;
        }
    }

    public bool isClimbable()
    {
        return canClimb;
    }
}
