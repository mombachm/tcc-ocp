using UnityEngine;
using System.Linq;
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
  public int cellDensity;
  private CullingGroup[] groups;
  private Cell[] cells;
  private float[] camDistances;
  private int[] camVisibleCount;
  private Bounds bounds;
  private int width;
  private int height;
  private int depth;
  private float cellDiameter;

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
    this.renderer = GetComponent<MeshRenderer>();
    this.covCollider = GetComponent<Collider>();
    this.covCollider.enabled = false;
    bounds = renderer.bounds;

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

    for (int i = 0; i < Camera.allCamerasCount; i++) {
      Camera camera = Camera.allCameras[i];
      var camController = camera.GetComponent<CameraController>();
      camController.setCamIndex(i);
    }
    this.camDistances = new float[Camera.allCamerasCount];
    this.camVisibleCount = new int[Camera.allCamerasCount];
  }

  private void Update()
  {

  }
  private void OnDestroy()
  {

  }

  private void OnDrawGizmos()
  {
    if (!Constants.DRAW_GISMOS) return;
    if (this.cells is null) return;
    this.verifyCoverageWithRayCasting();
    for (int x = 0; x < width * height * depth; ++x) {
      for (int i = 0; i < Camera.allCamerasCount; i++) {
          // Debug.DrawRay(Camera.allCameras[i].transform.position, direction, Color.yellow);
          if (this.cells[x].Visible) {
              Gizmos.color = Color.green;
              Gizmos.DrawWireSphere(this.cells[x].Position, this.cells[x].Radius);
          } else {
              Gizmos.color = Color.red;
              Gizmos.DrawWireSphere(this.cells[x].Position, this.cells[x].Radius);
          }
        // UnityEditor.Handles.Label(this.cells[x].Position, $"{this.cells[x].VisibleCount}");
      }
    }
  }

  private void verifyCoverageWithRayCasting() {
    if (this.cells is null) return;
    for (int x = 0; x < width * height * depth; x++) {
      this.cells[x].Visible = false;
      this.cells[x].VisibleCount = 0;
    }
    if (!this.renderer.isVisible) return;
    this.covCollider.enabled = true;
    for (int i = 0; i < Camera.allCamerasCount; i++) {
      this.camDistances[i] = 0;
      this.camVisibleCount[i] = 0;
      for (int x = 0; x < width * height * depth; x++) {
        Vector3 direction = this.cells[x].Position - Camera.allCameras[i].transform.position;
        bool isCellInFrustrum = IsCellVisibleFromCam(this.cells[x], Camera.allCameras[i]);
        if (isCellInFrustrum && Physics.Raycast(Camera.allCameras[i].transform.position, direction, out var raycastHit)) { // I forget actual racyast syntax/parameter order, check docs if this doesn't work
          //Debug.DrawRay(Camera.allCameras[i].transform.position, direction, Color.yellow);
          if (raycastHit.collider.gameObject == this.gameObject) {
              this.cells[x].Visible = true;
              this.cells[x].VisibleCount = this.cells[x].VisibleCount + 1;
              this.camDistances[i] += raycastHit.distance;
              this.camVisibleCount[i]++;
          }
        }
        // UnityEditor.Handles.Label(this.cells[x].Position, $"{this.cells[x].VisibleCount}");
      }
    }
    this.covCollider.enabled = false;
  }

  public bool IsCellVisibleFromCam(Cell cell, Camera camera)
  {
      Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
      return GeometryUtility.TestPlanesAABB(planes, new Bounds(cell.Position, new Vector3(cell.Radius, cell.Radius, cell.Radius)));
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
    this.verifyCoverageWithRayCasting();
    CoverageData covData = new CoverageData();
    float avgCamDistance = this.getAverageDistanceToCam();
    float totalArea = this.getTotalArea();
    float coverage = this.getCoverage();
    covData.AreaCovered = (coverage * totalArea) / 100f;
    covData.Coverage = coverage;
    covData.avgCamDistance = avgCamDistance;
    covData.TotalArea = totalArea;
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

  private float getCoverageCount() {
    int sumCoverageCount = 0;
    sumCoverageCount = this.cells.Count(c => c.Visible == true);
    return (float)sumCoverageCount;
  }
}

public struct ScoreData
{
    public float Score;

    public float AverageDistance;
}