using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Managers.StressManager;

public class LightningManager : MonoBehaviour
{
    [SerializeField] private float minTimeToLight;
    [SerializeField] private float maxTimeToLight;
    [SerializeField] private float timeToLight;
    [SerializeField] private int minNumberOfLightning; // number of lightning per time
    [SerializeField] private int maxNumberOfLightning;
    [SerializeField] private int numberOfLightning;
    [SerializeField] private float minTimeNextLightning;
    [SerializeField] private float maxTimeNextLightning;
    [SerializeField] private float timeNextLightning;

    [SerializeField] Light directionalLight;
    [SerializeField] float lightIntenisty;
    [SerializeField] private float minLerpTimeLight;
    [SerializeField] private float maxLerpTimeLight;
    [SerializeField] private float lerpTimeLight;
    private float timer;

	private void Start()
	{
        directionalLight.intensity = 0;
        timeToLight = Random.Range(minTimeToLight, maxTimeToLight);
        numberOfLightning = Random.Range(minNumberOfLightning, maxNumberOfLightning);

        StartCoroutine("Lightning");
	}

    IEnumerator Lightning(){
        while(true){
            yield return new WaitForSeconds(timeToLight);
            //StressManager.Instance.SetTransition(Transition.StrMng_LightningStrike);
            for (int i = 0; i < numberOfLightning; i++){
                directionalLight.intensity = lightIntenisty;
                StartCoroutine("ReduceLightningIntensity");
                timeNextLightning = Random.Range(minTimeNextLightning, maxTimeNextLightning);
                yield return new WaitForSeconds(timeNextLightning);
			}
            timeToLight = Random.Range(minTimeToLight, maxTimeToLight);
            numberOfLightning = Random.Range(minNumberOfLightning, maxNumberOfLightning);
        }
	}

    private IEnumerator ReduceLightningIntensity()
    {
        timer = 0;
        lerpTimeLight = Random.Range(minLerpTimeLight, maxLerpTimeLight);
        while (timer < lerpTimeLight)
        {
            timer += Time.deltaTime / lerpTimeLight;
            directionalLight.intensity = Mathf.Lerp(lightIntenisty, 0, timer);
            yield return 0;
        }
    }
}
