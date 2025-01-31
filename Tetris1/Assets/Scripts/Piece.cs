using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Default position not valid? Then it's game over
        if (!IsValidBoard())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame.
    // Implements all piece movements: right, left, rotate and down.
    void Update()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if it's valid
            if (IsValidBoard())
                // It's valid. Update grid.
                UpdateBoard();
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if it's valid
            if (IsValidBoard())
                // It's valid. Update grid.
                UpdateBoard();
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
        }

        // Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Rotate the piece
            transform.Rotate(0, 0, -90);

            // See if it's valid
            if (IsValidBoard())
                // It's valid. Update grid.
                UpdateBoard();
            else
                // It's not valid. revert rotation.
                transform.Rotate(0, 0, 90);
        }

        // Move Downwards and Fall
        if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time > nextFall)
        {
            // Modify position
            transform.position += new Vector3(0, -1, 0);

            // See if it's valid
            if (IsValidBoard())
            {
                // It's valid. Update grid.
                UpdateBoard();
            }
            else
            {
                // It's not valid. Revert.
                transform.position += new Vector3(0, 1, 0);

                // Add the piece to the board and spawn next
                LockPiece();
                Object.FindFirstObjectByType<Spawner>().SpawnNext();

            }

            // Reset fall timer
            nextFall = Time.time + fallInterval;
        }
    }

    // Updates the board with the current position of the piece.
    void UpdateBoard()
    {
        // First you have to loop over the Board and make current positions of the piece null.
        for (int y = 0; y < Board.h; y++)
        {
            for (int x = 0; x < Board.w; x++)
            {
                if (Board.grid[x, y] != null && Board.grid[x, y].transform.parent == transform)
                {
                    Board.grid[x, y] = null;
                }
            }
        }

        // Then you have to loop over the blocks of the current piece and add them to the Board.
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);
            Board.grid[(int)v.x, (int)v.y] = child.gameObject;
        }
    }

    // Returns if the current position of the piece makes the board valid or not
    bool IsValidBoard()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);

            // Not inside Border?
            if (!Board.InsideBorder(v))
                return false;

            // Block in grid cell (and not part of same group)?
            if (Board.grid[(int)v.x, (int)v.y] != null &&
                Board.grid[(int)v.x, (int)v.y].transform.parent != transform)
                return false;
        }
        return true;
    }

    // Locks the piece in place and updates the board.
    void LockPiece()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Board.RoundVector2(child.position);
            Board.grid[(int)v.x, (int)v.y] = child.gameObject;
        }

        // Check for full rows and delete them
        Board.DeleteFullRows();

        // Disable the piece once locked
        enabled = false;
    }

    // Timer for falling pieces
    private float nextFall = 0f;
    private float fallInterval = 1f;
}