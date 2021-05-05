﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BoardManager : MonoBehaviour, ICommonBoard {
	public static event Action<int> OnBoardPositionSet;

	[SerializeField] private List<Sprite> _charactersSprites;
	[SerializeField] private GameObject _tilePrefab;
	[SerializeField] private int _xSize, _ySize;

	public Tile[,] _tiles;

	private IEnumerator _currentCoroutine;

	public static bool IsShifting { get; private set; }

	void Start () {
		Tile.OnMatchFound += ClearAndRefillBoard;

		Vector2 offset = _tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
		SetBoardPosition(offset);

        CreateBoard(offset.x, offset.y);
	}

	public void CreateBoard (float xOffset, float yOffset) {
		_tiles = new Tile[_xSize, _ySize];

        float startX = transform.position.x;
		float startY = transform.position.y;

		for (byte x = 0; x < _xSize; x++) {
			for (byte y = 0; y < _ySize; y++) {
				var tileObject = Instantiate(_tilePrefab, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), _tilePrefab.transform.rotation, transform);
				var tile = tileObject.GetComponent<Tile>();

				var possibleCharacters = new List<Sprite>();
				possibleCharacters.AddRange(_charactersSprites);

				tile.ImageSource = GetCurrentTileImage(x, y, ref possibleCharacters);
				_tiles[x, y] = tile;
			}
        }
    }

	public Sprite GetCurrentTileImage(byte xIndex, byte yIndex, ref List<Sprite> possibleCharacters)
    {
		if (xIndex != 0)
		{
			possibleCharacters.Remove(_tiles[xIndex - 1, yIndex].ImageSource);
		}

		if (yIndex != 0)
		{
			possibleCharacters.Remove(_tiles[xIndex, yIndex - 1].ImageSource);
		}

		return possibleCharacters[UnityEngine.Random.Range(0, possibleCharacters.Count)];
	}

	public void ClearAndRefillBoard()
    {
		if(_currentCoroutine != null)
        {
			StopCoroutine(_currentCoroutine);
        }

		_currentCoroutine = FindNullTiles();
		StartCoroutine(_currentCoroutine);
    }

	private void SetBoardPosition(Vector2 tileOffset)
    {
		var xOffset = (_xSize * tileOffset.x - 1) / 2f;
		var yOffset = (_ySize * tileOffset.y - 1) / 2f;

		var holderOffset = new Vector2(xOffset, yOffset);
		holderOffset *= -1f;
		transform.position = holderOffset;

		OnBoardPositionSet?.Invoke(_xSize + 1);
	}

	private IEnumerator FindNullTiles()
	{
		for (var x = 0; x < _xSize; x++)
		{
			for (var y = 0; y < _ySize; y++)
			{
				if (_tiles[x, y].ImageSource == null)
				{
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}
	}

	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
	{
		IsShifting = true;
		var sprites = new List<Sprite>();
		var nullCount = 0;

		for (var y = yStart; y < _ySize; y++)
		{
			var sprite = _tiles[x, y].ImageSource;
			if (sprite == null)
			{
				nullCount++;
			}
			sprites.Add(sprite);
		}

		for (var i = 0; i < nullCount; i++)
		{
			yield return new WaitForSeconds(shiftDelay);

			for (var k = 0; k < sprites.Count - 1; k++)
			{
				sprites[k] = sprites[k + 1];
				sprites[k + 1] = null;
			}
		}

		IsShifting = false;
	}
}