using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/*
 * Script to make buttons inactive when function not available. 
 * Following: https://localjoost.github.io/A-behaviour-to-put-MRKT-buttons-in-a-disabled-state/
 * */
public class ButtonStatusController : MonoBehaviour
{
    private TextMeshPro textMeshPro;
    private Color textOriginalColor;
    private Color iconOriginalColor;
    private Renderer iconRenderer;
    private List<MonoBehaviour> buttonBehaviours;
    private Transform buttonHighLightComponent;
    private bool isInitialized = false;
    public bool initialStatus;

    private void Awake()
    {
        if (!isInitialized)
        {
            isInitialized = true;

            var iconParent = transform.Find("IconAndText");
            textMeshPro = iconParent.GetComponentInChildren<TextMeshPro>();
            iconRenderer = iconParent.Find("UIButtonSpriteIcon").
               gameObject.GetComponent<Renderer>();
            if(iconRenderer == null)
                iconRenderer = iconParent.Find("UIButtonSquareIcon").gameObject.GetComponent<Renderer>();
            buttonHighLightComponent = transform.Find("CompressableButtonVisuals");
            buttonBehaviours = GetComponents<MonoBehaviour>().ToList();
            textOriginalColor = textMeshPro.color;
            iconOriginalColor = iconRenderer.material.color;
        }
    }

    void Start()
    {
        SetStatus(initialStatus);
    }

    public void SetStatus(bool active)
    {
        try
        {
            if (buttonBehaviours.Count > 0 && buttonBehaviours != null)
            {
                foreach (var b in buttonBehaviours.Where(p => (p != this)))
                    b.enabled = active;

                buttonHighLightComponent.gameObject.SetActive(active);
                textMeshPro.color = active ? textOriginalColor : Color.gray;
                iconRenderer.material.color = active ? iconOriginalColor : Color.gray;
            }         
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.Log((buttonHighLightComponent != null) + ", " + (textMeshPro != null) + ", " + (iconRenderer != null));
        }
    }
}
