using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Tester : MonoBehaviour
{
    void OnTester()
    {
        this.gameObject.AddComponent<GameTimer>();
    }
}
