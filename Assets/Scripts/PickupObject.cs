using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour {

    GameObject mainCamera;
    bool carrying = false;
    bool mouseDown = false;
    GameObject window;
    Vector3 mousePositionAtPickup;
    Vector3 windowPositionAtPickup;

    public float zoomSensitivity = 5;
    public float rotateSensitivity = 0.5F;


    
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        mousePositionAtPickup = Vector3.zero;
        windowPositionAtPickup = Vector3.zero;
    }



    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        if (mouseDown || Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            if (carrying)
            {
                moveWindow(window);
            }
            else
            {
                pickWindow(mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (carrying)
            {
                dropWindow();
            } 
            mouseDown = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0)
        {
            zoomWindow(mousePosition, scroll);
        }
    }



    Pickable checkForMouseHoverOverWindow(Vector3 mousePosition)
    {
        Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Pickable p = hit.collider.GetComponent<Pickable>();
            if (p != null)
            {
                return p;
            }
        }
        return null;
    }



    void pickWindow(Vector3 mousePosition)
    {
        Pickable p = checkForMouseHoverOverWindow(mousePosition);
        if (p != null)
        {
            carrying = true;
            window = p.gameObject;
            mousePositionAtPickup = mousePosition;
            mousePositionAtPickup.z = p.gameObject.transform.position.z;
            mousePositionAtPickup = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(mousePositionAtPickup);
            windowPositionAtPickup = window.transform.position;
        }
    }



    void moveWindow(GameObject o)
    {
        Vector3 pos = Input.mousePosition;
        pos.z = o.transform.position.z;
        pos = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(pos);
        o.transform.RotateAround(Vector3.zero, Vector3.up, (pos.x - mousePositionAtPickup.x - (o.transform.position.x - windowPositionAtPickup.x)) * Time.deltaTime * rotateSensitivity);
        o.transform.RotateAround(Vector3.zero, Vector3.left, (pos.y - mousePositionAtPickup.y - (o.transform.position.y - windowPositionAtPickup.y)) * Time.deltaTime * rotateSensitivity);
    }


    void zoomWindow(Vector3 mousePosition, float zoom)
    {
        Pickable p = checkForMouseHoverOverWindow(mousePosition);
         if (p != null)
        {
            window = p.gameObject;
            if(window.transform.localPosition.z + zoom * zoomSensitivity > 1)
                window.transform.Translate(Vector3.forward * zoom * zoomSensitivity, Space.Self);
        }
    }



    void dropWindow()
    {
        carrying = false;
        window = null;
        mousePositionAtPickup = Vector3.zero;
        windowPositionAtPickup = Vector3.zero;
    }
}

