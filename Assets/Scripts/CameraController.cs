using System.Collections;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour {
  private CameraChromosome chromossome;
  private CoverageService coverageService = new CoverageService();
  private int index;
  private void Start() {
    
  }

  private void Update() {
    calculateChromossomeScore(this.chromossome);
  }

  private void calculateChromossomeScore(CameraChromosome c) {
    if (c == null || c.Evaluated == true) return;
    var covData = coverageService.getTotalCoverageData();
    float w1 = 1f;
    float w2 = 1;
    c.Score = (covData.Score * w1) + ((100 / covData.avgCamDistance) * w2) ;
    //Debug.Log($"SCORE: {score} CHROMO: {c.CamerasSetup[this.index].Position} {c.CamerasSetup[this.index].PanAngle} {c.CamerasSetup[this.index].TiltAngle}");
    c.Evaluated = true;
  }

  public void setChromossome(CameraChromosome chromossome) {
    if (chromossome == null) return;
    this.chromossome = chromossome;
    CameraSetup camSetup = chromossome.CamerasSetup[this.index];
    //Debug.Log(message: $"SETTING CAMERA POS: {camSetup.Position} {(float)camSetup.PanAngle} {(float)camSetup.TiltAngle}"); 
    this.transform.position = camSetup.Position;
    this.transform.rotation = Quaternion.Euler((float)camSetup.TiltAngle, (float)camSetup.PanAngle, 0);
  }

  public void setCamIndex(int index) {
    this.index = index;
  }
}