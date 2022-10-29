using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public class Cell {
  public Cell (Vector3 position, float radius) {
    this.Position = position;
    this.Radius = radius;
    this.VisibleCount = 0;
    this.Visible = false;
  }

  public Vector3 Position { get; set; }

  public float Radius { get; set; }

  public bool Visible { get; set; }

  public int VisibleCount { get; set; }

  public float Distance { get; set; }
}

public class CoverageBox2 : MonoBehaviour
{

  [HideInInspector][SerializeField] new Renderer renderer;
  Collider covCollider;
  public CoverageType type;
  private int cellDensity;
  private Cell[] cells;
  private float[] camDistances;
  private int[] camVisibleCount;
  private bool[] isRenderedByCam;
  private Bounds bounds;
  private int width;
  private int height;
  private int depth;
  private float cellDiameter;
  private int camCount;

  public enum CoverageType
  {
    Cover,
    Avoid
  }

  private void Awake()
  {

  }

  private void Start()
  {
    this.cellDensity = Constants.CAM_CELLS_DENSITY;
    this.renderer = GetComponent<MeshRenderer>();
    this.covCollider = GetComponent<Collider>();
    this.covCollider.enabled = false;
    bounds = renderer.bounds;

    this.updateDimensions();
  }

  private void Update()
  {
    // Debug.Log($"VISIBLE: {this.renderer.isVisible}");
    if (this.cellDensity != Constants.CELLS_DENSITY) {
      this.cellDensity = Constants.CELLS_DENSITY;
      this.updateDimensions();
    }
    if (this.type == CoverageType.Cover && this.renderer.enabled != Constants.SHOW_COV_AREAS) {
      this.renderer.enabled = !this.renderer.enabled;
    }
    if (this.type == CoverageType.Avoid && this.renderer.enabled != Constants.SHOW_PRIV_AREAS) {
      this.renderer.enabled = !this.renderer.enabled;
    }
  }
  private void OnDestroy()
  {

  }

  private void OnDrawGizmos()
  {
    if (!Constants.DRAW_GISMOS || !this.renderer.enabled) return;
    if (!Constants.DRAW_GISMOS) return;
    if (this.cells is null) return;
    this.verifyCoverageWithRayCasting();

    var cellSize = new Vector3(cellDiameter-0.01f, cellDiameter-0.001f, cellDiameter-0.01f);
    var cellSize2 = new Vector3(cellDiameter-0.02f, cellDiameter-0.02f, cellDiameter-0.02f);
    var cellSize3 = new Vector3(cellDiameter-0.03f, cellDiameter-0.03f, cellDiameter-0.03f);
    for (int x = 0; x < width * height * depth; ++x) {
      //for (int i = 0; i < Camera.allCamerasCount; i++) {
          // Debug.DrawRay(Camera.allCameras[i].transform.position, direction, Color.yellow);
         if (this.cells[x].Visible) {
              Gizmos.color = Color.green;
          } else {
              Gizmos.color = Color.red;            
          }
          Gizmos.DrawWireSphere(this.cells[x].Position, 0.1f / cellDensity);
          Gizmos.DrawWireSphere(this.cells[x].Position, 0.12f / cellDensity);
          Gizmos.DrawWireSphere(this.cells[x].Position, 0.13f / cellDensity);
          // Gizmos.DrawWireCube(this.cells[x].Position, cellSize);
          // Gizmos.DrawWireCube(this.cells[x].Position, cellSize2);
          // Gizmos.DrawWireCube(this.cells[x].Position, cellSize3);
        // UnityEditor.Handles.Label(this.cells[x].Position, $"{this.cells[x].VisibleCount}");
      //}
    }
    // for (int i = 0; i < Camera.allCamerasCount; i++) {
    //   Gizmos.color = Color.cyan;     
    //   var cam = Camera.allCameras[i];
    //   Gizmos.matrix = Matrix4x4.TRS(cam.transform.position, cam.transform.rotation, Vector3.one);
    //   Gizmos.DrawFrustum(cam.transform.position,
    //   cam.fieldOfView,
    //   cam.nearClipPlane,
    //   cam.farClipPlane,
    //   cam.aspect
    //   );
    // }
  }

  public void initCamData() {
    this.camCount = Constants.CAM_COUNT;
    for (int i = 0; i < Camera.allCamerasCount; i++) {
      Camera camera = Camera.allCameras[i];
      var camController = camera.GetComponent<CameraController>();
      camController.setCamIndex(i);
    }
    this.camDistances = new float[Camera.allCamerasCount];
    this.isRenderedByCam = new bool[Camera.allCamerasCount];
    this.camVisibleCount = new int[Camera.allCamerasCount];
  }

  private void updateDimensions() {
    width = (int)(bounds.size.x * cellDensity);
    height = (int)(bounds.size.y * cellDensity);
    depth = (int)(bounds.size.z * cellDensity);

    cellDiameter = 1.0f / cellDensity;


    this.cells = new Cell[width * height * depth];
    for (int x = 0; x < width; ++x) {
      for (int y = 0; y < height; ++y) {
        for (int z = 0; z < depth; ++z)
        {
          Vector3 translate = transform.position - (bounds.size / 2) + new Vector3(cellDiameter / 2, cellDiameter / 2, cellDiameter / 2);
          Vector3 pos = translate + new Vector3(x: (float)(x) / cellDensity, (float)(y) / cellDensity, (float)(z) / cellDensity);
          this.cells[x + width * (y + height * z)] = new Cell(pos, cellDiameter / 2);
        }
      }
    }
  }

