using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessGame.Pieces;

namespace ChessGame{
    public class Cell : MonoBehaviour
    {
        private Vector2Int cellCoordinates;
        private Piece currentPiece;
        public RectTransform rectTransform;
        private SpriteRenderer spriteRenderer;

        public Vector2Int CellPosition {get {return cellCoordinates;} set {cellCoordinates = value;}}


        private void Awake() 
        {
            cellCoordinates = Vector2Int.zero;
            currentPiece = null;
            rectTransform = null;

            spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
            rectTransform = this.gameObject.GetComponent<RectTransform>();
        }

        public void Setup(Vector2Int newBoardPosition)
        {
            cellCoordinates = newBoardPosition;
        }

        public void PlacePiece(Piece piece){
            currentPiece = piece;
            currentPiece.CurrentCell = this;
        }

        public void RemovePiece()
        {
            if (currentPiece != null)
            {
                currentPiece = null;
            }
        }

        public void KillPiece(){
            if(currentPiece != null){
                currentPiece.Kill();
                currentPiece = null;
               
            }
        }

        public bool IsHasPiece(){
            return currentPiece == null;
        }
        public Piece GetPiece(){
            return currentPiece;
        }

        public void SwitchToAttackSprite(){
            spriteRenderer.color = new Color(255,0,0,1);
        }

        public void SwithToMovementSprite(){
            spriteRenderer.color = new Color(0,0,0,1);
        }

    }
}
