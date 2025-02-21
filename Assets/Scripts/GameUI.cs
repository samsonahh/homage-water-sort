using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    ContainerManager containerManager;

    public TMP_Text winCountText;
    public TMP_Text timerText;
    public TMP_Text moveCountText;
    public TMP_Text emptyCountText;

    private void Awake()
    {
        containerManager = FindAnyObjectByType<ContainerManager>();
    }

    private void Update()
    {
        winCountText.text = $"Wins: {containerManager.wins}";
        timerText.text = $"Time: {Mathf.Round(containerManager.roundTimer)}s";
        moveCountText.text = $"Moves: {containerManager.savedMoves.Count}";
        emptyCountText.text = $"Empty: {containerManager.emptyContainers}";
    }
}