using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInputUnlock : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(FindObjectOfType<PlayerManager>().UnlockInput);
    }

    
}
