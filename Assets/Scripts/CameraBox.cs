using UnityEngine;


public class CameraBox : MonoBehaviour
{
  [HideInInspector][SerializeField] new Renderer renderer;
  public Vector3[] cameraPositions;
  private Bounds bounds;
  private int width;
  private int height;
  private int depth;
  private float cellDiameter;
  private int cellDensity;
  private void Awake()
  {
    this.cellDensity = Constants.CELLS_DENSITY;
    this.renderer = GetComponent<MeshRenderer>();
    this.bounds = renderer.bounds;
    this.updateDimensions();
  }

  private void Start()
  {

  }

  private void Update()
  {
    if (this.cellDensity != Constants.CAM_CELLS_DENSITY) {
      this.cellDensity = Constants.CAM_CELLS_DENSITY;
      this.updateDimensions();
    }
    if (this.renderer.enabled != Constants.SHOW_CAM_AREAS) {
      this.renderer.enabled = !this.renderer.enabled;
    }
  }

  private void OnDestroy()
  {
    this.cameraPositions = null;
  }

  private void OnDrawGizmos()
  {
    if (!this.renderer) return;
    if (!Constants.DRAW_GISMOS || !this.renderer.enabled) return;
    if (this.cameraPositions is not null)
    {
      for (int x = 0; x < width * height * depth; ++x)
      {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.cameraPositions[x], .05f);
      }
    }
  }

  private void updateDimensions() {
    this.width = (int)(bounds.size.x * this.cellDensity + 1);
    this.height = (int)(bounds.size.y * this.cellDensity + 1);
    this.depth = (int)(bounds.size.z * this.cellDensity + 1);

    this.cellDiameter = 1.0f / this.cellDensity;

    this.cameraPositions = new Vector3[width * height * depth];
    for (int x = 0; x < width; ++x)
    {
      for (int y = 0; y < height; ++y)
      {

        for (int z = 0; z < depth; ++z)
        {
          Vector3 translate = transform.position - (bounds.size / 2);// + new Vector3(cellDiameter/2, cellDiameter/2, cellDiameter/2);
          Vector3 pos = translate + new Vector3(x: (float)(x) / cellDensity, (float)(y) / cellDensity, (float)(z) / cellDensity);
          this.cameraPositions[x + width * (y + height * z)] = pos;
        }
      }
    }
  }
}