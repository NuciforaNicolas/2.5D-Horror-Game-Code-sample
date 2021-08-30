using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Character_Scripts
{
    [RequireComponent(typeof(BoxCollider))]
    public class Npc : IA
    {
        private BoxCollider _collider;
        private TextMeshProUGUI talk_text;
        
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            _collider = GetComponent<BoxCollider>();
            if (_collider is null)
            {
                Debug.LogWarning(gameObject.name + ": Nessun collider trovato");
                return;
            }

            Transform canvas = transform.GetComponentInChildren<Canvas>().transform;
            if (canvas is null)
            {
                Debug.LogWarning(gameObject.name + ": Nessun canvas trovato");
                return;
            }

            talk_text = canvas.GetComponentInChildren<TextMeshProUGUI>();
            if (talk_text is null)
            {
                Debug.LogWarning(gameObject.name + ": Nessun TMP text trovato");
                return;
            }
            
            talk_text.text = "Ehi tu, vieni qui!";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                talk_text.text = "Perfavore... aiutami!";
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                talk_text.text = "Ehi tu, vieni qui!";
            }
        }
    }
}