using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Level
{
    Easy,
    Moderate,
    Medium,
    Tricky,
    Hard,
    Extreme
}
public class LevelState : MonoBehaviour
{
    public Level level;
}
