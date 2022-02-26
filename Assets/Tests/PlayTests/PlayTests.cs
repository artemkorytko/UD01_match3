using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayTests
{
    private const string PREFAB_NAME = "BoardController";
    private BoardController _boardController;
    
    [SetUp]
    public void Setup()
    {
        _boardController = MonoBehaviour.Instantiate(Resources.Load<BoardController>(PREFAB_NAME));
    }

    [TearDown]
    public void TearDown()
    {
        MonoBehaviour.Destroy(_boardController.gameObject);
    }
    
    [Test]
    public void InField_IfCreate_ElementsAreEquals()
    {
        //Arrange
        var field = _boardController;
        int raw = 5;
        int colum = 10;
        var size = raw * colum;
        
        //Act
        field.CreateField(raw, colum);
        var currentSize = field.elements.Length;

        //Assert
        Assert.AreEqual(size, currentSize);
    }

    [Test]
    public void InField_OnGenerateElements_ElementsExist()
    {
        var field = _boardController;
        int raw = 5;
        int colum = 10;
        
        
        field.CreateField(raw, colum);
        field.GenerateElements();
        
        for (int x = 0; x < raw; x++)
        {
            for(int y = 0; y < colum; y++)
            {
                Assert.IsNotEmpty(field.elements[x, y].Key);
            }
        }
    }

    [UnityTest]
    public IEnumerator InField_OnStart_FieldCreate()
    {
        var field = _boardController;
        yield return null;
        Assert.AreEqual(BoardController.rawCount * BoardController.columnCount, field.elements.Length);
    }

    [UnityTest]
    public IEnumerator InField_CheckVertical_ElementsCountNotEqualZero()
    {
        var field = _boardController;
        var rawCount = 5;
        var columCount = 10;
        var testX = 1;
        var testY = 3;
        field.CreateField(rawCount, columCount);
        field.GenerateElements();
        yield return new WaitForSeconds(2);
        field.GenerateCollectedColumn(testX, testY);
        var elements = field.CheckVertical(testX, testY);
        Assert.AreNotEqual(0, elements.Count);
    }

    [UnityTest]
    public IEnumerator InField_CheckHorizontal_ElementsCountNotEqualZero()
    {
        var field = _boardController;
        var rawCount = 5;
        var columCount = 10;
        var testX = 1;
        var testY = 3;
        field.CreateField(rawCount, columCount);
        field.GenerateElements();
        yield return new WaitForSeconds(2);
        field.GenerateCollectedRaw(testX, testY);
        var elements = field.CheckHorizontal(testX, testY);
        Assert.AreNotEqual(0, elements.Count);
    }

    [UnityTest]
    public IEnumerator InField_SearchLines_CollectElementsExist()
    {
        var field = _boardController;
        var rawCount = 10;
        var columCount = 10;
        var testXRaw = 1;
        var testYRaw = 3;
        var testXColumn = 3;
        var testYColumn = 5;
        field.CreateField(rawCount, columCount);
        field.GenerateElements();
        yield return new WaitForSeconds(2);
        field.GenerateCollectedRaw(testXRaw, testYRaw);
        field.GenerateCollectedColumn(testXColumn, testYColumn);
        var elements = field.SearchLines();
        Assert.GreaterOrEqual(elements.Count, 3);
    }
}
