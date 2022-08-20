using Unity.VisualScripting;
using UnityEngine;

public class Utils
{
  public static T findInactiveObj<T>() where T : Component
  {
    T[] objects = GameObject.FindObjectsOfType<T>(true);
    foreach (T obj in objects)
    {
      if (!obj.gameObject.activeInHierarchy)
      {
        return obj;
      }
    }
    return null;
  }
}