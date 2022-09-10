using UnityEngine;

public class CameraController : MonoBehaviour {
  private CameraChromosome chromossome;
  private CoverageService coverageService = new CoverageService();
  private void Start() {
    
  }

  private void Update() {
    if (!this.chromossome.Evaluated) {
        checkCoverageScore();
    }
  }

  private void checkCoverageScore() {
    float score = coverageService.getTotalCoverageData().Score;
    this.chromossome.Score = score;
    Debug.Log(score);
    this.chromossome.Evaluated = true;
  }

  public void setChromossome(CameraChromosome chromossome) {
    this.chromossome = chromossome;
    this.transform.position = chromossome.CameraPosition;
    this.transform.rotation = Quaternion.Euler((float)chromossome.TiltAngle, (float)chromossome.PanAngle, this.transform.rotation.z);
  }
}