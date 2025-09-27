using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_SaveSlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI SlotName;
    [SerializeField] TextMeshProUGUI LastSavedTime_Manual;
    [SerializeField] TextMeshProUGUI LastSavedTime_Automatic;

    [SerializeField] Image ManualSaveBackGround;
    [SerializeField] Image AutomaticSaveBackGround;

    [SerializeField] Color DefaultColour = Color.black;
    [SerializeField] Color SelectedColour = Color.gray;

    public UnityEvent<ESaveSlot, ESaveType> OnSlotSelected = new UnityEvent<ESaveSlot, ESaveType>();

    UI_SaveLoadUI.EMode CurrentMode;
    ESaveSlot Slot;
    bool HasManualSave;
    bool HasAutomaticSave;

    public void PrepareForMode(UI_SaveLoadUI.EMode mode, ESaveSlot slot)
    {
        Slot = slot;
        CurrentMode = mode;

      
        HasManualSave = SaveLoadManager.Instance.DoesSaveExist(Slot, ESaveType.Manual);
        HasAutomaticSave = SaveLoadManager.Instance.DoesSaveExist(Slot, ESaveType.Automatic);

        SlotName.text = $"Slot {(int)Slot}";

        if (HasManualSave)
            LastSavedTime_Manual.text = SaveLoadManager.Instance.GetLastSavedTime(Slot, ESaveType.Manual);
        else
            LastSavedTime_Manual.text = CurrentMode == UI_SaveLoadUI.EMode.Save ? "Empty" : "None";

      
        if (HasAutomaticSave)
            LastSavedTime_Automatic.text = SaveLoadManager.Instance.GetLastSavedTime(Slot, ESaveType.Automatic);
        else
            LastSavedTime_Automatic.text = CurrentMode == UI_SaveLoadUI.EMode.Save ? "Empty" : "None";

       
        gameObject.SetActive(true);

        
        AutomaticSaveBackGround.gameObject.SetActive(CurrentMode == UI_SaveLoadUI.EMode.Load);
    }

    private void OnEnable()
    {
        AutomaticSaveBackGround.color = DefaultColour;
        ManualSaveBackGround.color = DefaultColour;
    }

    public void SetSelectedSlot(ESaveSlot slot)
    {
        if (slot == Slot)
        {
            ManualSaveBackGround.color = SelectedColour;
            AutomaticSaveBackGround.color = SelectedColour;
        }
        else
        {
            ManualSaveBackGround.color = DefaultColour;
            AutomaticSaveBackGround.color = DefaultColour;
        }
    }

    public void OnSelectManualSave()
    {
        if (!HasManualSave && CurrentMode == UI_SaveLoadUI.EMode.Load)
            return;

        ManualSaveBackGround.color = SelectedColour;
        AutomaticSaveBackGround.color = DefaultColour;

        OnSlotSelected.Invoke(Slot, ESaveType.Manual);
    }

    public void OnSelectAutomaticSave()
    {
        if (!HasAutomaticSave && CurrentMode == UI_SaveLoadUI.EMode.Load)
            return;

        AutomaticSaveBackGround.color = SelectedColour;
        ManualSaveBackGround.color = DefaultColour;

        OnSlotSelected.Invoke(Slot, ESaveType.Automatic);
    }
}