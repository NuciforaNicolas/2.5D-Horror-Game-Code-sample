using System;
using System.Collections;
using Character_Scripts;
using Inventory;
using Managers;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private int battery;
    //[SerializeField] private GameObject currentCollider;
    [SerializeField] private GameObject fullLight;
    [SerializeField] private GameObject emptyLight;
    [SerializeField] private GameObject currentLight;
    [SerializeField] private int maxBattery;
    [SerializeField] private Player player;
    [SerializeField] private int time;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Quaternion startRotation;
    bool _batteryCoolDownState;
    
    private Camera _camera;
    private bool _isOn;
    private ItemDatabase _itemDatabase;

    public event Action TorchDisabled;

    private UIManager _uiManager;

    private void Start()
    {
        _itemDatabase = Resources.Load<ItemDatabase>(ResourceRegister.GetResourceString(ResourceRegisterType.ItemDatabase));
        _camera = Camera.main;
        SwitchOff();
        startRotation = currentLight.transform.localRotation;

        _uiManager = UIManager.Instance;
        
        // Al primo avvio modifichiamo lo status della torcia nell'inventario
        if (_uiManager is null)
        {
            Debug.LogWarning("Torch Start(): uimanager not set!");
            return;
        }
        
        UpdateTorchStatus();
    }

    private void LateUpdate()
    {
        if (InputManager.Instance.FreeTorch)
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100, layerMask))
            {
                currentLight.transform.LookAt(hit.point);
            }
        }
        else
        {
            currentLight.transform.localRotation = startRotation;
        }
    }

    private void OnEnable()
    {
        InputManager.Instance.ChargingBattery += ChargeBattery;
        InputManager.Instance.TorchToggle += Toggle;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;
        InputManager.Instance.ChargingBattery -= ChargeBattery;
        InputManager.Instance.TorchToggle -= Toggle;
    }

    private void SwitchOn()
    {
        // if (battery <= 0) return;
        //currentLight.SetActive(true);
        currentLight.SetActive(true);
        _isOn = true;
        //currentCollider.SetActive(true);
        StartCoroutine(nameof(BatteryCoolDown));
    }

    private void SwitchLight(GameObject torchLight){
        currentLight.SetActive(false);
        currentLight = torchLight;
        if(_isOn){
            currentLight.SetActive(true);
        }
	}

    private void SwitchOff()
    {
        TorchDisabled?.Invoke();
        // currentLight.SetActive(false);
        currentLight.SetActive(false);
        _isOn = false;
        //currentCollider.SetActive(false);
        StopCoroutine(nameof(BatteryCoolDown));
        
        player.ResetInteractables();
    }

    private void ChargeBattery()
    {
        if (battery >= maxBattery) return;

        Consumable batteryItem = (Consumable)_itemDatabase.GetItem("Battery");
        if (!player.Inventory.RemoveItem(batteryItem, 1)) return;

        if(battery <= 0){
            SwitchLight(fullLight);
            if (!_batteryCoolDownState && _isOn)
                StartCoroutine(nameof(BatteryCoolDown));
        }

        battery += (int)batteryItem.GetValue();

        // Se supero il massimo
        battery = Mathf.Min(battery, maxBattery);
        
        UpdateTorchStatus();
    }

    private IEnumerator BatteryCoolDown()
    {
        _batteryCoolDownState = true;
        while (battery > 0)
        {
            yield return new WaitForSeconds(time);
            battery--;
            UpdateTorchStatus();
        }
        if (battery <= 0)
            SwitchLight(emptyLight);
        _batteryCoolDownState = false;
    }

    private void Toggle()
    {
        if (_isOn)
            SwitchOff();
        else
            SwitchOn();
    }

    private void UpdateTorchStatus()
    {
        int charge = (battery * 100) / maxBattery;
        
        _uiManager.SetTorchBattery(charge);
    }
}