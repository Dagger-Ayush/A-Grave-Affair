using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;





public class UI_SaveLoadUI : MonoBehaviour
{
    [SerializeField] EMode Mode;
    [SerializeField] Transform SlotUIRoot;
    [SerializeField] GameObject SlotUIPrefab;

    [SerializeField] GameObject SaveLoadButton;
    [SerializeField] TextMeshProUGUI SaveLoadButtonText;

    [SerializeField] UnityEvent OnLevelLoadReady = new UnityEvent();
    
    
    List<UI_SaveSlot> AllSlots = new List<UI_SaveSlot>();

    ESaveSlot SelectedSlot;
    ESaveType SelectedType;


    public enum EMode
    {
        Save,
        Load
    }

    private void OnEnable()
    {
        SaveLoadButton.SetActive(false);
    }
    void Start()
    {
        RefreshUI();
    }
    void RefreshUI()
    {
        SaveLoadButtonText.text = Mode == EMode.Load ? "Load Save Game" : "Save Game";

        foreach(var slotUI in AllSlots)
        {
            Destroy(slotUI.gameObject);
        }
        AllSlots.Clear();
        var allSlots = System.Enum.GetValues(typeof(ESaveSlot));
        foreach (var slot in allSlots)
        {
            var slotEnum = (ESaveSlot)slot;

            if (slotEnum == ESaveSlot.None)
            {
                continue;

            }
           var slotUIGO =  Instantiate(SlotUIPrefab, SlotUIRoot);
           var slotUI = slotUIGO.GetComponent<UI_SaveSlot>();

           slotUI.PrepareForMode(Mode,slotEnum);
           slotUI.OnSlotSelected.AddListener(OnSlotSelected);
                      
           slotUI.PrepareForMode(Mode,slotEnum);
           AllSlots.Add(slotUI);
        }

        SelectedSlot=ESaveSlot.None;
        SaveLoadButton.SetActive(false);
            
           


    }
    void Update()
    {



        
    }



    void OnSlotSelected(ESaveSlot slot,ESaveType saveType)
    {
        SelectedSlot = slot;
        SelectedType = Mode == EMode.Save ? ESaveType.Manual : saveType;
        SaveLoadButton.SetActive(true);

        foreach(var slotUI in AllSlots)
        {
            slotUI.SetSelectedSlot(slot);
        }

    }
    public void OnPerformSaveLoad()
    {
        if (Mode == EMode.Load)
        {
            SaveLoadManager.Instance.RequestLoad(SelectedSlot, SelectedType);
            OnLevelLoadReady.Invoke();
        }
        else
        {
            SaveLoadManager.Instance.RequestSave(SelectedSlot, SelectedType);
            RefreshUI();
        }
    }
    
    public void SetMode_Save()
    {
        if (Mode == EMode.Save)
            return;
        Mode = EMode.Save;
        RefreshUI();
    }
    public void SetMode_Load()
    {
        if (Mode == EMode.Load)
            return;
        Mode = EMode.Load;
        RefreshUI();
    }
}
