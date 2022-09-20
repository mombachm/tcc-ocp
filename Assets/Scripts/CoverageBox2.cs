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
}

public class CoverageBox2 : MonoBehaviour
{

  [HideInInspector][SerializeField] new Renderer renderer;
  Collider covCollider;
  public CoverageType type;
  public int cellDensity;
  private CullingGroup[] groups;
  private Cell[] cells;
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
    this.groups = new CullingGroup[Camera.allCamerasCount];

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
      // this.groups[i] = new CullingGroup();
      // this.groups[i].targetCamera = camera;
      // this.groups[i].SetBoundingSpheres(this.spheres);
      // this.groups[i].SetBoundingSphereCount(this.spheres.Length);
      // this.groups[i].SetBoundingDistances(new float[] { 0, 10000f, 10000f });
      // this.groups[i].SetDistanceReferencePoint(camera.transform.position);
    }
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
    for (int x = 0; x < width * height * depth; ++x) {
      this.cells[x].Visible = false;
      this.cells[x].VisibleCount = 0;
    }
    if (!this.renderer.isVisible) return;
    this.covCollider.enabled = true;
    for (int i = 0; i < Camera.allCamerasCount; i++) {
      for (int x = 0; x < width * height * depth; ++x) {
        Vector3 direction = this.cells[x].Position - Camera.allCameras[i].transform.position;
        bool isCellInFrustrum = IsCellVisibleFromCam(this.cells[x], Camera.allCameras[i]);
        if (isCellInFrustrum && Physics.Raycast(Camera.allCameras[i].transform.position, direction, out var raycastHit)) { // I forget actual racyast syntax/parameter order, check docs if this doesn't work
          //Debug.DrawRay(Camera.allCameras[i].transform.position, direction, Color.yellow);
          if (raycastHit.collider.gameObject == this.gameObject) {
              this.cells[x].Visible = true;
              this.cells[x].VisibleCount = this.cells[x].VisibleCount + 1;
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
    CoverageData covData = new CoverageData();
    float score = this.getScore();
    float totalArea = this.getTotalArea();
    covData.AreaCovered = (score * totalArea) / 100f;
    covData.Score = score;
    covData.TotalArea = totalArea;
    return covData;
  }

  public float getTotalArea() {
    Vector3 scale = this.transform.localScale;
    return scale.x * scale.y * scale.z;
  }

  public float getScore() {
    this.verifyCoverageWithRayCasting();
    return getTotalCoverageCount() * 100f / this.cells.Length;
  }

  private float getTotalCoverageCount() {
    bool visible = true;
    if (this.type == CoverageType.Avoid) {
      visible = false;
    }
    int sumCoverageCount = 0;
    // for (int i = 0; i < this.groups.Length; i++) {
    //   sumCoverageCount += this.groups[i].QueryIndices(visible, null, 0);
    // }
    sumCoverageCount = this.cells.Count(c => c.Visible == visible);
    // Debug.Log($"VISIBLE CELLS COUNT: {sumCoverageCount}");
    return (float)sumCoverageCount;
  }
}