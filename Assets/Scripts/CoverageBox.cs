using UnityEngine;

public class CoverageBox : MonoBehaviour
{

  [HideInInspector][SerializeField] new Renderer renderer;
  public CoverageType type;
  public int cellDensity;
  private CullingGroup[] groups;
  private BoundingSphere[] spheres;
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
    Debug.Log($"CAMERAS COUNT: {Camera.allCamerasCount}");
    this.groups = new CullingGroup[Camera.allCamerasCount];

    this.renderer = GetComponent<MeshRenderer>();
    bounds = renderer.bounds;

    width = (int)(bounds.size.x * cellDensity);
    height = (int)(bounds.size.y * cellDensity);
    depth = (int)(bounds.size.z * cellDensity);

    cellDiameter = 1.0f / cellDensity;


    this.spheres = new BoundingSphere[width * height * depth];
    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
      {

        for (int z = 0; z < depth; ++z)
        {
          Vector3 translate = transform.position - (bounds.size / 2) + new Vector3(cellDiameter / 2, cellDiameter / 2, cellDiameter / 2);
          Vector3 pos = translate + new Vector3(x: (float)(x) / cellDensity, (float)(y) / cellDensity, (float)(z) / cellDensity);
          this.spheres[x + width * (y + height * z)] = new BoundingSphere(pos, cellDiameter / 2);
        }
      }
    }

    for (int i = 0; i < Camera.allCamerasCount; i++) {
      Camera camera = Camera.allCameras[i];
      var camController = camera.GetComponent<CameraController>();
      camController.setCamIndex(i);
      this.groups[i] = new CullingGroup();
      this.groups[i].targetCamera = camera;
      this.groups[i].SetBoundingSpheres(this.spheres);
      this.groups[i].SetBoundingSphereCount(this.spheres.Length);
      this.groups[i].SetBoundingDistances(new float[] { 0, 10000f, 10000f });
      this.groups[i].SetDistanceReferencePoint(camera.transform.position);
    }
  }

  private void Update()
  {

  }
  private void OnDestroy()
  {
    for (int i = 0; i < this.groups.Length; i++) {
      this.groups[0].Dispose();
      this.groups[0] = null;
    }
  }

  private void OnDrawGizmos()
  {
    if (!Constants.DRAW_GISMOS) return;
    if (this.spheres is not null)
    {
      for (int x = 0; x < width * height * depth; ++x)
      {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(this.spheres[x].position, this.spheres[x].radius);
        for (int i = 0; i < this.groups.Length; i++) {
          if (this.groups[i].IsVisible(x))
          {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.spheres[x].position, this.spheres[x].radius);
          }
          else
          {

          }
        }
      }
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
    return getTotalCoverageCount() * 100f / this.spheres.Length;
  }

  private float getTotalCoverageCount() {
    bool visible = true;
    if (this.type == CoverageType.Avoid) {
      visible = false;
    }
    int sumCoverageCount = 0;
    for (int i = 0; i < this.groups.Length; i++) {
      sumCoverageCount += this.groups[i].QueryIndices(visible, null, 0);
    }
    return (float)sumCoverageCount;
  }
}