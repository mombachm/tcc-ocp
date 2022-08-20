using UnityEngine;

public class CoverageService
{
    public CoverageService() {
      Debug.Log("CONSTRUCTOR");
    }

    public float getTotalCoverageScore()
    {
      return GameObject.FindObjectOfType<CoverageBox>().GetComponent<CoverageBox>().getScore();
    }
}