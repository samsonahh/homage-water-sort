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
    Container selectedContainer;

    public int wins;
    bool isRoundWon;
    public float roundTimer;

    Dictionary<Container, List<WaterColors>> cachedInitialRoundState = new();
    public List<(Container, Container)> savedMoves = new();

    private void Awake()
    {
        foreach(Container container in containers)
        {
            container.AssignContainerManager(this);
        }

        SetupContainers();
    }

    private void Update()
    {
        CheckWin();
        if (!isRoundWon && roundTimer < 10000) roundTimer += Time.deltaTime;
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
            if (emptyContainerIndices.Contains(i))
            {
                cachedInitialRoundState.Add(containers[i], new());
                continue;
            }

            List<WaterColors> containerColors = new();
            for(int j = 0; j < MAX_WATER_COUNT; j++)
            {
                int randomIndex = Random.Range(0, possibleColors.Count);
                WaterColors randomColor = possibleColors[randomIndex];
                containers[i].ForceAddColor(randomColor);
                containerColors.Add(randomColor);
                possibleColors.RemoveAt(randomIndex);
            }
            cachedInitialRoundState.Add(containers[i], containerColors);
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
        if (newContainer.TryAddColor(incomingColor))
        {
            selectedContainer.RemoveTopColor();
            savedMoves.Add((selectedContainer, newContainer));
        }
        newContainer.highlight.SetActive(false);
        newContainer.isSelected = false;
        selectedContainer.isSelected = false;
        selectedContainer.highlight.SetActive(false);
        selectedContainer = null;
    }

    void CheckWin()
    {
        if (isRoundWon) return;

        int completeCount = 0;
        foreach(Container container in containers)
        {
            if (container.IsComplete()) completeCount++;
        }

        if(completeCount >= containers.Length - emptyContainers)
        {
            isRoundWon = true;
            wins++;
        }
    }

    void ClearAllContainers()
    {
        foreach(Container container in containers)
        {
            container.ClearContainer();
        }
    }

    public void UndoMove()
    {
        if (isRoundWon) return;
        if(savedMoves.Count == 0) return;

        (Container removedContainer, Container addedContainer) = savedMoves[savedMoves.Count - 1];
        removedContainer.ForceAddColor(addedContainer.GetNextColor()[0]);
        addedContainer.RemoveTopColor();

        savedMoves.RemoveAt(savedMoves.Count - 1);
    }

    public void ResetGame()
    {
        if (isRoundWon)
        {
            CreateNewGame();
            return;
        }

        roundTimer = 0;
        savedMoves.Clear();

        ClearAllContainers();
        foreach(var keyValue in cachedInitialRoundState)
        {
            Container container = keyValue.Key;
            List<WaterColors> colors = keyValue.Value;

            foreach(var color in colors)
            {
                container.ForceAddColor(color);
            }
        }
    }

    public void CreateNewGame()
    {
        isRoundWon = false;
        roundTimer = 0;
        savedMoves.Clear();
        cachedInitialRoundState.Clear();

        ClearAllContainers();
        SetupContainers();
    }

    public void AddEmptyContainers()
    {
        if (emptyContainers + 1 == containers.Length - 1) return;

        emptyContainers++;
        CreateNewGame();
    }

    public void RemoveEmptyContainers()
    {
        if (emptyContainers - 1 == 0) return;

        emptyContainers--;
        CreateNewGame();
    }
}
