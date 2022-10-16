using Unity.VisualScripting;
using UnityEngine;

public class Utils
{
  public static string[] objToEnableColliders = new string[2] {"group_20", "group_27"}; 
  public static string[] objToDisableColliders = new string[1] {"group_160"}; 

  public static T findInactiveObj<T>() where T : Component {
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

  public static void initializeColliders() {
    var objs = UnityEngine.Object.FindObjectsOfType<GameObject>();
    foreach (var obj in objs) {
      Renderer renderer = obj.GetComponent<Renderer>();      
      if (renderer) {
        Material material = renderer.sharedMaterial;
        if (material.name == "Translucent_Glass_Gray" || material.name == "Translucent_Resin_Crush_Gray") {
          material.color = new Color(0.3f,0.5f,0.8f, 0.2f);
          var collider = obj.GetComponent<MeshCollider>();
          if (collider) collider.enabled = false;
          foreach (var objName in objToEnableColliders){
            if(obj.name == objName) collider.enabled = true;
          }
          foreach (var objName in objToDisableColliders){
            if(obj.name == objName) collider.enabled = false;
          }
        }
      }
    }
  }
}