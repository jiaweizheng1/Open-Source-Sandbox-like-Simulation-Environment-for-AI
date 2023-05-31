using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSettings : MonoBehaviour
{
    public void OneSpeed() {
        Time.timeScale = 3f;
    }
    public void TwoSpeed() {
        Time.timeScale = 3.5f;
    }
    public void FourSpeed() {
        Time.timeScale = 4f;
    }
}
