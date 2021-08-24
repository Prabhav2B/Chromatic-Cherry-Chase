using UnityEngine;

public class OpenURLGameKit : MonoBehaviour
{
    public string websiteAddress;

    public void OpenURLOnClick()
    {
        Application.OpenURL(websiteAddress);
    }
}