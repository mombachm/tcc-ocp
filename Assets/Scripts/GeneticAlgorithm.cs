using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    private void Start() {
        CameraBox[] objs = (CameraBox[])FindObjectsOfType(typeof(CameraBox)); 
        print("OBJECTS:");
        print(objs.Length);
        print(objs[0].cameraPositions.Length);
    }

    private void Update() {
        
    }
    private void OnDestroy() {

    }

}