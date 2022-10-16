using System.Collections;
using System.IO;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour {
  private CameraChromosome chromossome;
  public int index;
  public bool hasChanged = false;

  private void Start() {}

  private void Update() {}

  public void setChromossome(CameraChromosome chromossome) {
    this.hasChanged = false;
    CameraSetup camSetup = chromossome.CamerasSetup[this.index];
    this.transform.position = camSetup.Position;
    this.transform.rotation = Quaternion.Euler((float)camSetup.TiltAngle, (float)camSetup.PanAngle, 0);
  }

  public void setCamIndex(int index) {
    this.index = index;
  }
  
  public void captureImage(string titleText, string subTitleText, string fileText) {
      Camera cam = this.gameObject.GetComponent<Camera>();
      int resW = 1920;
      int resH = 1080;
      RenderTexture rt = new RenderTexture(resW, resH, 24);
      cam.targetTexture = rt;
      Texture2D image = new Texture2D(resW, resH, TextureFormat.RGB24, false);
      cam.Render();
       RenderTexture.active = rt;
      image.ReadPixels(new Rect(0, 0, resW, resH), 0, 0);
      image.Apply();
      RenderTexture.active = rt;

      byte[] bytes = image.EncodeToPNG();
      Destroy(image);

      string path =$"./Captures/{titleText}/{subTitleText}/cam{this.index.ToString()}";
      bool exists = System.IO.Directory.Exists(path);
      if(!exists) System.IO.Directory.CreateDirectory(path);

      File.WriteAllBytes($"{path}/{fileText}-cam{this.index.ToString()}.png", bytes);
  }

  public void destroy() {
    Destroy(this.gameObject);
  }
}