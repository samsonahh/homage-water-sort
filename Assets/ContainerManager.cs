using AYellowpaper.SerializedCollections;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ContainerManager : MonoBehaviour
{
    public static int MAX_WATER_COUNT = 4;

    [SerializedDictionary("Color Type", "Color")]
    public SerializedDictionary<WaterColors, Color> waterColors = new();

    public Container[] containers;
    public int emptyContainers = 2;
    public Container selectedContainer;
    public int completedContainers;

    private void Awake()
    {
        SetupContainers();
    }

    void SetupContainers()
    {
        // Get the possible colors first
        List<WaterColors> allColors = new List<WaterColors>((WaterColors[])System.Enum.GetValues(typeof(WaterColors)));
        List<WaterColors> possibleColors = new();
        for(int i = 0; i < containers.Length - emptyContainers; i++)
        {
            WaterColors randomColor = allColors[Random.Range(0, allColors.Count)];
            for(int j = 0; j < MAX_WATER_COUNT; j++) possibleColors.Add(randomColor);
            allColors.Remove(randomColor);
        }

        // Get the empty container indices
        List<int> fullContainerIndices = new List<int>();
        for(int i = 0; i < containers.Length; i++) fullContainerIndices.Add(i);
        List<int> emptyContainerIndices = new List<int>();
        for(int i = 0; i < emptyContainers; i++)
        {
            int randomIndex = Random.Range(0, fullContainerIndices.Count);
            emptyContainerIndices.Add(fullContainerIndices[randomIndex]);
            fullContainerIndices.RemoveAt(randomIndex);
        }
        
        // Fill the non empty containers
        for(int i = 0; i < containers.Length; i++)
        {
            if (emptyContainerIndices.Contains(i)) continue;
            for(int j = 0; j < MAX_WATER_COUNT; j++)
            {
                int randomIndex = Random.Range(0, possibleColors.Count);
                WaterColors randomColor = possibleColors[randomIndex];
                containers[i].ForceAddColor(randomColor);
                possibleColors.RemoveAt(randomIndex);
            }
        }
    }

    public void OnSelectContainer(Container newContainer)
    {
        if (selectedContainer == null)
        {
            if (newContainer.IsComplete()) return;
            if(newContainer.IsEmpty()) return;  

            newContainer.highlight.SetActive(true);
            newContainer.isSelected = true;
            selectedContainer = newContainer;
            return;
        }

        // new container is done
        if (newContainer.IsComplete())
        {
            selectedContainer.highlight.SetActive(false);
            selectedContainer.isSelected = false;
            newContainer.highlight.SetActive(false);
            selectedContainer.isSelected = false;
            
            selectedContainer = null;
            return;
        }

        WaterColors incomingColor = selectedContainer.GetNextColor()[0];
        if (newContainer.TryAddColor(incomingColor)) selectedContainer.RemoveTopColor();
        newContainer.highlight.SetActive(false);
        newContainer.isSelected = false;
        selectedContainer.isSelected = false;
        selectedContainer.highlight.SetActive(false);
        selectedContainer = null;
    }
    
    public void CompleteContainer()
    {
        completedContainers++;
        if(completedContainers >= containers.Length - emptyContainers)
        {
            Debug.Log("You win!");
        }
    }
}
