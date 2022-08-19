using UnityEngine;


public class CameraBox : MonoBehaviour {
    [HideInInspector][SerializeField] new Renderer renderer;
    public Vector3[] cameraPositions;
    private Bounds bounds;
   private int width;
   private int height;
   private int depth;
   private float cellDiameter;
   public int cellDensity;
    private void Awake() {
        renderer = GetComponent<MeshRenderer>();
        bounds = renderer.bounds;

        width = (int)(bounds.size.x * cellDensity + 1);
        height =(int)(bounds.size.y * cellDensity + 1);
        depth = (int)(bounds.size.z * cellDensity + 1);

        cellDiameter = 1.0f / cellDensity;

        this.cameraPositions = new Vector3[width * height * depth];
        for (int x=0; x<width; ++x)
        {
            for (int y=0; y<height; ++y)
            {

                for (int z=0; z<depth; ++z)
                {
                    Vector3 translate = transform.position - (bounds.size / 2);// + new Vector3(cellDiameter/2, cellDiameter/2, cellDiameter/2);
                    Vector3 pos = translate  + new Vector3(x: (float)(x)/cellDensity, (float)(y)/cellDensity, (float)(z)/cellDensity);
                    this.cameraPositions[x + width * (y + height * z)] = pos;
                } 
            }
        }
        Camera.main.transform.position = this.cameraPositions[20];
    }

    private void Start() {

    }

    private void Update() {
        
    }

    private void OnDestroy() {
        this.cameraPositions = null;
    }

    private void OnDrawGizmos() {
        if (this.cameraPositions is not null) {
            for (int x=0; x<width * height * depth; ++x){
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(this.cameraPositions[x], .05f);
            }
        }
    }


}