using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[ExecuteInEditMode]
public class CustomImage : MonoBehaviour
{
    public Vector2 size = Vector2.one;

    private RectTransform m_Transform;
    private Material m_Material;

    private bool m_Dirty;

    private void OnValidate()
    {
        size.x = Mathf.Clamp(size.x, 0.0f, float.MaxValue);
        size.y = Mathf.Clamp(size.y, 0.0f, float.MaxValue);

        m_Dirty = true;
    }

    private void Awake()
    {
        m_Transform = GetComponent<RectTransform>();
        m_Material = GetComponent<Image>().material;
    }

    private void Update()
    {
        if (!m_Dirty)
            return;
        m_Dirty = false;

        m_Transform.sizeDelta = size;

        if (m_Material == null)
        {
            m_Material = GetComponent<Image>().materialForRendering;
            return;
        }

        m_Material.SetVector("_Size", size);
    }
}
