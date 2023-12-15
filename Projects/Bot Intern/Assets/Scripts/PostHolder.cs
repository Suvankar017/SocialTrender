using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PostHolder : MonoBehaviour, IPointerClickHandler
{
    public string link;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(link) || string.IsNullOrWhiteSpace(link))
            return;

        Application.OpenURL(link);
    }
}
