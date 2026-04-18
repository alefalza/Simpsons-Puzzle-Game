using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameModes.Core;
using GameModes.DonutStack.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModes.DonutStack.Gameplay
{
    public class DonutStackGameManager : BaseGameManager<DonutStackGameManager>
    {
        [Header("Grid Settings")]
        [SerializeField] private DonutGrid donutGrid;
    
        [Header("Piece Settings")]
        [SerializeField] private Core.DonutStack stackPrefab;
        [SerializeField] private DonutStackItemData itemData;
        
        [Header("Game Settings")]
        [SerializeField] private Transform dragLayer;
        [SerializeField] private StackSlot[] stackSlots;

        protected override string GameModeName => "DonutStack";

        private readonly List<WeightedType> weightedLevelTypes = new List<WeightedType>();
        
        // Properties that get values from levelData or default values
        private int TargetScore => levelData != null ? ((DonutStackLevelDefinition)levelData).targetScore : 10;
        private int GridRadius => levelData != null ? ((DonutStackLevelDefinition)levelData).gridRadius : 1;
        private int StacksPerTurn => levelData != null ? ((DonutStackLevelDefinition)levelData).stacksPerTurn : 3;
        private int PiecesToDestroy => levelData != null ? ((DonutStackLevelDefinition)levelData).piecesToDestroy : 10;
        private float MatchProcessDelay => levelData != null ? ((DonutStackLevelDefinition)levelData).matchProcessDelay : GameConstants.DonutStack.MatchProcessDelay;
        private float PieceRemoveDelay => levelData != null ? ((DonutStackLevelDefinition)levelData).pieceRemoveDelay : GameConstants.DonutStack.PieceRemoveDelay;
        private float PostDestroyDelay => levelData != null ? ((DonutStackLevelDefinition)levelData).postDestroyDelay : GameConstants.DonutStack.PostDestroyDelay;
        private float NewTurnDelay => levelData != null ? ((DonutStackLevelDefinition)levelData).newTurnDelay : GameConstants.DonutStack.NewTurnDelay;
        private DonutStackLevelDefinition DonutStackLevelDefinition => levelData as DonutStackLevelDefinition;
        
        private readonly List<Core.DonutStack> currentTurnStacks = new List<Core.DonutStack>();
        
        private int score = 0;
        
        public Transform DragLayer => dragLayer;
        public bool IsProcessingMatches { get; private set; } = false;

        protected override void Start()
        {
            base.Start();
            donutGrid.Initialize(GridRadius);
            hudController.SetLevelText(currentLevelNumber);
            ConfigureLevelTypeWeights();
            UpdateScoreUI();
            GenerateNewTurn();
        }
        
        private void ConfigureLevelTypeWeights()
        {
            weightedLevelTypes.Clear();

            if (itemData == null)
            {
                return;
            }

            List<DonutColor> availableTypes = itemData.GetAvailableColors();
            if (availableTypes.Count == 0)
            {
                return;
            }

            int positiveWeightCount = 0;

            foreach (DonutColor color in availableTypes)
            {
                int weight = DonutStackLevelDefinition != null ? DonutStackLevelDefinition.GetSpawnWeight(color) : 1;
                if (weight > 0)
                {
                    positiveWeightCount++;
                }

                weightedLevelTypes.Add(new WeightedType(color, weight));
            }

            // Fallback: if all configured weights are 0, use uniform weight 1.
            if (positiveWeightCount == 0)
            {
                weightedLevelTypes.Clear();
                foreach (DonutColor type in availableTypes)
                {
                    weightedLevelTypes.Add(new WeightedType(type, 1));
                }
            }
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

            for (int i = 0; i < StacksPerTurn; i++)
            {
                Core.DonutStack stack = CreateRandomStack();
                currentTurnStacks.Add(stack);
            }
        }

        private Core.DonutStack CreateRandomStack()
        {
            StackSlot parentSlot = stackSlots.First(x => !x.IsOccupied);
            Core.DonutStack stack = Instantiate(stackPrefab, parentSlot.transform);
        
            if (stack == null)
            {
                Debug.LogError("PieceStack component not found on prefab!");
                return null;
            }
            
            stack.Initialize(parentSlot, GetSpriteForColor, GetRandomWeightedAvailableColor);
        
            return stack;
        }

        private Sprite GetSpriteForColor(DonutColor color)
        {
            return itemData.GetSpriteForColor(color);
        }

        private DonutColor GetRandomWeightedAvailableColor()
        {
            if (weightedLevelTypes.Count == 0)
            {
                return itemData != null ? itemData.GetRandomAvailableColor() : DonutColor.None;
            }

            int totalWeight = 0;
            foreach (WeightedType weightedType in weightedLevelTypes)
            {
                totalWeight += Mathf.Max(0, weightedType.Weight);
            }

            if (totalWeight <= 0)
            {
                return itemData != null ? itemData.GetRandomAvailableColor() : DonutColor.None;
            }

            int randomValue = Random.Range(0, totalWeight);
            foreach (WeightedType weightedType in weightedLevelTypes)
            {
                int weight = Mathf.Max(0, weightedType.Weight);
                if (weight == 0 || weightedType.Color == DonutColor.None) continue;

                if (randomValue < weight)
                {
                    return weightedType.Color;
                }

                randomValue -= weight;
            }

            return weightedLevelTypes[weightedLevelTypes.Count - 1].Color;
        }

        public void TryPlaceStack(GridCell cell, Core.DonutStack stack)
        {
            if (stack == null || IsProcessingMatches) return;
            if (cell.IsOccupied) return;
        
            cell.SetStack(stack);
            stack.PlaceOnCell(cell);
            currentTurnStacks.Remove(stack);
        
            StartCoroutine(ProcessMatchesRecursive(cell));
        
            if (AllStacksPlaced())
            {
                Invoke(nameof(GenerateNewTurn), NewTurnDelay);
            }
        }

        private bool AllStacksPlaced()
        {
            return currentTurnStacks.All(stack => stack == null || stack.IsPlaced);
        }

        private IEnumerator ProcessMatchesRecursive(GridCell startCell)
        {
            IsProcessingMatches = true;
        
            yield return StartCoroutine(ProcessCellMatchesRecursively(startCell));
        
            IsProcessingMatches = false;
            CheckLoseCondition();
        }

        private IEnumerator ProcessCellMatchesRecursively(GridCell cell)
        {
            if (!cell.IsOccupied) yield break;

            bool foundMatch = true;

            while (foundMatch && cell.IsOccupied)
            {
                foundMatch = false;

                List<GridCell> neighbours = donutGrid.GetNeighbours(cell);
                DonutColor currentTopColor = cell.Stack.GetTopColor();

                foreach (var neighbour in neighbours)
                {
                    if (!neighbour.IsOccupied) continue;

                    // If colors at the top match, move pieces from one stack to the other.
                    if (neighbour.Stack.GetTopColor() == currentTopColor)
                    {
                        List<Donut> piecesToMove = cell.Stack.RemovePiecesOfColor(currentTopColor);

                        foreach (var piece in piecesToMove)
                            neighbour.Stack.AddPiece(piece);

                        neighbour.Stack.ArrangePieces();

                        yield return new WaitForSeconds(MatchProcessDelay);

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
                        if (neighbour.IsOccupied && topColorCount >= PiecesToDestroy)
                        {
                            yield return StartCoroutine(
                                neighbour.Stack.RemoveTopGroupWithDelay(PiecesToDestroy, PieceRemoveDelay)
                            );
                            
                            score += topColorCount;
                            UpdateScoreUI();
                            CheckWinCondition();

                            // If the stack ends up empty after destroying its top pieces, destroy it.
                            if (neighbour.Stack.PieceCount == 0)
                            {
                                Destroy(neighbour.Stack.gameObject);
                                neighbour.ClearStack();
                                yield break;
                            }

                            yield return new WaitForSeconds(PostDestroyDelay);
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

        private readonly struct WeightedType
        {
            public WeightedType(DonutColor color, int weight)
            {
                Color = color;
                Weight = weight;
            }

            public DonutColor Color { get; }
            public int Weight { get; }
        }
        
        private void CheckWinCondition()
        {
            if (hasLost) return;
            
            if (score >= TargetScore)
            {
                OnGameWon(score);
            }
        }

        protected override void OnGameWon(int finalScore = 0)
        {
            IsInputBlocked = true;
            
            MarkLevelAsCompleted();
            
            base.OnGameWon(finalScore);
        }

        private void CheckLoseCondition()
        {
            if (hasWon) return;
            
            if (!donutGrid.HasEmptyCells() && currentTurnStacks.Count > 0)
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
                    OnGameLost(score);
                }
            }
        }
        
        protected override void OnGameLost(int finalScore = 0)
        {
            IsInputBlocked = true;
            
            base.OnGameLost(finalScore);
        }
    }
}
