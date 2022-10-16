using System.Threading.Tasks;
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
    
    void OnGUI() {
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
          if (Constants.TEST_ROUTINE) {
            ga.testRoutineW();
          } else {
            ga.startGARoutine();
          }
        }
        if(GUILayout.Button(text: "Stop GA")) {
            var ga = GameObject.FindObjectOfType<CoverageGA>();
            ga.stopGA();
        }
        if(GUILayout.Button(text: "Set best chromosome in scene")) {
            var ga = GameObject.FindObjectOfType<CoverageGA>();
            ga.setBestChromosomeInScene();
        }
        if(GUILayout.Button(text: "Capture cameras")) {
          var testName = Constants.testName;
          Constants.testName = "Manually Capture";
          var ga = GameObject.FindObjectOfType<CoverageGA>();
          ga.captureCameraImages();
          //Constants.testName = testName;
        }
        if(GUILayout.Button(text: "Rando Position cameras")) {
          var ga = GameObject.FindObjectOfType<CoverageGA>();
          ga.cameraConfigService.randomPositionCameras();
        }
        if(GUILayout.Button("Total Score")) {
          TotalCoverageData totalCovData = MainController.getTotalCoverageData();
          Debug.Log($"COV SCORE: {totalCovData.Score} / AVG DISTANCE: {totalCovData.AvgCamDistance}");
          Debug.Log($"COV PRIO: {totalCovData.PriorityCoverage}");
          Debug.Log($"COV MULTI PRIO: {totalCovData.MultiPriorityCoverage}");
          Debug.Log($"COV PRIV: {totalCovData.PrivacyCoverage}");
        }
        Constants.TEST_ROUTINE = EditorGUILayout.Toggle("Test Routine", Constants.TEST_ROUTINE);
        Constants.DRAW_GISMOS = EditorGUILayout.Toggle("Draw Coverage", Constants.DRAW_GISMOS);
        Constants.SHOW_COV_AREAS = EditorGUILayout.Toggle("Show/Hide Coverage Areas", Constants.SHOW_COV_AREAS);
        Constants.SHOW_PRIV_AREAS = EditorGUILayout.Toggle("Show/Hide Privacy Areas", Constants.SHOW_PRIV_AREAS);
        Constants.SHOW_CAM_AREAS = EditorGUILayout.Toggle("Show/Hide Camera Areas", Constants.SHOW_CAM_AREAS);
        Constants.CAM_COUNT = int.Parse(EditorGUILayout.TextField ("Cam Count", Constants.CAM_COUNT.ToString()));
        Constants.CELLS_DENSITY = int.Parse(EditorGUILayout.TextField ("Cells Density", Constants.CELLS_DENSITY.ToString()));
        Constants.CAM_CELLS_DENSITY = int.Parse(EditorGUILayout.TextField ("Cam Cells Density", Constants.CAM_CELLS_DENSITY.ToString()));
        Constants.WEIGHT_PRIV = float.Parse(EditorGUILayout.TextField ("W - Avoid Areas", Constants.WEIGHT_PRIV.ToString()));
        Constants.WEIGHT_PRI0 = float.Parse(EditorGUILayout.TextField ("W - Priority Areas", Constants.WEIGHT_PRI0.ToString()));
        Constants.WEIGHT_MULTI_PRIO = float.Parse(EditorGUILayout.TextField ("W - Multi Cam Cov Priority Areas", Constants.WEIGHT_MULTI_PRIO.ToString()));
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