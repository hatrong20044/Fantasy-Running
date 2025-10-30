using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<UIName, GameObject> activeUIs = new Dictionary<UIName, GameObject>();
    [SerializeField] private Transform uiRoot; 
    protected override void Awake()
    {
        base.Awake();

        if (uiRoot == null)
        {
            GameObject rootCanvasGO = GameObject.FindWithTag("MainCanvas");
            if (rootCanvasGO != null)
            {
                uiRoot = rootCanvasGO.transform;
            }
            
        }
    }

    /// <summary>
    /// Hiển thị UI prefab từ Resources/UI/
    /// </summary>
    public GameObject ShowUI(UIName ui)
    {
        if (activeUIs.ContainsKey(ui))
            return activeUIs[ui];

        string uiName = ui.ToString();
        GameObject prefab = Resources.Load<GameObject>($"UI/{uiName}");
        if (prefab == null)
        {
            return null;
        }

        GameObject uiInstance = Instantiate(prefab, uiRoot);
        uiInstance.SetActive(true);

       
        RectTransform rect = uiInstance.GetComponent<RectTransform>();
        if (rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        activeUIs[ui] = uiInstance;
        return uiInstance;
    }

    public void HideUI(UIName ui)
    {
        if (activeUIs.TryGetValue(ui, out GameObject uiObj))
        {
            Destroy(uiObj);
            activeUIs.Remove(ui);
        }
    }

    public void HideAllUI()
    {
        foreach (var ui in activeUIs.Values)
            Destroy(ui);
        activeUIs.Clear();
    }
    public T GetActiveUI<T>(UIName ui) where T : Component
    {
        if (activeUIs.TryGetValue(ui, out GameObject uiObj))
            return uiObj.GetComponent<T>();
        return null;
    }

}