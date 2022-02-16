using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _sizeX = 1;
    [SerializeField] private int _sizeY = 1;
    [SerializeField] private float _elementOffset = 1f;

    [Header("References")]
    [SerializeField] private GameObject _elementPrefab = null;
    [SerializeField] private ElementsConfig _elemrntsConfig = null;

    private Element[,] _elements = null;
    private Element _firstSelected = null;
    private bool _isBlocked = false;
    private Coroutine _checkCoroutine = null;

    public System.Action<int> OnMatch = null;
    public System.Action OnBoardClosed = null;

    public void Initialize()
    {
        GenerateField(_sizeY, _sizeX);
        GenerateElements();
    }

    public void Initialize(List<string> boardState)
    {
        GenerateField(_sizeY, _sizeX);
        LoadElements(boardState);
    }

    public List<string> GetBoardState()
    {
        List<string> state = new List<string>();

        for (int i = 0; i < _sizeY; i++)
        {
            for (int j = 0; j < _sizeX; j++)
            {
                state.Add(_elements[i, j].Key);
            }
        }

        return state;
    }

    public void Reset()
    {
        if (_checkCoroutine != null)
        {
            _isBlocked = false;
            StopCoroutine(_checkCoroutine);
            _checkCoroutine = null;
        }

        GenerateElements();
    }

    private void GenerateField(int row, int column)
    {
        _elements = new Element[row, column];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                _elements[i, j] = Instantiate(_elementPrefab, transform).GetComponent<Element>();
            }
        }
    }

    private void GenerateElements()
    {
        int rowCount = _elements.GetUpperBound(0) + 1;
        int columnCount = _elements.Length / rowCount;
        Vector3 startPoint = new Vector3(-_elementOffset * columnCount / 2f + _elementOffset / 2f, _elementOffset * rowCount / 2f - _elementOffset / 2f, 0f);

        for (int i = rowCount - 1; i >= 0; i--)
        {
            for( int j = 0; j < columnCount; j++)
            {
                Vector3 position = startPoint + new Vector3(_elementOffset * j, -_elementOffset * i, 0f);
                List<ElementsConfig.Element> elements = GetPossibleElements(i, j, rowCount, columnCount, true);

                _elements[i, j].Initialize(elements[Random.Range(0, elements.Count)], position, new Vector2(i, j));
                _elements[i, j].OnClicked += OnElementClicked;
            }
        }
    }

    private void LoadElements(List<string> boardState)
    {
        Vector3 startPoint = new Vector3(-_elementOffset * _sizeX / 2f + _elementOffset / 2f, _elementOffset * _sizeY / 2f - _elementOffset / 2f, 0f);

        for (int i = 0; i < boardState.Count; i++)
        {
            int row = i / _sizeX;
            int column = i % _sizeX;
            Vector3 position = startPoint + new Vector3(_elementOffset * column, -_elementOffset * row);
            ElementsConfig.Element element = _elemrntsConfig.GetByKey(boardState[i]);
            _elements[row, column].Initialize(element, position, new Vector2(row, column));
            _elements[row, column].OnClicked += OnElementClicked;
        }
    }

    private List<ElementsConfig.Element> GetPossibleElements(int row, int column, int rowCount, int columnCount, bool isFirst = false)
    {
        List<ElementsConfig.Element> possibleElements = new List<ElementsConfig.Element>();
        possibleElements.AddRange(_elemrntsConfig.Elements);

        int r = row;
        int c = column - 1;

        if (r >= 0 && r < rowCount && c >= 0 && c < columnCount)
        {
            if (_elements[r, c].IsInitialized)
            {
                possibleElements.Remove(_elements[r, c].ElementConfig);
            }
        }

        r = row + 1;
        c = column;

        if (r >= 0 && r < rowCount && c >= 0 && c < columnCount)
        {
            if (_elements[r, c].IsInitialized)
            {
                possibleElements.Remove(_elements[r, c].ElementConfig);
            }
        }

        return possibleElements;
    }

    private void OnElementClicked(Element element)
    {
        if (_isBlocked) return;

        if (_firstSelected == null)
        {
            _firstSelected = element;
            element.SetSelected(true);
        }
        else
        {
            if (IsCanSwap(_firstSelected, element))
            {
                element.SetSelected(true);
                Swap(_firstSelected, element);
                _firstSelected.SetSelected(false);
                element.SetSelected(false);
                _firstSelected = null;
                _checkCoroutine = StartCoroutine(CheckBoard());
            }
            else
            {
                if (_firstSelected == element)
                {
                    _firstSelected.SetSelected(false);
                    _firstSelected = null;
                }
                else
                {
                    _firstSelected.SetSelected(false);
                    _firstSelected = element;
                    element.SetSelected(true);
                }
            }
        }
    }

    private bool IsCanSwap(Element first, Element second)
    {
        Vector2 comparePosition = first.GridPosition;
        comparePosition.x += 1;
        if (comparePosition == second.GridPosition)
        {
            return true;
        }

        comparePosition.x = first.GridPosition.x - 1;
        if (comparePosition == second.GridPosition)
        {
            return true;
        }

        comparePosition = first.GridPosition;
        comparePosition.y += 1;
        if (comparePosition == second.GridPosition)
        {
            return true;
        }

        comparePosition.y = first.GridPosition.y - 1;
        if (comparePosition == second.GridPosition)
        {
            return true;
        }

        return false;
    }

    private void Swap(Element first, Element second)
    {
        _elements[(int)first.GridPosition.x, (int)first.GridPosition.y] = second;
        _elements[(int)second.GridPosition.x, (int)second.GridPosition.y] = first;

        Vector3 position = second.transform.localPosition;
        Vector2 gridPosition = second.GridPosition;
        second.SetLocalPosition(first.transform.localPosition, first.GridPosition);
        first.SetLocalPosition(position, gridPosition);
    }

    private IEnumerator CheckBoard()
    {
        _isBlocked = true;
        bool isNeedRecheck = false;
        List<Element> elementsForCollecting = new List<Element>();
        
        do
        {
            isNeedRecheck = false;
            
            elementsForCollecting.Clear();
            elementsForCollecting = SearchLines();

            if (elementsForCollecting.Count > 0)
            {
                DisableElements(elementsForCollecting);
                OnMatch?.Invoke(elementsForCollecting.Count);
                yield return new WaitForSeconds(elementsForCollecting[0].EffectDuration + 0.1f);

                if (NormalizeBoard())
                {
                    yield return new WaitForSeconds(elementsForCollecting[0].EffectDuration);
                }
                isNeedRecheck = true;
            }
        }
        while (isNeedRecheck);

        _checkCoroutine = null;
        _isBlocked = false;
    }

    private List<Element> SearchLines()
    {
        List<Element> elementsForCollecting = new List<Element>();
        int rowCount = _elements.GetUpperBound(0) + 1;
        int columnCount = _elements.Length / rowCount;

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                if (_elements[i, j].IsActive && !elementsForCollecting.Contains(_elements[i, j]))
                {
                    bool needAddFirst = false;

                    List<Element> checkResult = CheckHorizontal(i, j);

                    if (checkResult != null && checkResult.Count >= 2)
                    {
                        elementsForCollecting.AddRange(checkResult);
                        needAddFirst = true;
                    }

                    checkResult = CheckVertical(i, j);

                    if (checkResult != null && checkResult.Count >= 2)
                    {
                        elementsForCollecting.AddRange(checkResult);
                        needAddFirst = true;
                    }

                    if (needAddFirst)
                    {
                        elementsForCollecting.Add(_elements[i, j]);
                    }
                }
            }
        }

        return elementsForCollecting;
    }

    private List<Element> CheckHorizontal(int row, int column)
    {
        int rowCount = _elements.GetUpperBound(0) + 1;
        int columnCount = _elements.Length / rowCount;

        int nextRow = row;
        int nextColumn = column + 1;

        if (nextColumn >= columnCount)
        {
            return null;
        }

        List<Element> elementsInLine = new List<Element>();
        Element mainElement = _elements[row, column];

        while (_elements[nextRow, nextColumn].IsActive && mainElement.Key == _elements[nextRow, nextColumn].Key)
        {
            elementsInLine.Add(_elements[nextRow, nextColumn]);
            if (nextColumn + 1 < columnCount)
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

    private List<Element> CheckVertical(int row, int column)
    {
        int rowCount = _elements.GetUpperBound(0) + 1;

        int nextRow = row + 1;
        int nextColumn = column;

        if (nextRow >= rowCount)
        {
            return null;
        }

        List<Element> elementsInLine = new List<Element>();
        Element mainElement = _elements[row, column];
        while (_elements[nextRow, nextColumn].IsActive && mainElement.Key == _elements[nextRow, nextColumn].Key)
        {
            elementsInLine.Add(_elements[nextRow, nextColumn]);
            if (nextRow + 1 < rowCount)
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

    private void DisableElements(List<Element> elements)
    {
        for (int i = 0; i < elements.Count; i++)
        {
            elements[i].Disable();
        }
    }

    private bool NormalizeBoard()
    {
        bool isNeedWait = false;
        int rowCount = _elements.GetUpperBound(0) + 1;
        int columnCount = _elements.Length / rowCount;

        for (int i = columnCount - 1; i >= 0; i--)
        {
            List<Element> freeElements = new List<Element>();

            for (int j = rowCount - 1; j >= 0; j--)
            {
                while(j >= 0 && !_elements[j, i].IsActive)
                {
                    freeElements.Add(_elements[j, i]);
                    j--;
                }

                if (j >= 0 && freeElements.Count > 0)
                {
                    Swap(_elements[j, i], freeElements[0]);
                    freeElements.Add(freeElements[0]);
                    freeElements.RemoveAt(0);
                }
            }
        }

        for (int i = rowCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < columnCount; j++)
            {
                if (!_elements[i, j].IsActive)
                {
                    GenerateRandomElement(_elements[i, j], rowCount, columnCount);
                    _elements[i, j].Enable();
                    isNeedWait = true;
                }
            }
        }

        return isNeedWait;
    }

    private void GenerateRandomElement(Element element, int rowCount, int columnCount)
    {
        Vector2 gridPosition = element.GridPosition;
        List<ElementsConfig.Element> elements = GetPossibleElements((int)gridPosition.x, (int)gridPosition.y, rowCount, columnCount);

        if (elements.Count > 0)
        {
            element.SetConfig(elements[Random.Range(0, elements.Count)]);
        }
        else
        {
            element.SetConfig(_elemrntsConfig.Elements[Random.Range(0, _elemrntsConfig.Elements.Count)]);
        }
    }

    private void OnApplicationQuit()
    {
        OnBoardClosed?.Invoke();
    }
}
