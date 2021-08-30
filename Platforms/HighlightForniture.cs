using System.Collections;
using UnityEngine;

namespace Platforms
{
    public class HighlightForniture : MonoBehaviour
    {
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private int numberOfMaterial;
        [SerializeField] private float timeToHighLight;
        [SerializeField] [Range(0f, 5f)] private float lerpTimeOn;
        [SerializeField] [Range(0f, 5f)] private float lerpTimeOff;
        [SerializeField] private Color colorA;
        [SerializeField] private Color colorB;
        [SerializeField] private bool stillCollided = false;

        private float _t;

        private void Start()
        {
            stillCollided = false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (stillCollided) {
                return;
            }
            
            if (collision.transform.CompareTag("PlayerBody"))
            {
                stillCollided = true;
                StartCoroutine(nameof(HighLightForniture));
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.transform.CompareTag("PlayerBody"))
            {
                stillCollided = false;
                StartCoroutine(nameof(DisableHighLight));
            }
        }

        private IEnumerator HighLightForniture()
        {
            yield return new WaitForSeconds(timeToHighLight);
            // Debug.Log("HighLightForniture - Still Collided: " + stillCollided);
            for (var i = 0; i < numberOfMaterial; i++)
            {
                var m = GetComponent<Renderer>().materials[i];
                if (stillCollided)
                    StartCoroutine(nameof(EnableEmission), m);
            }
        }

        private IEnumerator DisableHighLight()
        {
            yield return new WaitForSeconds(timeToHighLight);
            for (var i = 0; i < numberOfMaterial; i++)
            {
                var m = GetComponent<Renderer>().materials[i];
                if (!stillCollided)
                    StartCoroutine(nameof(DisableEmission), m);
            }
        }

        private IEnumerator EnableEmission(Material material)
        {
            _t = 0;
            if (!material.IsKeywordEnabled("_EMISSION")) material.EnableKeyword("_EMISSION");
            while (_t < lerpTimeOn)
            {
                _t += Time.deltaTime / lerpTimeOn;
                material.SetColor(EmissionColor, Color.Lerp(colorA, colorB, _t));
                yield return 0;
            }
        }

        private IEnumerator DisableEmission(Material material)
        {
            _t = 0;
            if (material.IsKeywordEnabled("_EMISSION"))
            {
                while (_t < lerpTimeOff)
                {
                    _t += Time.deltaTime / lerpTimeOff;
                    // Debug.Log(material.GetColor(EmissionColor));
                    material.SetColor(EmissionColor, Color.Lerp(colorB, colorA, _t));
                    yield return 0;
                }
            }
            material.DisableKeyword("_EMISSION");
        }
    }
}