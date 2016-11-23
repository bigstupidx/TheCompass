﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

/// <summary>
/// Logic to handle object selection when a UI element is moused over.
/// Per object, attach this script to individual buttons/sliders.
/// </summary>

[RequireComponent (typeof(Selectable))]
public class UINavigationPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static GameObject mousedOver;
    Selectable sel;

    //void Start()
    void Awake()
    {
        sel = GetComponent<Selectable>();
        //Debug.Log(gameObject + " initialized");
    }

    // An object that is being moused over plays the Highlighted animator/sprite state,
    // but does not appear as selected in EventSystem. This needs to be done manually.

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!sel.IsInteractable()) return;

        mousedOver = gameObject;

        // (SOLVED) The program occasionally coughs up a NullRefException around this part.

        // Reproduction:
        // Enter any menu while LMB is held.
        // The button in the subsequent menu at the mouse position will throw the error.
        // This can be done by holding LMB over a button while pressing the button with Enter.

        // Cause:
        // Menus other than the initial (and subsequently their buttons) start out disabled,
        // and sel was never initialized by the time it is moused over.
        // This only happens once per menu on the very first frame it is enabled.

        // Solution:
        // Move initialization from Start() to Awake().
        // Attach the Late Disable script to every menu that isn't the initial menu,
        // then enable them all by default.
        // Since Start() executes following Awake(), objects are initialized then disabled.

        if (sel == null)
            Debug.Log("NULL detected in " + gameObject);

        sel.Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mousedOver = null;
    }

    /// <summary>
    /// Corrects a behavior where an object becomes visually highlighted when it is
    /// moused over, regardless of whether it is actually being selected. <para/>
    /// If a directional button/axis input occurs during this, two objects now appear
    /// highlighted: the next in the navigation graph and the one underneath the pointer.
    /// </summary>

    public static void ResetMousedOver()
    {
        if (mousedOver != null && UIHelper.selected != null &&
            UIHelper.selected != mousedOver)
        {
            // Sets the object's visual state to Normal.
            // Requires an auto-generated Animator, otherwise the problem persists.

            Animator anim = mousedOver.GetComponent<Animator>();
            if (anim != null && anim.isInitialized)
                anim.SetTrigger(640249298); // = "Normal"

            // + Avoids hardcoding the trigger hash
            // - Passes by string
            //Selectable sel = mousedOver.GetComponent<Selectable>();
            //Animator anim = sel.animator;
            //if (anim != null && anim.isInitialized)
            //    anim.SetTrigger(sel.animationTriggers.normalTrigger);
        }
    }

}
