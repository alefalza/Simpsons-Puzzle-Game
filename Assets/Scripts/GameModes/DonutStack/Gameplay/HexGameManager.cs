using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModes.DonutStack.Core;
using GameModes.DonutStack.UI;
using UnityEngine;

namespace GameModes.DonutStack.Gameplay
{
    public class HexGameManager : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private HexGrid hexGrid;
        [SerializeField] private int gridRadius = 3;
    
        [Header("Piece Settings")]
        [SerializeField] private PieceStack pieceStackPrefab;
        
        [Header("Game Settings")]
        [SerializeField] private int stacksPerTurn = 3;
        [SerializeField] private Transform stackContainer;
        [SerializeField] private Transform dragLayer;
        [SerializeField] private int piecesToDestroy = 10;
    
        [Header("HUD")]
        [SerializeField] private HexHUDController hudController;
        
        private readonly List<PieceStack> currentTurnStacks = new List<PieceStack>();
        
        private int score = 0;

        public static HexGameManager Instance { get; private set; }
        
        public Transform StackContainer => stackContainer;
        public Transform DragLayer => dragLayer;
        
        public bool IsPaused { get; private set; } = false;
        public bool IsInputBlocked { get; private set; } = false;
        public bool IsProcessingMatches { get; private set; } = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            hexGrid.Initialize(gridRadius);
            UpdateScoreUI();
            GenerateNewTurn();
        }

        private void Update()
        {
            GetInput();
        }

        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
            
            if (IsInputBlocked) return;
        }
        
        private void UpdateScoreUI()
        {
            hudController.UpdateScore(score);
        }
        
        private void GenerateNewTurn()
        {
            foreach (var stack in currentTurnStacks)
            {
                if (stack != null && !stack.IsPlaced)
                {
                    Destroy(stack.gameObject);
                }
            }
            
            currentTurnStacks.Clear();

            for (int i = 0; i < stacksPerTurn; i++)
            {
                PieceStack stack = CreateRandomStack();
                currentTurnStacks.Add(stack);
            }
        }

        private PieceStack CreateRandomStack()
        {
            PieceStack stack = Instantiate(pieceStackPrefab, stackContainer.transform);
        
            if (stack == null)
            {
                Debug.LogError("PieceStack component not found on prefab!");
                return null;
            }
            
            stack.Initialize();
        
            return stack;
        }

        public void TryPlaceStack(HexCell cell, PieceStack stack)
        {
            if (stack == null || IsProcessingMatches) return;
            if (cell.IsOccupied) return;
        
            cell.SetStack(stack);
            stack.PlaceOnCell(cell);
            currentTurnStacks.Remove(stack);
        
            StartCoroutine(ProcessMatchesRecursive(cell));
        
            if (AllStacksPlaced())
            {
                Invoke(nameof(GenerateNewTurn), 0.5f);
            }
        }

        private bool AllStacksPlaced()
        {
            return currentTurnStacks.All(stack => stack == null || stack.IsPlaced);
        }

        private IEnumerator ProcessMatchesRecursive(HexCell startCell)
        {
            IsProcessingMatches = true;
        
            yield return StartCoroutine(ProcessCellMatchesRecursively(startCell));
        
            IsProcessingMatches = false;
            CheckGameOver();
        }

        private IEnumerator ProcessCellMatchesRecursively(HexCell cell)
        {
            if (!cell.IsOccupied) yield break;

            bool foundMatch = true;

            while (foundMatch && cell.IsOccupied)
            {
                foundMatch = false;

                List<HexCell> neighbours = hexGrid.GetNeighbours(cell);
                PieceColor currentTopColor = cell.Stack.GetTopColor();

                foreach (var neighbour in neighbours)
                {
                    if (!neighbour.IsOccupied) continue;

                    // If colors at the top match, move pieces from one stack to the other.
                    if (neighbour.Stack.GetTopColor() == currentTopColor)
                    {
                        List<Piece> piecesToMove = cell.Stack.RemovePiecesOfColor(currentTopColor);

                        foreach (var piece in piecesToMove)
                            neighbour.Stack.AddPiece(piece);

                        neighbour.Stack.ArrangePieces();

                        yield return new WaitForSeconds(0.2f);

                        // If origin stack ends up empty after moving its pieces to the target stack, destroy it.
                        if (cell.Stack.PieceCount == 0)
                        {
                            Destroy(cell.Stack.gameObject);
                            cell.ClearStack();
                        }
                        else
                        {
                            cell.Stack.ArrangePieces();
                        }

                        int topColorCount = neighbour.Stack.TopColorCount();
                        
                        // Destroy pieces of the same color if amount to destroy is reached.
                        if (neighbour.IsOccupied && topColorCount >= piecesToDestroy)
                        {
                            yield return StartCoroutine(
                                neighbour.Stack.RemoveTopGroupWithDelay(piecesToDestroy, 0.05f)
                            );
                            
                            score += topColorCount;
                            UpdateScoreUI();

                            // If the stack ends up empty after destroying its top pieces, destroy it.
                            if (neighbour.Stack.PieceCount == 0)
                            {
                                Destroy(neighbour.Stack.gameObject);
                                neighbour.ClearStack();
                                yield break;
                            }

                            yield return new WaitForSeconds(0.2f);
                        }
                        else if (neighbour.IsOccupied)
                        {
                            yield return StartCoroutine(ProcessCellMatchesRecursively(neighbour));
                        }

                        if (cell.IsOccupied)
                        {
                            foundMatch = true;
                            break;
                        }
                        else
                        {
                            yield break;
                        }
                    }
                }
            }
        }

        #region Pause Logic
        private void TogglePause()
        {
            if (!hudController.CanTogglePause()) return;

            if (!IsPaused)
                Pause();
            else
                Resume();
        }

        private void Pause()
        {
            IsPaused = true;
            IsInputBlocked = true;

            hudController.ShowPausePopup();
        }

        private void Resume()
        {
            IsPaused = false;
            IsInputBlocked = false;

            hudController.HidePausePopup();
        }

        public void TogglePauseFromOverlay()
        {
            Resume();
        }
        #endregion

        private void CheckGameOver()
        {
            if (!hexGrid.HasEmptyCells() && currentTurnStacks.Count > 0)
            {
                bool canPlaceAny = false;
            
                foreach (var stack in currentTurnStacks)
                {
                    if (stack != null && !stack.IsPlaced)
                    {
                        canPlaceAny = true;
                        break;
                    }
                }
            
                if (canPlaceAny)
                {
                    OnGameOver();
                }
            }
        }
        
        private void OnGameOver()
        {
            IsInputBlocked = true;
            hudController.ShowGameOverOverlay(score);
        }
    }
}
