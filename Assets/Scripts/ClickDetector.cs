using UnityEngine;
using System.Collections;
 
public class ClickDetector : MonoBehaviour
{
    public bool HandleLeftClick = true;
    public bool HandleRightClick = true;
    public bool HandleMiddleClick = false;
    public string OnLeftClickMethodName = "OnLeftClick";
    public string OnRightClickMethodName = "OnRightClick";
    public string OnMiddleClickMethodName = "OnMiddleClick";
    public LayerMask layerMask;

    private RaycastHit hit;

    void Update()
    {
 
        // Left click
        if (HandleLeftClick && Input.GetMouseButtonDown(0))
        {
            if (GetClickedGameObject())
            {
                transform.SendMessage(OnLeftClickMethodName, hit, SendMessageOptions.RequireReceiver);
            }
                
        }
 
        // Right click
        if (HandleRightClick && Input.GetMouseButtonDown(1))
        {
            if (GetClickedGameObject())
            {
                transform.SendMessage(OnRightClickMethodName, hit, SendMessageOptions.RequireReceiver);
            }    
        }
        
        // Middle click
        if (HandleMiddleClick && Input.GetMouseButtonDown(2))
        {
            if (GetClickedGameObject())
            {
                transform.SendMessage(OnMiddleClickMethodName, hit, SendMessageOptions.RequireReceiver);
            }  
        }
    }
 
    bool GetClickedGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask));
    }

}
