using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using System.Text;

public class BoardController : MonoBehaviour
{
    [SerializeField] private float elementOffset = 1f;
    [SerializeField] private float sizeX = 1f;
    [SerializeField] private float sizeY = 1f;
    
    [SerializeField] private ElementsConfig _config;
    [SerializeField] private Elements prefab;
    public Elements[,] elements;
    
    public const int rowCount = 5;
    public const int columnCount = 10;
    
    public event Action<int> OnMatch;
    
    private int _rows;
    private int _columns;
    private bool isBlocked;
    private Elements _selectedElement;
    
    private Coroutine _coroutine;
    
    public void CreateGame(List<string> data)
    {
        CreateField();
        if (data == null)
        {
            GenerateElements();
        }
        else
        {
            GenerateElements(data);
        }
    }
    

    public void Reset()
    {
        if (_coroutine != null)
        {
            isBlocked = false;
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        GenerateElements();
    }

    public List<string> GetBoardData()
    {
        List<string> data = new List<string>();
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                data.Add(elements[x,y].Key);
            }
        }

        return data;
    }

    public void CreateField(int row, int column)
    {
        //tests
        if (elements != null)
        {
            int rows = elements.GetLength(0);
            int columns = elements.GetLength(1);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < columns; y++)
                {
                    Destroy(elements[x,y].gameObject);
                }
                
            }

        }
       
        _rows = row;
        _columns = column;
        elements = new Elements[row, column];
        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < column; y++)
            {
                elements[x, y] = Instantiate(prefab,transform);
            }
            
        }
    }

    public void CreateField()
    {
        _rows = rowCount;
        _columns = columnCount;
        elements = new Elements[_rows, _columns];
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                elements[x, y] = Instantiate(prefab,transform);
            }
            
        }
    }
    public void GenerateElements()
    {
        Vector2 startPosition = new Vector2(-elementOffset * _columns * 0.5f + elementOffset * 0.5f,
            elementOffset * _rows * 0.5f - elementOffset * 0.5f);
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                Vector2 position = startPosition + new Vector2(elementOffset * x, -elementOffset * y);
                List<Element> elements = GetPossibleElements(x,y,_rows,_columns);
                this.elements[x, y].Initialize(elements[Random.Range(0,elements.Count)], new Vector2(x,y), position);
                this.elements[x, y].OnClicked += OnElementClicked;
            }
        }
    }

    public void GenerateElements(List<string> data)
    {
        Vector2 startPosition = new Vector2(-elementOffset * _columns * 0.5f + elementOffset * 0.5f,
            elementOffset * _rows * 0.5f - elementOffset * 0.5f);
        int i = 0;
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                Vector2 position = startPosition + new Vector2(elementOffset * x, -elementOffset * y);
                var dataFromConfig = _config.GetByKey(data[i++]);
                elements[x, y].Initialize(dataFromConfig, new Vector2(x, y), position);
                elements[x, y].OnClicked += OnElementClicked;
            }
        }
    }
    
    private List<Element> GetPossibleElements(int row, int column, int rows, int columns)
    {
        var list = _config.Elements.ToList();
        
        int x = row;
        int y = column - 1;

        if (x >= 0 && x < rows && y >= 0 && y < columns)
        {
            if (elements[x, y].IsInitialized)
            {
                list.Remove(_config.GetByKey(elements[x, y].Key));
            }
        }

        x = row - 1;
        y = column;

        if (x >= 0 && x < rows && y >= 0 && y < columns)
        {
            if (elements[x, y].IsInitialized)
            {
                list.Remove(_config.GetByKey(elements[x, y].Key));
            }
        } 

        return list;
    }

    public List<Elements> SearchLines()
    {
        List<Elements> elementsForCollect = new List<Elements>();
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                if (elements[x, y].IsActive && !elementsForCollect.Contains(elements[x, y]))
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
                        elementsForCollect.Add(elements[x, y]);
                    }
                }
            }
        }

        return elementsForCollect;
    }
    public List<Elements> CheckHorizontal(int row, int column)
    {
        List<Elements> elementsInLine = new List<Elements>();
        Elements mainElement = elements[row, column];
        int nextRow = row + 1;

        if (nextRow >= _rows - 1)
            return null;
        
        while (elements[nextRow, column].IsActive && mainElement.Key == elements[nextRow, column].Key)
        {
            elementsInLine.Add(elements[nextRow,column]);
            if (nextRow + 1 < _rows)
            {
                nextRow++;
            }
            else
            {
                break;
            }
        }

        return elementsInLine;
    }
    public List<Elements> CheckVertical(int row, int column)
    {
        List<Elements> elementsInLine = new List<Elements>();
        Elements mainElement = elements[row, column];
        int nextColumn = column + 1;
        if (nextColumn >= _columns - 1)
            return null;
        
        while (elements[row, nextColumn].IsActive && mainElement.Key == elements[row, nextColumn].Key)
        {
            elementsInLine.Add(elements[row,nextColumn]);
            if (nextColumn + 1 < _columns)
            {
                nextColumn++;
            }
            else
            {
                break;
            }
        }

        return elementsInLine;
    }

    
    private void OnElementClicked(Elements element)
    {
        if(isBlocked)
            return;

        if (_selectedElement == null)
        {
            _selectedElement = element;
            element.SetSelected(true);
        }
        else
        {
            if (IsCanSwap(_selectedElement,element))
            {
                _selectedElement.SetSelected(false);
                Swap(_selectedElement, element);
                _selectedElement = null;
                _coroutine = StartCoroutine(CheckBoard());
            }
            else
            {
                _selectedElement.SetSelected(false);
                _selectedElement = element;
                element.SetSelected(true);
            }
        }
    }

    private bool IsCanSwap(Elements selectedElement, Elements targetElement)
    {
        Vector2 selectedPosition = selectedElement.GridPosition;
        Vector2 targetPosition = targetElement.GridPosition;
        
        if (selectedPosition.x + 1 == targetPosition.x)
        {
            return true;
        }
        if (selectedPosition.x - 1 == targetPosition.x)
        {
            return true;
        }
        if (selectedPosition.y + 1 == targetPosition.y)
        {
            return true;
        }
        if (selectedPosition.y - 1 == targetPosition.y)
        {
            return true;
        }
        
        return false;
    }

    private void Swap(Elements selectedElement, Elements targetElement)
    {
        Vector2 selectedPosition = selectedElement.GridPosition;
        Vector2 targetPosition = targetElement.GridPosition;
        
        Vector2 selectedBoardPosition = selectedElement.transform.localPosition;
        Vector2 targetBoardPosition = targetElement.transform.localPosition;
        
        elements[(int) selectedPosition.x, (int) selectedPosition.y] = targetElement;
        elements[(int) targetPosition.x, (int) targetPosition.y] = selectedElement;
        
        selectedElement.SetLocalPosition(targetPosition,targetBoardPosition);
        targetElement.SetLocalPosition(selectedPosition,selectedBoardPosition);
    }
    private IEnumerator CheckBoard()
    {
        isBlocked = true;
        bool isNeedRecheck;

        List<Elements> elementsListForCollected = new List<Elements>();

        do
        {
            isNeedRecheck = false;
            elementsListForCollected = SearchLines();
            if (elementsListForCollected.Count > 0)
            {
                OnMatch?.Invoke(elementsListForCollected.Count);
                DisableElements(elementsListForCollected);
                yield return new WaitForSeconds(Elements.ScalingTime);
                NormalizeBoard();
                yield return new WaitForSeconds(Elements.ScalingTime);
                isNeedRecheck = true;
            }
        } while (isNeedRecheck);
        
        isBlocked = false;
        _coroutine = null;
    }

    private void DisableElements(List<Elements> elementsListForCollected)
    {
        for (int i = 0; i < elementsListForCollected.Count; i++)
        {
            elementsListForCollected[i].Disable();
        }
    }
    
    private void NormalizeBoard()
    {
        
        for (int x = _rows - 1; x >= 0; x--)
        {
            List<Elements> freeElements = new List<Elements>();
            for (int y = _columns - 1; y >= 0; y--)
            {
                while (y>=0 && !elements[x,y].IsActive)
                {
                    freeElements.Add(elements[x,y]);
                    y--;
                }

                if (y >= 0 && freeElements.Count > 0)
                {
                    Swap(elements[x,y],freeElements[0]);
                    freeElements.RemoveAt(0);
                }
            }
        }

        for (int x = _rows - 1; x >= 0; x--)
        {
            for (int y = _columns - 1; y >= 0; y--)
            {
                if (!elements[x, y].IsActive)
                {
                    GenerateRandomElements(elements[x,y],_rows,_columns);
                    elements[x,y].Enable();
                    
                }
                
            }
        }
       
    }

    private void GenerateRandomElements(Elements element, int rows, int columns)
    {
        Vector2 gridPosition = element.GridPosition;
        var elements = GetPossibleElements((int) gridPosition.x, (int) gridPosition.y, _rows, _columns);
        element.SetConfig(elements[Random.Range(0,elements.Count)]);
    }


   

    /// <summary>
    /// Only For Tests
    /// </summary>
    public void GenerateCollectedRow(int testX, int testY)
    {
        var testConfig = _config.GetRandomElement();
        for (int x = testX; x < testX + 3; x++)
        {
            elements[x, testY].SetTestConfig(testConfig);
        }
    }
    /// <summary>
    /// Only For Tests
    /// </summary>

    public void GenerateCollectedColumn(int testX, int testY)
    {
        var testConfig = _config.GetRandomElement();
        for (int y = testY; y < testY + 3; y++)
        {
            elements[testX, y].SetTestConfig(testConfig);
        }
    }
}
