using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Container : MonoBehaviour
{
    ContainerManager containerManager;

    public GameObject highlight;

    public List<(SpriteRenderer, WaterColors)> waterDatas = new();
    public SpriteRenderer waterPrefab;

    public bool isHovered = false;
    public bool isSelected = false;

    public void AssignContainerManager(ContainerManager containerManager)
    {
        this.containerManager = containerManager;
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
        (SpriteRenderer, WaterColors) data = (
            Instantiate(waterPrefab, transform.position + (waterDatas.Count + 0.5f) * Vector3.up, Quaternion.identity, transform),
            color
            );
        data.Item1.color = containerManager.waterColors[color];
        waterDatas.Add(data);
    }

    public bool TryAddColor(WaterColors color)
    {
        if(waterDatas.Count == 0)
        {
            ForceAddColor(color);
            return true;
        }

        if (waterDatas.Count == ContainerManager.MAX_WATER_COUNT)
        {
            return false;
        }

        WaterColors topColor = waterDatas[waterDatas.Count - 1].Item2;
        if (topColor != color) return false;

        ForceAddColor(color);
        return true;
    }

    public List<WaterColors> GetNextColor()
    {
        if(waterDatas.Count == 0)
        {
            return null;
        }
        return new List<WaterColors>() { waterDatas[waterDatas.Count - 1].Item2 };
    }

    public void RemoveTopColor()
    {
        if(waterDatas.Count == 0)
        {
            return;
        }

        int indexToRemove = waterDatas.Count - 1;
        Destroy(waterDatas[indexToRemove].Item1.gameObject);
        waterDatas.RemoveAt(indexToRemove);
    }

    public bool IsComplete()
    {
        if(waterDatas.Count == 0) return false;
        if (waterDatas.Count < ContainerManager.MAX_WATER_COUNT) return false;

        WaterColors firstColor = waterDatas[0].Item2;
        for(int i = 1; i < ContainerManager.MAX_WATER_COUNT; i++)
        {
            if(firstColor != waterDatas[i].Item2) return false;
        }
        return true;
    }

    public bool IsEmpty() => waterDatas.Count == 0; 

    public void ClearContainer()
    {
        foreach(var data in waterDatas)
        {
            Destroy(data.Item1.gameObject);
        }
        waterDatas.Clear();
    }

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