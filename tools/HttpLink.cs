using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using TMPro;

public class HttpLink : MonoBehaviour
{
    [SerializeField]
    private float CLICK_INTERVAL = 1.0f;

    private TMP_Text m_TextComponent;
    private float m_lastClick;
    private Camera m_Camera;
    private Canvas m_Canvas;

    void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
        if (m_TextComponent.GetType() == typeof(TextMeshProUGUI))
        {
            m_Canvas = gameObject.GetComponentInParent<Canvas>();
            if (m_Canvas != null)
            {
                if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    m_Camera = null;
                else
                    m_Camera = m_Canvas.worldCamera;
            }
        }
        else
        {
            m_Camera = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (TMP_TextUtilities.IsIntersectingRectTransform(m_TextComponent.rectTransform, Input.mousePosition, m_Camera))
        {
            if (Input.touches.Length > 0 || Input.GetMouseButton(0))
            {
                var n = Time.time;
                if (n > m_lastClick + CLICK_INTERVAL)
                {
                    m_lastClick = n;
                    int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextComponent, Input.mousePosition, m_Camera);
                    if (linkIndex != -1)
                    {
                        TMP_LinkInfo linkInfo = m_TextComponent.textInfo.linkInfo[linkIndex];
                        OnLinkSelection(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkIndex);
                    }
                }
            }
        }
    }

    void OnLinkSelection(string linkID, string linkText, int linkIndex)
    {
        Application.OpenURL(linkID);
    }


}
