using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.Pieces{
    public class Rook : Piece
    {
        public Cell castleTriggerCell = null;
        private Cell castleCell = null;

        public override void Setup(Side side)
        {
            base.Setup(side);
            movement = new Vector3Int(7, 7, 0);
        }


         public void Place(Cell newCell)
        {
           

            // Trigger cell
            int triggerOffset = currentCell.CellPosition.x < 4 ? 2 : -1;
            castleTriggerCell = SetCell(triggerOffset);

            // Castle cell
            int castleOffset = currentCell.CellPosition.x < 4 ? 3 : -2;
            castleCell = SetCell(castleOffset);
        }

        public void Castle()
        {
            // Set new target
            targetCell = castleCell;

            // Actually move
            Move();
        }

        private Cell SetCell(int offset)
        {
            // New position
            Vector2Int newPosition = currentCell.CellPosition;
            newPosition.x += offset;

            // Return
            return board.BoardCells[newPosition.x, newPosition.y];
        }
    }
}