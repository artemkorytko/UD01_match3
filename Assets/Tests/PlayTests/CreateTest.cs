using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CreateTest
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
        MonoBehaviour.Destroy(_boardController);
    }
    
  /*  [Test]
    public void InField_IfValueIs1_ValueEqual1()
    {
        int value = 1;
       Assert.AreEqual(1,value);
    }*/
    
    [Test]
    public void InField_IfCreate_ElementsCountEqualRowsAndColumns()
    {
       //AAA
       
       // Arrange
       var field = _boardController;
       var rowCount = 5;
       var columnCount = 10;
       var expected = rowCount * columnCount;

       //Act
       field.CreateField(rowCount, columnCount);
       var elementsCount = field.elements.Length;
       
       //Assert
       Assert.AreEqual(expected,elementsCount);
    }

    [UnityTest]
    public IEnumerator InField_OnStart_FieldCreated()
    {
        var field = _boardController;
        yield return null;
        
        Assert.AreEqual(BoardController.rowCount * BoardController.columnCount,field.elements.Length);
    }

    [Test]
    public void InField_OnGenerateElements_ElementsExist()
    {
        var field = _boardController;
        var rowCount = 5;
        var columnCount = 10;
        field.CreateField(rowCount,columnCount);
        field.GenerateElements();
        for (int x = 0; x < BoardController.rowCount; x++)
        {
            for (int y = 0; y < BoardController.columnCount; y++)
            {
                Assert.IsNotEmpty(field.elements[x,y].Key);
            }
        }
    }
    
    [UnityTest]
    public IEnumerator InField_CheckVertical_CountNotEqualZero()
    {
        var field = _boardController;
        var rowCount = 5;
        var columnCount = 10;
        var testX = 1;
        var testY = 3;
        field.CreateField(rowCount,columnCount);
        field.GenerateElements();
        yield return new WaitForSeconds(2);
        field.GenerateCollectedColumn(testX,testY);
        var elements = field.CheckVertical(testX,testY);
        Assert.AreNotEqual(0,elements.Count);
         
    }
    
    [UnityTest]
    public IEnumerator InField_CheckHorizontal_CountNotEqualZero()
    {
        var field = _boardController;
        var rowCount = 5;
        var columnCount = 10;
        var testX = 1;
        var testY = 3;
        field.CreateField(rowCount,columnCount);
        field.GenerateElements();
        yield return new WaitForSeconds(2);
        field.GenerateCollectedRow(testX,testY);
        var elements = field.CheckHorizontal(testX,testY);
        Assert.AreNotEqual(0,elements.Count);

    }
    
    [UnityTest]
    public IEnumerator InField_SearchLines_CollectsExist()
    {
        var field = _boardController;
        var rowCount = 10;
        var columnCount = 10;
        var testXRow = 1;
        var testYRow = 3;
        var testXColumn = 3;
        var testYColumn = 5;
        field.CreateField(rowCount,columnCount);
        field.GenerateElements();
        yield return new WaitForSeconds(2);
        field.GenerateCollectedRow(testXRow,testYRow);
        field.GenerateCollectedColumn(testXColumn,testYColumn);
        var elements = field.SearchLines();
        Assert.GreaterOrEqual(elements.Count,3);
           
    }
}