using UnityEditor;
using UnityEngine;

public class MyWindow : EditorWindow
{   
    public CoverageBox2 coverageBox;
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
        if(GUILayout.Button(text: "Start GA")) {
            var ga = GameObject.FindObjectOfType<CoverageGA>();
            ga.startGA();
        }
        if(GUILayout.Button(text: "Stop GA")) {
            var ga = GameObject.FindObjectOfType<CoverageGA>();
            ga.stopGA();
        }
        if(GUILayout.Button(text: "Set best chromosome in scene")) {
            var ga = GameObject.FindObjectOfType<CoverageGA>();
            ga.setBestChromosomeInScene();
        }
        if(GUILayout.Button("Total Score")) {
          TotalCoverageData totalCovData = MainController.getTotalCoverageData();
          Debug.Log($"COV SCORE: {totalCovData.Score} / AVG DISTANCE: {totalCovData.AvgCamDistance}");
          Debug.Log($"COV PRIO: {totalCovData.PriorityCoverage}");
          Debug.Log($"COV PRIV: {totalCovData.PrivacyCoverage}");
        }
        Constants.DRAW_GISMOS = EditorGUILayout.Toggle("Draw Coverage", Constants.DRAW_GISMOS);
        Constants.CELLS_DENSITY = int.Parse(EditorGUILayout.TextField ("Cells Density", Constants.CELLS_DENSITY.ToString()));
        Constants.WEIGHT_PRIV = float.Parse(EditorGUILayout.TextField ("W - Avoid Areas", Constants.WEIGHT_PRIV.ToString()));
        Constants.WEIGHT_PRI0 = float.Parse(EditorGUILayout.TextField ("W - Priority Areas", Constants.WEIGHT_PRI0.ToString()));
        // groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
        //     myBool = EditorGUILayout.Toggle ("Toggle", myBool);
        //     myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
        // EditorGUILayout.EndToggleGroup ();
    }

    private void startBaseObjects() {
      this.coverageBox = Utils.findInactiveObj<CoverageBox2>();
      this.cameraBox = Utils.findInactiveObj<CameraBox>();
    }

    private void createPossibleCameraArea() {
        CameraBox newBox = Instantiate(this.cameraBox);
        newBox.gameObject.SetActive(true);
        newBox.transform.position = new Vector3(0, 0, 0);
    }

    private void createAvoidedArea() {
        CoverageBox2 newBox = Instantiate(this.coverageBox);
        newBox.setType(CoverageBox2.CoverageType.Avoid);
        newBox.gameObject.SetActive(true);
        newBox.transform.position = new Vector3(0, 0, 0);
    }

    private void createCoverageArea() {
        CoverageBox2 newBox = Instantiate(this.coverageBox);
        newBox.setType(CoverageBox2.CoverageType.Cover);
        newBox.gameObject.SetActive(true);
        newBox.transform.position = new Vector3(0, 0, 0);
    }
}