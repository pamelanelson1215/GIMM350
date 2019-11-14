using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//tells unity that this class can be serialized
[System.Serializable]
public class Save
{
  public List<int> livingTargetPositions = new List<int>();
  public List<int> livingTargetsTypes = new List<int>();

  public int hits = 0;
  public int shots = 0;
}
