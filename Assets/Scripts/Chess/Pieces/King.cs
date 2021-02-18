using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;

namespace ChessGame.Pieces{
    public class King : Piece
    {
        private Rook leftRook = null;
        private Rook rightRook = null;

        public override void Setup(Side side)
        {
            base.Setup(side);
            movement = new Vector3Int(1, 1, 1);
        }

        public override void Kill()
        {
            board.KillTheKing(this);
        }

        protected override void CheckPathing()
        {
            base.CheckPathing();

            rightRook = GetRook(1, 3);

            leftRook = GetRook(-1, 4);
        }

        [PunRPC]
        protected override void Move()
        {
            // Base move
            base.Move();

            // Left rook
            if (CanCastle(leftRook)){
                leftRook.Castle();
            }

            // Right rook
            if (CanCastle(rightRook)){
                rightRook.Castle();
            }
        }

        private bool CanCastle(Rook rook)
        {
            
            if (rook == null)
            {
                return false;
            }

            // Do the cells match?
            if (rook.castleTriggerCell != currentCell)
            {
                return false;
            }

            // Check if same team, and hasn't moved
            if (rook.Side != this.Side || !rook.IsFirstMove)
            {
                return false;
            }

            return true;
        }

        private Rook GetRook(int direction, int count)
        {
            // Has the king moved?
            if (!this.IsFirstMove)
            {
                return null;
            }

           
            int currentX = currentCell.CellPosition.x;
            int currentY = currentCell.CellPosition.y;

            // Go through the cells
            for (int i = 1; i < count; i++)
            {
                int offsetX = currentX + (i * direction);
                CellState cellState = DetermineTargetCellState(currentX,currentY);

                if (cellState != CellState.FREE)
                {
                    return null;
                }
            }

            // Try and get rook
            Cell rookCell = board.BoardCells[currentX + (count * direction), currentY];
            Rook rook = null;

            // Check for cast
            if (rookCell.GetPiece() is Rook)
            {
                rook = (Rook)rookCell.GetPiece();
            }

            
            if (rook != null)
            {
                highlightedCells.Add(rook.castleTriggerCell);
            }

            return rook;
        }
    }
}
