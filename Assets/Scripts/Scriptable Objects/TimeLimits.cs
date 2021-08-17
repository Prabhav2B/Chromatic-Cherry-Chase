using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeLimits", menuName = "Data/TimeLimits")]
public class TimeLimits : ScriptableObject
{
   public List<TimeLimit> timeLimits;

   [Serializable]
   public struct TimeLimit
   {
      public float minutes;
      public float seconds;
      public string surveyURL;
   }

}

