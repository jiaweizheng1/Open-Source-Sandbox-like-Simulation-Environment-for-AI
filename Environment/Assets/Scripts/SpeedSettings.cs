using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSettings : MonoBehaviour
{
    public void OneSpeed() {
        Time.timeScale = 1f;
    }
    public void TwoSpeed() {
        Time.timeScale = 2f;
    }
    public void FourSpeed() {
        Time.timeScale = 4f;
    }
}
