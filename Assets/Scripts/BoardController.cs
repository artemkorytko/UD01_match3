using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    [SerializeField] private float elementOffset = 1f;
    [SerializeField] private float sizeX = 1f;
    [SerializeField] private float sizeY = 1f;

    [SerializeField] private ElementsConfig _config;
    [SerializeField] private Elements prefab;
    public Elements[,] Elements;

    public const int rawCount = 20;
    public const int columnCount = 50;

    private int _raws;
    private int _columns;

    private void Start()
    {
        CreateField(rawCount, columnCount);
    }

    public void CreateField(int raw, int colum)
    {
        if (Elements != null)
        {
            int rows = Elements.GetLength(0); // количество строк
            int columns = Elements.GetLength(1); // количество столбцов
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    Destroy(Elements[x, y].gameObject);
                }
            }
        }

        _raws = raw;
        _columns = colum;
        Elements = new Elements[raw, colum];
        for (int x = 0; x < raw; x++)
        {
            for (int y = 0; y < colum; y++)
            {
                Elements[x, y] = Instantiate(prefab, transform);
            }
        }
    }

    public void GenerateElements()
    {
        Vector2 startPosition = new Vector2(-elementOffset * _columns * 0.5f + elementOffset * 0.5f,
            elementOffset * _raws * 0.5f - elementOffset * 0.5f);
        for (int x = 0; x < _raws; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                Vector2 position = startPosition + new Vector2(elementOffset * y, -elementOffset * x);
                List<Element> elements = GetPossibleElements(x, y, _raws, _columns);
                Elements[x, y].Initialize(elements[Random.Range(0, elements.Count)], new Vector2(x, y), position);
                Elements[x, y].OnClicked += OnElementClicked;
            }
        }
    }

    private List<Element> GetPossibleElements(int row, int column, int raws, int columns)
    {
        var list = _config.Elements.ToList();

        int x = row;
        int y = column - 1;

        if (x >= 0 && x < raws && y >= 0 && y < columns)
        {
            if (Elements[x, y].IsInitialized)
            {
                list.Remove(_config.GetByKey(Elements[x, y].Key));
            }
        }

        x = row - 1;
        y = column;
        if (x >= 0 && x < raws && y >= 0 && y < columns)
        {
            if (Elements[x, y].IsInitialized)
            {
                list.Remove(_config.GetByKey(Elements[x, y].Key));
            }
        }

        return list;
    }

    public List<Elements> SearchLines()
    {
        List<Elements> elementsForCollect = new List<Elements>();
        for (int x = 0; x < _raws; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                if (Elements[x, y].IsActive && !elementsForCollect.Contains(Elements[x, y]))
                {
                    var horizontalElements = CheckHorizontal(x, y);
                    bool needAddThisElement = false;

                    if (horizontalElements != null && horizontalElements.Count >= 2)
                    {
                        needAddThisElement = true;
                        elementsForCollect.AddRange(horizontalElements);
                    }

                    var verticalElements = CheckVertical(x, y);
                    if (verticalElements != null && verticalElements.Count >= 2)
                    {
                        needAddThisElement = true;
                        elementsForCollect.AddRange(verticalElements);
                    }

                    if (needAddThisElement)
                    {
                        elementsForCollect.Add(Elements[x, y]);
                    }
                }
            }
        }

        return elementsForCollect;
    }

    public List<Elements> CheckHorizontal(int raw, int colum)
    {
        List<Elements> elementsInLine = new List<Elements>();
        Elements mainElement = Elements[raw, colum];
        int nextRaw = raw + 1;
        if (nextRaw >= _raws - 1)
            return null;

        while (Elements[nextRaw, colum].IsActive && mainElement.Key == Elements[nextRaw, colum].Key)
        {
            elementsInLine.Add(Elements[nextRaw, colum]);
            if (nextRaw + 1 < _raws)
            {
                nextRaw++;
            }
            else
            {
                break;
            }
        }

        return elementsInLine;
    }

    public List<Elements> CheckVertical(int raw, int colum)
    {
        List<Elements> elementsInLine = new List<Elements>();
        Elements mainElement = Elements[raw, colum];
        int nextColum = colum + 1;
        if (nextColum >= _columns - 1)
            return null;

        while (Elements[raw, nextColum].IsActive && mainElement.Key == Elements[raw, nextColum].Key)
        {
            elementsInLine.Add(Elements[raw, nextColum]);
            if (nextColum + 1 < _columns)
            {
                nextColum++;
            }
            else
            {
                break;
            }
        }

        return elementsInLine;
    }

    private void OnElementClicked(Elements elements)
    {
    }

    /// <summary>
    /// Only for tests
    /// </summary>
    public void GenerateCollectedRaw(int testX, int testY)
    {
        var testConfig = _config.GetRandomElement();
        for (int x = testX; x < testX + 3; x++)
        {
            Elements[x, testY].SetTestConfig(testConfig);
        }
    }

    /// <summary>
    /// Only for tests
    /// </summary>
    public void GenerateCollectedColumn(int testX, int testY)
    {
        var testConfig = _config.GetRandomElement();
        for (int y = testY; y < testY + 3; y++)
        {
            Elements[testX, y].SetTestConfig(testConfig);
        }
    }
}