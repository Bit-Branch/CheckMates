using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChessGame.Pieces{
    public class Queen : Piece
    {
        public override void Setup(Side side)
        {
            base.Setup(side);
            movement = new Vector3Int(7, 7, 7);
        }
    }
}
