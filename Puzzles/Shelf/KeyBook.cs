using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBook : MonoBehaviour
{
    MeshRenderer mesh;
    [SerializeField] Material shadowMaterial, normalMaterial;
    bool unlocked;

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material = shadowMaterial;
        unlocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) /*&&  controllare nell'inventario se presente il libro*/)
        {
            mesh.material = normalMaterial;
            unlocked = true;
        }
    }

    public bool isUnlocked()
    {
        return unlocked;
    }
}
