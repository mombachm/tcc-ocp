using UnityEditor;
using UnityEngine;

public class MyWindow : EditorWindow
{   
    public CoverageBox coverageBox;
    public CameraBox cameraBox;

    [MenuItem("Window/OCP")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(MyWindow));
    }
    
    void OnGUI()
    {
        this.startBaseObjects();

        GUILayout.Label ("OCP Settings", EditorStyles.boldLabel);
        if(GUILayout.Button("Possible Camera Area")) {
            this.createPossibleCameraArea();
        }
        if(GUILayout.Button("Avoided Area")) {
            this.createAvoidedArea();
        }
        if(GUILayout.Button("Coverage Area")) {
            this.createCoverageArea();
        }
        if(GUILayout.Button("Total Score")) {
            Debug.Log("Count");
            Debug.Log(GameObject.FindObjectOfType<CoverageBox>().GetComponent<CoverageBox>().getScore());
        }

        // myString = EditorGUILayout.TextField ("Text Field", myString);
        // groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        //     myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        //     myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup ();
    }

    private void startBaseObjects() {
      this.coverageBox = Utils.findInactiveObj<CoverageBox>();
      this.cameraBox = Utils.findInactiveObj<CameraBox>();
    }

    private void createPossibleCameraArea() {
        CameraBox newBox = Instantiate(this.cameraBox);
        newBox.gameObject.SetActive(true);
        newBox.transform.position = new Vector3(0, 0, 0);
    }

    private void createAvoidedArea() {
        CoverageBox newBox = Instantiate(this.coverageBox);
        newBox.setType(CoverageBox.CoverageType.Avoid);
        newBox.gameObject.SetActive(true);
        newBox.transform.position = new Vector3(0, 0, 0);
    }

    private void createCoverageArea() {
        CoverageBox newBox = Instantiate(this.coverageBox);
        newBox.setType(CoverageBox.CoverageType.Cover);
        newBox.gameObject.SetActive(true);
        newBox.transform.position = new Vector3(0, 0, 0);
    }
}