using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.Pieces{
    public class Pawn : Piece
    {
        public override void Setup(Side side)
        {
            base.Setup(side);
            movement = new Vector3Int(0, 1, 1);
        }

        protected override void Move()
        {
            base.Move();

            CheckForPromotion();
        }

        private bool MatchesState(int targetX, int targetY, CellState targetState)
        {
            CellState cellState = DetermineTargetCellState(targetX,targetY);
            if (cellState == targetState)
            {
                Cell cell = board.BoardCells[targetX, targetY];
                if (cellState == CellState.ENEMY)
                {
                    cell.SwitchToAttackSprite();
                }
                highlightedCells.Add(cell);
                return true;
            }

            return false;
        }

        private void CheckForPromotion()
        {
            
            int currentX = currentCell.CellPosition.x;
            int currentY = currentCell.CellPosition.y;

            // Check if pawn has reached the end of the board
            CellState cellState = DetermineTargetCellState(currentX, currentY + movement.y);

            if (cellState == CellState.OUT_OF_BOUNDS)
            {
             board.PromotePiece(this);
            }
        }

        

        protected override void CheckPathing()
        {
            // Target position
            Debug.Log("Check pathing + !" + CurrentCell.CellPosition + " " );

            int currentX = CurrentCell.CellPosition.x;
            int currentY = CurrentCell.CellPosition.y;

            // Top left
            MatchesState(currentX - movement.z, currentY + movement.z, CellState.ENEMY);

            // Forward
            if (MatchesState(currentX, currentY + movement.y, CellState.FREE))
            {
                // If the first forward cell is free, and first move, check for next
                if (this.IsFirstMove)
                {
                    MatchesState(currentX, currentY + (movement.y * 2), CellState.FREE);
                }
            }

            // Top right
            MatchesState(currentX + movement.z, currentY + movement.z, CellState.ENEMY);
        }

    }
}