  private void verifyCoverageWithRayCasting() {
    if (this.cells is null) return;
    if (!this.renderer.isVisible) return;
    for (int x = 0; x < width * height * depth; x++) {
      this.cells[x].Visible = false;
      this.cells[x].VisibleCount = 0;
    }

    this.covCollider.enabled = true;
    for (int i = 0; i < Camera.allCamerasCount; i++) {
      this.camDistances[i] = 0;
      this.camVisibleCount[i] = 0;
      var cameraPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.allCameras[i]);
      // if (this.isRenderedByCam[Camera.allCameras[i].GetComponent<CameraController>().index]) {
        if (this.IsObjInFrustrum(cameraPlanes)) {
          for (int x = 0; x < width * height * depth; x++) {
            Vector3 direction = this.cells[x].Position - Camera.allCameras[i].transform.position;
            bool isCellInFrustrum = IsCellInFrustrum(this.cells[x], cameraPlanes);
            if (isCellInFrustrum) {
              if (Physics.Raycast(Camera.allCameras[i].transform.position, direction, out var raycastHit)) {
                // Debug.DrawRay(Camera.allCameras[i].transform.position, direction, Color.yellow);
                if (raycastHit.collider.gameObject == this.gameObject) {
                    this.cells[x].Visible = true;
                    this.cells[x].VisibleCount = this.cells[x].VisibleCount + 1;
                    this.camDistances[i] += raycastHit.distance;
                    this.camVisibleCount[i]++;
                }
              }
            }
          }
        } 
      // }
    }
    this.covCollider.enabled = false;
  }

  void OnWillRenderObject() {
    if (isRenderedByCam is null) return;
    if (Camera.current.name == "SceneCamera") return;
    CameraController camController = Camera.current.GetComponent<CameraController>();
    if (camController is null) return;
    isRenderedByCam[camController.index] = true;
  }

  public bool IsCellInFrustrum(Cell cell, Plane[] cameraPlanes) {
    return GeometryUtility.TestPlanesAABB(cameraPlanes, new Bounds(cell.Position, new Vector3(cell.Radius, cell.Radius, cell.Radius)));
  }

  public bool IsObjInFrustrum(Plane[] cameraPlanes) {
    return GeometryUtility.TestPlanesAABB(cameraPlanes, this.bounds);
  }
 
  public void resetCullingInfo() {
    this.isRenderedByCam = new bool[Camera.allCamerasCount];
    for (int i = 0; i < isRenderedByCam.Length; i++) {
      isRenderedByCam[i] = false;
    }
  }

  public void setType(CoverageType type)
  {
    this.renderer = GetComponent<MeshRenderer>();
    switch (type)
    {
      case CoverageType.Cover:
        renderer.material = (Material)Resources.Load(path: "Blue", typeof(Material));
        break;
      case CoverageType.Avoid:
        renderer.material = (Material)Resources.Load(path: "Yellow", typeof(Material));
        break;
    }
    this.type = type;
  }

  public CoverageData getCoverageData() {
    verifyCoverageWithRayCasting();
    CoverageData covData = new CoverageData();
    float avgCamDistance = this.getAverageDistanceToCam();
    float totalArea = this.getTotalArea();
    float coverage = this.getCoverage();
    float multiCoverage = this.getMultiCoverage();
    covData.Coverage = coverage;
    covData.AreaCovered = (coverage * totalArea) / 100f;
    covData.MultiCoverage = multiCoverage;
    covData.AreaMultiCovered = (multiCoverage * totalArea) / 100f;
    covData.avgCamDistance = avgCamDistance;
    covData.TotalArea = totalArea;
    // Debug.Log( $"SCORE: {covData.Coverage}");
    return covData;
  }

  public float getTotalArea() {
    Vector3 scale = this.transform.localScale;
    return scale.x * scale.y * scale.z;
  }

  public float getAverageDistanceToCam() {
    float avgDistance = 0;
    int validCamCount = 0;
    for (int i = 0; i < this.camDistances.Length; i++) {
      if (this.camVisibleCount[i] > 0) {
        avgDistance += (this.camDistances[i] / this.camVisibleCount[i]);
        validCamCount++;
      }
    }
    return avgDistance / validCamCount;
  }

  public float getCoverage() {
    return getCoverageCount() * 100f / this.cells.Length;
  }

  public float getMultiCoverage() {
    return getMultiCoverageCount() * 100f / this.cells.Length;
  }

  private float getCoverageCount() {
    int sumCoverageCount = 0;
    sumCoverageCount = this.cells.Count(c => c.Visible == true);
    return (float)sumCoverageCount;
  }

  private float getMultiCoverageCount() {
    int sumCoverageCount = 0;
    sumCoverageCount = this.cells.Count(c => c.VisibleCount > 1);
    return (float)sumCoverageCount;
  }
}

public struct ScoreData
{
    public float Score;

    public float AverageDistance;
}