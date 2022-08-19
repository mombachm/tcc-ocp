using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
  private CullingGroup group;
  public GameObject block;
  public GameObject[] blocks;
  public int width;
  public int height;
  public int depth;
  private BoundingSphere[] spheres;
  void Awake()
  {
    this.group = new CullingGroup();
    this.spheres = new BoundingSphere[width * height * depth];

    //this.blocks = new GameObject[height*width*depth];
  }
  // Start is called before the first frame update
  void Start()
  {
    //     for (int y=0; y<height; ++y)
    //    {
    //        for (int x=0; x<width; ++x)
    //        {
    //             for (int z=0; z<depth; ++z)
    //             {
    //                 this.blocks[0]=Instantiate(block, new Vector3(x,y,z), Quaternion.identity);
    //             } 
    //        }
    //    }       

    // Scene scene = SceneManager.GetActiveScene();
    // print(Camera.main);

    this.group.targetCamera = Camera.main;
    for (int y = 0; y < height; ++y)
    {
      for (int x = 0; x < width; ++x)
      {
        for (int z = 0; z < depth; ++z)
        {
          this.spheres[(width * height * y) + width * x + z] = new BoundingSphere(new Vector3(x, y, z), 0.5f);
        }
      }
    }

    //BoundingSphere[] spheresFlat = spheres[0].Concat(spheres[1]).ToArray().Concat(spheres[2]).ToArray();

    this.group.SetBoundingSpheres(this.spheres);
    this.group.SetBoundingSphereCount(this.spheres.Length);
    //this.group.SetBoundingDistances(new float[] { 0f, 300f, 300f});
    //this.group.SetDistanceReferencePoint(Camera.main.transform.position);
  }

  void OnDrawGizmos()
  {
    if (this.spheres is not null)
    {
      for (int x = 0; x < width * height * depth; ++x)
      {
        if (this.group.IsVisible(x))
        {
          Gizmos.DrawSphere(this.spheres[x].position, this.spheres[x].radius);
        }
        //print(this.group.IsVisible(0));
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
