using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class MenuManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public GameObject firstSelectedGameObject;

    private bool active = false;

    public void ChangeLocale(int localeID)
    {
        if (active)
            return;
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int _localeID)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        active = false;
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("UI");
        EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);

    }
}
