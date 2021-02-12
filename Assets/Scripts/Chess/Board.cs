using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessGame;
using ChessGame.Pieces;
using Photon.Pun;

namespace ChessGame{

    public class Board : MonoBehaviour
    {
        public GameObject MainCamera;

        private static Board Instance = null;
        
        private bool isWhite = true;

        public GameObject piecePrefab;

        public GameObject highlightedCellPrefab;

        private Sprite[] piecesSprites;

        private Cell[,] boardCells = new Cell[8, 8];

        private string[] pieceOrder = new string[16]{
        "P","P","P","P","P","P","P","P",
        "R","KN","B","Q","K","B","KN","R"
        };

        private Dictionary<string, Type> piecesDictionary = new Dictionary<string, Type>(){
            {"P", typeof(Pawn)},
            {"R", typeof(Rook)},
            {"KN", typeof(Knight)},
            {"B", typeof(Bishop)},
            {"Q", typeof(Queen)},
            {"K", typeof(King)}
        };

        private List<Piece> blackPieces = new List<Piece>();
        private List<Piece> whitePieces = new List<Piece>();

        public Cell[,] BoardCells { get { return boardCells;}}

        private void Awake() {
            Instance = this;
            piecesSprites = Resources.LoadAll<Sprite>("Pieces");    
        }
        
        void Start()
        {
            FillPiecesLists();
            CreateAndPlaceCells();
            PlacePieces(1,0, whitePieces);
            PlacePieces(6,7, blackPieces);

            if(!PhotonNetwork.IsMasterClient)
            {
                ChangeViewForConnectedPlayer();
            }
        }

        internal void PromotePiece(Pawn pawn)
        {

            //my pieces delete pawn and add queen
            Piece promotedPiece = CreatePiece(typeof(Queen),pawn.Side);
            
            pawn.CurrentCell.PlacePiece(promotedPiece);
            promotedPiece.CurrentCell = pawn.CurrentCell;

            promotedPiece.transform.position = pawn.transform.position;
             
            pawn.Kill();
        }

        private void ChangeViewForConnectedPlayer()
        {
            GameObject.FindWithTag("Board").transform.Rotate(0,0,-180);

            foreach(GameObject piece in GameObject.FindGameObjectsWithTag("Piece"))
            {
                piece.transform.Rotate(0,0,-180);
            }
        }

        private void CreateAndPlaceCells(){
            for (int column = 0; column < 8; column++)
            {
                for (int row = 0; row < 8; row++)
                {
                    GameObject newCellObject = Instantiate(highlightedCellPrefab);
                    newCellObject.name = $"Row {row}, Column {column}";
                    newCellObject.transform.parent = this.transform;
                    newCellObject.transform.position = new Vector3(AlignCoordinate((float)column), AlignCoordinate((float)row), -1.0f);
                   
                    Cell createdCell = (Cell)newCellObject.AddComponent(typeof(Cell));
                    createdCell.CellPosition = new Vector2Int(column,row);
                    boardCells[column,row] = createdCell;
                    createdCell.gameObject.SetActive(false);
                }
                
            }
        }

        private void FillPiecesLists(){
            for(int i = 0; i < pieceOrder.Length; i++)
            {
                Type pieceType = piecesDictionary[pieceOrder[i]];
                whitePieces.Add(CreatePiece(pieceType,Side.WHITE));
                blackPieces.Add(CreatePiece(pieceType,Side.BLACK));
            }
            if(isWhite)
            { 
                blackPieces.Swap(11,12);
            }else
            {
                whitePieces.Swap(11,12);
            }
        }

        private Sprite FindPieceSprite(string spriteName)
        {
            foreach(Sprite sprite in piecesSprites)
            {
                if(sprite.name.Equals(spriteName))
                {
                    return sprite;
                }
            }
            return null;
        }

        private Piece CreatePiece(Type pieceType, Side side)
        {
            GameObject newPieceObject = Instantiate(piecePrefab);
            newPieceObject.transform.parent = this.transform;

            Piece createdPiece = (Piece)newPieceObject.AddComponent(pieceType);
            Sprite pieceSprite = FindPieceSprite(side.ToString().ToLower() + pieceType.Name);

            newPieceObject.GetComponent<SpriteRenderer>().sprite = pieceSprite;
            newPieceObject.name = side.ToString().ToLower() + pieceType.Name;

            createdPiece.Setup(side);
            
            return createdPiece;
        }

        private void PlacePieces(int pawnRow, int royaltyRow, List<Piece> pieces)
        {
            for(int i =0; i < 8 ; i++)
            {
            
                pieces[i].CurrentCell = boardCells[i,pawnRow];
                pieces[i+8].CurrentCell = boardCells[i,royaltyRow]; 

                boardCells[i,pawnRow].PlacePiece(pieces[i]);
                boardCells[i,royaltyRow].PlacePiece(pieces[i+8]);
                
                pieces[i].transform.position = new Vector3(AlignCoordinate((float)i), AlignCoordinate((float)pawnRow), -1.0f);
                pieces[i+8].transform.position = new Vector3(AlignCoordinate((float)i), AlignCoordinate((float)royaltyRow), -1.0f);
            }
        }

        /// <summary>
        /// The method fits the coordinate to the coordinate of one cell of the board according to the size of the board sprite.
        /// </summary>
        /// <param name="coordinate">X or Y coordinate value.</param>
        private float AlignCoordinate(float coordinate){
            coordinate = coordinate * 0.42f - 1.47f;
            return coordinate;
        }

        public void KillTheKing(King king, Side side){
            //TODO
        }
    }
}
