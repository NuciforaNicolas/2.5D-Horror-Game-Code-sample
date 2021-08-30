using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SeeThrough : MonoBehaviour
{
    [SerializeField] static int posId = Shader.PropertyToID("_Position");
    [SerializeField] static int sizeId = Shader.PropertyToID("_Size");
    [SerializeField] static int opacityId = Shader.PropertyToID("_Opacity");

    [SerializeField] Material wallMaterial;
    //[SerializeField] SkinnedMeshRenderer playerMaterial;
    [SerializeField] Camera camera;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] float rayDist, yOffset, timeToOpacity, opacity;
    bool blockCoroutinesCalls = false;
    float _timerOpacity;

    private void Awake()
    {
        opacity = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var dir = camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if(Physics.Raycast(ray, out var hit ,rayDist, wallLayer)){
            wallMaterial = hit.transform.GetComponent<MeshRenderer>().materials[0];
            wallMaterial.SetFloat(opacityId, opacity);
            //playerMaterial.shadowCastingMode = ShadowCastingMode.Off;
            if (opacity <= 0 && !blockCoroutinesCalls){
                Debug.Log("Starting increase op");
                StartCoroutine("IncreaseOpacity");
            }
            var playerPos = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
            var view = camera.WorldToViewportPoint(playerPos);
            wallMaterial.SetVector(posId, view);
        }
        else{
            //playerMaterial.shadowCastingMode = ShadowCastingMode.On;
            if (opacity > 0 && !blockCoroutinesCalls){
                StartCoroutine("DecreaseOpacity");
            }
        } 
    }

    IEnumerator IncreaseOpacity(){
        blockCoroutinesCalls = true;
        _timerOpacity = 0;
        while(_timerOpacity < timeToOpacity){
            _timerOpacity += Time.deltaTime;
            opacity = Mathf.Lerp(0, 1, _timerOpacity / timeToOpacity);
            wallMaterial.SetFloat(opacityId, opacity);
            yield return null;
        }
        blockCoroutinesCalls = false;
    }
    IEnumerator DecreaseOpacity()
    {
        blockCoroutinesCalls = true;
        _timerOpacity = 0;
        while (_timerOpacity < timeToOpacity)
        {
            _timerOpacity += Time.deltaTime;
            opacity = Mathf.Lerp(1, 0, _timerOpacity / timeToOpacity);
            wallMaterial.SetFloat(opacityId, opacity);
            yield return null;
        }
        blockCoroutinesCalls = false;
    }
}
