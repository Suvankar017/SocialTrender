using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostOpener : MonoBehaviour
{
    public PostHolder m_Holder;

    public void OpenPost()
    {
        m_Holder.OnPointerClick(null);
    }
}
