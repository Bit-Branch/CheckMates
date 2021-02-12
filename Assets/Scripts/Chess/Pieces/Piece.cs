using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;


namespace ChessGame.Pieces
{
    public abstract class Piece : MonoBehaviour
    {
        private Side side;
        protected Cell originalCell;
        protected Cell currentCell;
        protected RectTransform rectTransform;
        protected Cell targetCell;
        private Vector3 screenPoint;
        private Vector3 offset;
        private bool isFirstMove;
        protected Vector3Int movement;
        protected List<Cell> highlightedCells;
        protected Board board;

        public Cell CurrentCell {get{return currentCell;} set{currentCell = value;}}
        public Cell OriginalCell {get{return originalCell;}}

        public Side Side {get{return side;} set{side = value;}}

        public bool IsFirstMove {get{return isFirstMove;}}

        private void Awake() 
        {
            originalCell = null;
            currentCell = null;
            rectTransform = null;
            targetCell = null;
            isFirstMove = true;
            movement = Vector3Int.one;
            highlightedCells = new List<Cell>();
            board = GameObject.FindWithTag("Board").GetComponent<Board>();
        }
        
        public virtual void Setup(Side side)
        {
            this.side = side;
        }

        public virtual void Kill()
        {
            this.gameObject.SetActive(false);
        } 

        protected CellState DetermineTargetCellState(int targetX, int targetY)
        {
            // Bounds check
            if (targetX < 0 || targetX > 7)
                return CellState.OUT_OF_BOUNDS;

            if (targetY < 0 || targetY > 7)
                return CellState.OUT_OF_BOUNDS;

            Debug.Log("Determine " + board);

            Cell targetCell = this.board.BoardCells[targetX, targetY];

            if (!targetCell.IsHasPiece())
            {
                if (targetCell.GetPiece().Side != this.side){
                    return CellState.ENEMY;
                }
                if (targetCell.GetPiece().Side == this.side){
                    return CellState.ALLY;
                }
            }

            return CellState.FREE;
        }

        protected void CreateCellPath(int xDirection, int yDirection, int movement)
        {
            
            int currentX = currentCell.CellPosition.x;
            int currentY = currentCell.CellPosition.y;

            // Check each cell
            for (int i = 1; i <= movement; i++)
            {
                currentX += xDirection;
                currentY += yDirection;

                CellState targetCellState = DetermineTargetCellState(currentX,currentY);

                Debug.Log("Target cell state^" + targetCellState);
                
                if (targetCellState != CellState.OUT_OF_BOUNDS)
                {
                    if (targetCellState == CellState.ENEMY)
                    {
                        Cell cell = board.BoardCells[currentX, currentY];
                        cell.SwitchToAttackSprite();
                        highlightedCells.Add(cell);
                        break;
                    }

                    if(targetCellState == CellState.ALLY)
                    {
                        break;
                    }

                    highlightedCells.Add(board.BoardCells[currentX, currentY]);
                }
            }
        }

        protected void ShowCells()
        {
            foreach (Cell cell in highlightedCells)
            {
                cell.gameObject.SetActive(true);
            }
        }

        protected void ClearCells()
        {
            foreach (Cell cell in highlightedCells)
            {
                cell.SwithToMovementSprite();
                cell.gameObject.SetActive(false);
            }

            highlightedCells.Clear();
        }

        protected virtual void CheckPathing()
        {
            // Horizontal
            CreateCellPath(1, 0, movement.x);
            CreateCellPath(-1, 0, movement.x);

            // Vertical 
            CreateCellPath(0, 1, movement.y);
            CreateCellPath(0, -1, movement.y);

            // Upper diagonal
            CreateCellPath(1, 1, movement.z);
            CreateCellPath(-1, 1, movement.z);

            // Lower diagonal
            CreateCellPath(-1, -1, movement.z);
            CreateCellPath(1, -1, movement.z);
        }

    protected virtual void Move()
        {
            
            isFirstMove = false;

            // If there is an enemy piece, remove it
            targetCell.KillPiece();

            // Clear current
            currentCell.RemovePiece();

            // Switch cells
            currentCell = targetCell;
            currentCell.PlacePiece(this);

            // Move on board
            transform.position = currentCell.rectTransform.position;
            targetCell = null;
        }
    
        public void OnMouseDown()
        {
            CheckPathing();
            ShowCells();
        }

        public void OnMouseDrag()
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 8);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            transform.position = curPosition;

            foreach (Cell cell in highlightedCells)
            {
                // Does the cursor with the shape fall within the selected cell?
                if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition, Camera.main))
                {
                    // If the mouse is within a valid cell, get it, and break.
                    targetCell = cell;
                    break;
                }

                // If the mouse is not within any highlighted cell, we don't have a valid move.
                targetCell = null;
            }
        }

        public void OnMouseUp()
        {
            ClearCells();

            // Return to original position
            if (!targetCell)
            {
                transform.position = CurrentCell.rectTransform.position;
                return;
            }

            // Else move to new cell
            Move();
         
        }
         
    }
}
