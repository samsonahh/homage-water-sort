using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour
{
    ContainerManager containerManager;

    public GameObject highlight;

    [System.Serializable]
    public class WaterData
    {
        public SpriteRenderer spriteRenderer;
        public WaterColors waterColor;
    }

    public List<WaterData> colors = new();
    public SpriteRenderer waterPrefab;

    public bool isHovered = false;
    public bool isSelected = false;

    private void Awake()
    {
        containerManager = FindAnyObjectByType<ContainerManager>();
    }

    private void Start()
    {
        highlight.SetActive(false); 
    }

    private void Update()
    {
        HandleClickInput();
    }

    public void ForceAddColor(WaterColors color)
    {
        WaterData data = new WaterData
        {
            spriteRenderer = Instantiate(waterPrefab, transform.position + (colors.Count + 0.5f) * Vector3.up, Quaternion.identity, transform),
            waterColor = color
        };
        data.spriteRenderer.color = containerManager.waterColors[color];
        colors.Add(data);

        if (IsComplete())
        {
            containerManager.CompleteContainer();
        }
    }

    public bool TryAddColor(WaterColors color)
    {
        if(colors.Count == 0)
        {
            ForceAddColor(color);
            return true;
        }

        if (colors.Count == ContainerManager.MAX_WATER_COUNT)
        {
            return false;
        }

        WaterColors topColor = colors[colors.Count - 1].waterColor;
        if (topColor != color) return false;

        ForceAddColor(color);
        return true;
    }

    public List<WaterColors> GetNextColor()
    {
        if(colors.Count == 0)
        {
            return null;
        }
        return new List<WaterColors>() { colors[colors.Count - 1].waterColor };
    }

    public void RemoveTopColor()
    {
        if(colors.Count == 0)
        {
            return;
        }

        int indexToRemove = colors.Count - 1;
        Destroy(colors[indexToRemove].spriteRenderer.gameObject);
        colors.RemoveAt(indexToRemove);
    }

    public bool IsComplete()
    {
        if(colors.Count == 0) return false;
        if (colors.Count < ContainerManager.MAX_WATER_COUNT) return false;

        WaterColors firstColor = colors[0].waterColor;
        for(int i = 1; i < ContainerManager.MAX_WATER_COUNT; i++)
        {
            if(firstColor != colors[i].waterColor) return false;
        }
        return true;
    }

    public bool IsEmpty() => colors.Count == 0; 

    void HandleClickInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isHovered) return;
            containerManager.OnSelectContainer(this);
        }
    }

    void OnMouseEnter()
    {
        isHovered = true;

        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        isHovered = false;

        if (!isSelected)
        {
            highlight.SetActive(false);
        }
    }
}