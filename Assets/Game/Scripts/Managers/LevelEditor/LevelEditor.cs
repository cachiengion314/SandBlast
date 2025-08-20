using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class LevelInformation
{
  [ViewOnly] public int Index;
  [ViewOnly] public float3 TopGridPosition;
  [ViewOnly] public int2 TopGridSize;
  [Range(1, 8)]
  public int AmountSlot = 2;
  public ColorBlockPartitionData[] ColorBlockPartitionDatas;
}
[Serializable]
public class GenerateLevelData
{
  public int ColorValue;
  public int RowAmount;
}

public class LevelEditor : MonoBehaviour
{
  [Header("Level Editor")]
  [SerializeField] GridWorld gridWord;
  [SerializeField] GridEditorControl gridEditorPref;
  [SerializeField] GridEditorControl[] gridEditorInstance;
  [SerializeField] LevelInformation levelInformation;
  [SerializeField] GenerateLevelData[] datas;
  [SerializeField][Range(1, 30)] int levelSelected = 1;

  [NaughtyAttributes.Button]
  void GenerateLevelData()
  {
    float limitY = 10;
    int y = GetGridY();
    gridWord.GridScale = new float2(1, 1);
    gridWord.GridSize = new int2(10, y);
    gridWord.transform.position = new float3(0, limitY + y / 2, 0);
    CreateGrid();
    FillData(y);
  }
  void CreateGrid()
  {
    ClearGrid();
    gridWord.InitConvertedComponents();

    gridEditorInstance = new GridEditorControl[gridWord.GridSize.x * gridWord.GridSize.y];
    for (int i = 0; i < gridEditorInstance.Length; ++i)
    {
      var instance = Instantiate(gridEditorPref, gridWord.transform);
      var pos = gridWord.ConvertIndexToWorldPos(i);
      var scale = gridWord.GridScale * 0.9f;
      instance.transform.position = pos;
      instance.transform.localScale = new Vector3(scale.x, scale.y, 1);
      gridEditorInstance[i] = instance;
    }
  }

  void FillData(int y)
  {
    var listRows = new List<int>(y);
    for (int i = 0; i < y; i++) listRows.Add(i);
    foreach (var data in datas)
    {
      for (int i = 0; i < data.RowAmount; i++)
      {
        var idx = UnityEngine.Random.Range(0, listRows.Count);
        var row = listRows[idx];
        listRows.RemoveAt(idx);
        for (int x = 0; x < gridWord.GridSize.x; x++)
        {
          var grid = new int2(x, row);
          var index = gridWord.ConvertGridPosToIndex(grid);
          var block = gridEditorInstance[index];
          block.type = GirdEditorControlType.Block;
          block.data = new ColorBlockPartitionData
          {
            Index = index,
            ColorValue = data.ColorValue,
            Health = 1
          };
          block.OnValidate();
        }
      }
    }
  }

  int GetGridY()
  {
    int y = 0;
    foreach (var data in datas)
      y += data.RowAmount;
    return y;
  }

  [NaughtyAttributes.Button]
  void Clear()
  {
    levelInformation = new LevelInformation();
    ClearGrid();
  }

  void ClearGrid()
  {
    for (int i = gridWord.transform.childCount - 1; i >= 0; i--)
    {
      var child = gridWord.transform.GetChild(i);
      DestroyImmediate(child.gameObject);
    }
  }

  [NaughtyAttributes.Button]
  void LoadLevel()
  {
    LoadLevelFrom(levelSelected);
  }

  public void LoadLevelFrom(int level)
  {
    var _rawLevelInfo = Resources.Load<TextAsset>(
      "Levels/" + KeyString.NAME_LEVEL_FILE + level
    ).text;
    var levelInfo = JsonUtility.FromJson<LevelInformation>(_rawLevelInfo);

    if (levelInfo == null) { print("This level is not existed!"); return; }
    levelInformation = levelInfo;

    gridWord.transform.position = levelInformation.TopGridPosition;
    gridWord.GridSize = levelInformation.TopGridSize;
    CreateGrid();

    LoadColorBlockPartitionData();

    print("Load level successfully");
  }

  void LoadColorBlockPartitionData()
  {
    var ColorBlockPartitionDatas = levelInformation.ColorBlockPartitionDatas;
    for (int i = 0; i < ColorBlockPartitionDatas.Length; i++)
    {
      var data = ColorBlockPartitionDatas[i];
      var grid = gridEditorInstance[data.Index];
      grid.type = GirdEditorControlType.Block;
      grid.data = data;
      grid.OnValidate();
    }
  }

  [NaughtyAttributes.Button]
  void SaveLevel()
  {
    levelInformation.Index = levelSelected - 1;
    levelInformation.TopGridPosition = gridWord.transform.position;
    levelInformation.TopGridSize = gridWord.GridSize;

    SaveColorBlockPartitionData();

    HoangNam.SaveSystem.Save(
      levelInformation,
      "Resources/Levels/" + KeyString.NAME_LEVEL_FILE + levelSelected
    );
    print("Save level successfully");
  }

  void SaveColorBlockPartitionData()
  {
    List<ColorBlockPartitionData> InitColorBlockPartitionDatas = new();
    for (int i = 0; i < gridEditorInstance.Length; i++)
    {
      var grid = gridEditorInstance[i];
      if (grid.type != GirdEditorControlType.Block) continue;
      grid.data.Index = i;
      InitColorBlockPartitionDatas.Add(grid.data);
    }
    levelInformation.ColorBlockPartitionDatas = InitColorBlockPartitionDatas.ToArray();
  }
}
