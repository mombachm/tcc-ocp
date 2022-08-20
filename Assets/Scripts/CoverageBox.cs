using UnityEngine;

public class CoverageBox : MonoBehaviour
{

  [HideInInspector][SerializeField] new Renderer renderer;
  public CoverageType type;
  public int cellDensity;
  private CullingGroup group;
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
    this.group = new CullingGroup();

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
    this.group.targetCamera = Camera.main;
    this.group.SetBoundingSpheres(this.spheres);
    this.group.SetBoundingSphereCount(this.spheres.Length);
    this.group.SetBoundingDistances(new float[] { 100f, 100f, 100f });
    this.group.SetDistanceReferencePoint(Camera.main.transform.position);
  }

  private void Start()
  {


  }

  private void Update()
  {

  }
  private void OnDestroy()
  {
    this.group.Dispose();
    this.group = null;
  }

  private void OnDrawGizmos()
  {
    if (this.spheres is not null)
    {
      for (int x = 0; x < width * height * depth; ++x)
      {
        if (this.group.IsVisible(x))
        {
          Gizmos.color = Color.green;
          Gizmos.DrawWireSphere(this.spheres[x].position, this.spheres[x].radius);
        }
        else
        {
          Gizmos.color = Color.red;
          Gizmos.DrawWireSphere(this.spheres[x].position, this.spheres[x].radius);
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
    return getCoverageCount() * 100f / this.spheres.Length;
  }

  private int getCoverageCount() {
    bool visible = true;
    if (this.type == CoverageType.Avoid) {
      visible = false;
    }
    return this.group.QueryIndices(visible, null, 0);
  }

}