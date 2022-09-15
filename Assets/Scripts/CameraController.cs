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

  }

  private void checkCoverageScore() {
    Debug.Log(message: $"CUR CAMERA POS: {this.transform.position} {this.transform.rotation}"); 
    float score = coverageService.getTotalCoverageData().Score;
    this.chromossome.Score = score;
    Debug.Log($"SCORE: {score} CHROMO: {this.chromossome.CamerasSetup[0].Position} {this.chromossome.CamerasSetup[0].PanAngle} {this.chromossome.CamerasSetup[0].TiltAngle}");
    this.chromossome.Evaluated = true;
  }

  public void setChromossome(CameraChromosome chromossome) {
    if (this.chromossome != null && this.chromossome.Evaluated == false) return;
    this.chromossome = chromossome;
    CameraSetup camSetup = chromossome.CamerasSetup[this.index];
    Debug.Log(message: $"SETTING CAMERA POS: {camSetup.Position} {(float)camSetup.PanAngle} {(float)camSetup.TiltAngle}"); 
    this.transform.position = camSetup.Position;
    this.transform.rotation = Quaternion.Euler((float)camSetup.TiltAngle, (float)camSetup.PanAngle, 0);
    StartCoroutine(waiter());
  }

  IEnumerator waiter()
  {
    yield return new WaitForEndOfFrame();
    if (this.chromossome != null && !this.chromossome.Evaluated) {
        checkCoverageScore();
    }
  }

  public void setCamIndex(int index) {
    this.index = index;
  }
}