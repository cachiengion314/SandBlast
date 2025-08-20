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
  [Range(1,8)]
  public int AmountSlot = 2;
  public ColorBlockPartitionData[] ColorBlockPartitionDatas;
}

public class LevelEditor : MonoBehaviour
{
  [Header("Level Editor")]
  [SerializeField] GridEditorControl gridEditorPref;
  [SerializeField] GridEditorControl[] gridEditorInstance;
  [SerializeField] LevelInformation levelInformation;
  [SerializeField][Range(1, 30)] int levelSelected = 1;
  [SerializeField] GridWorld gridWord;

  [NaughtyAttributes.Button]
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
