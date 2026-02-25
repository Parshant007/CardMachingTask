using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// LeveState class define how deficult level is using Enum and this is public so that every class can access it and manupate it
/// </summary>
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
