using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.Pieces{
    public class Knight : Piece
    {
        private void CreateCellPath(int flipper)
        {
            // Target position
            int currentX = currentCell.CellPosition.x;
            int currentY = currentCell.CellPosition.y;

            // Left
            MatchesState(currentX - 2, currentY + (1 * flipper));

            // Upper left
            MatchesState(currentX - 1, currentY + (2 * flipper));

            // Upper right
            MatchesState(currentX + 1, currentY + (2 * flipper));

            // Right
            MatchesState(currentX + 2, currentY + (1 * flipper));
        }

        
        protected override void CheckPathing()
        {
            // Draw top half
            CreateCellPath(1);

            // Draw bottom half
            CreateCellPath(-1);
        }

        
        private void MatchesState(int targetX, int targetY)
        {
            CellState cellState = DetermineTargetCellState(targetX,targetY);

            if (cellState != CellState.OUT_OF_BOUNDS && cellState != CellState.ALLY)
            {
                Cell cell = board.BoardCells[targetX, targetY];

                if (cellState == CellState.ENEMY)
                {
                    cell.SwitchToAttackSprite();
                }
                
                highlightedCells.Add(cell);
            }
            
        }
    }
}
