using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private int piecesToDestroy = 10;
    
        [Header("UI")]
        [SerializeField] private HexHUDController hudController;
        
        private readonly List<PieceStack> currentTurnStacks = new List<PieceStack>();
        
        private int score = 0;

        public static HexGameManager Instance { get; private set; }
        
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
            hexGrid.InitializeGrid(gridRadius);
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
                stack.transform.position = new Vector3(-4 + i * 2, -4, 0); // TODO: remove hardcode
            }
        }

        private PieceStack CreateRandomStack()
        {
            PieceStack stack = Instantiate(pieceStackPrefab);
        
            if (stack == null)
            {
                Debug.LogError("PieceStack component not found on prefab!");
                return null;
            }
        
            stack.Initialize();
            stack.ArrangePieces();
        
            return stack;
        }

        public void TryPlaceStack(HexCell cell, PieceStack stack)
        {
            if (stack == null || IsProcessingMatches) return;
            if (cell.IsOccupied) return;
        
            cell.SetStack(stack);
            currentTurnStacks.Remove(stack);
        
            StartCoroutine(ProcessMatchesRecursive(cell));
        
            if (AllStacksPlaced())
            {
                Invoke(nameof(GenerateNewTurn), 0.5f);
            }
        }

        private bool AllStacksPlaced()
        {
            foreach (var stack in currentTurnStacks)
            {
                if (stack != null && !stack.IsPlaced)
                {
                    return false;
                }
            }
        
            return true;
        }

        private IEnumerator ProcessMatchesRecursive(HexCell startCell)
        {
            IsProcessingMatches = true;
        
            // Procesar matches recursivos desde esta celda
            yield return StartCoroutine(ProcessCellMatchesRecursively(startCell));
        
            IsProcessingMatches = false;
            CheckGameOver();
        }

        private IEnumerator ProcessCellMatchesRecursively(HexCell cell)
        {
            if (!cell.IsOccupied) yield break;
        
            bool foundMatch = true;
        
            // Repetir mientras haya matches
            while (foundMatch && cell.IsOccupied)
            {
                foundMatch = false;
            
                List<HexCell> neighbors = hexGrid.GetNeighbours(cell);
                PieceColor currentTopColor = cell.Stack.GetTopColor();
            
                // Buscar el primer vecino que hace match
                foreach (var neighbor in neighbors)
                {
                    if (!neighbor.IsOccupied) continue;
                
                    if (neighbor.Stack.GetTopColor() == currentTopColor)
                    {
                        // Match encontrado! Mover piezas del mismo color
                        List<Piece> piecesToMove = cell.Stack.RemovePiecesOfColor(currentTopColor);
                    
                        foreach (var piece in piecesToMove)
                        {
                            neighbor.Stack.AddPiece(piece);
                        }
                    
                        neighbor.Stack.ArrangePieces();
                    
                        yield return new WaitForSeconds(0.2f);
                    
                        // Si la celda origen quedó vacía, destruir el stack
                        if (cell.Stack.PieceCount == 0)
                        {
                            Destroy(cell.Stack.gameObject);
                            cell.ClearStack();
                        }
                        else
                        {
                            cell.Stack.ArrangePieces();
                        }
                    
                        // Verificar si el stack vecino llegó a 10 piezas
                        if (neighbor.IsOccupied && neighbor.Stack.PieceCount >= piecesToDestroy)
                        {
                            int points = neighbor.Stack.PieceCount;
                            score += points;
                            UpdateScoreUI();
                        
                            Destroy(neighbor.Stack.gameObject);
                            neighbor.ClearStack();
                        
                            yield return new WaitForSeconds(0.3f);
                        }
                        else if (neighbor.IsOccupied)
                        {
                            // Procesar recursivamente el vecino que recibió las piezas
                            yield return StartCoroutine(ProcessCellMatchesRecursively(neighbor));
                        }
                    
                        // Si la celda actual aún tiene piezas, puede haber más matches
                        if (cell.IsOccupied)
                        {
                            foundMatch = true;
                            break; // Salir del foreach y reintentar desde el principio
                        }
                        else
                        {
                            yield break; // La celda quedó vacía, terminar
                        }
                    }
                }
            
                // Verificar si el stack actual llegó a 10 después de perder piezas
                if (cell.IsOccupied && cell.Stack.PieceCount >= piecesToDestroy)
                {
                    int points = cell.Stack.PieceCount;
                    score += points;
                    UpdateScoreUI();
                
                    Destroy(cell.Stack.gameObject);
                    cell.ClearStack();
                
                    yield return new WaitForSeconds(0.3f);
                    yield break;
                }
            }
        }

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

            hudController.ShowPauseOverlay();
        }

        private void Resume()
        {
            IsPaused = false;
            IsInputBlocked = false;

            hudController.HidePauseOverlay();
        }

        public void TogglePauseFromOverlay()
        {
            Resume();
        }
        #endregion

        private void OnGameOver()
        {
            IsInputBlocked = true;
            hudController.ShowGameOverOverlay(score);
        }
        
        private void UpdateScoreUI()
        {
            hudController.UpdateScore(score);
        }
    }
}
