using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Interactor : MonoBehaviour
{
    public GameObject terminalWindowUI;
    public StarterAssetsInputs playerInput;

    private Camera m_Cam;

    private void Start()
    {
        m_Cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Terminal"))
                {
                    if (hit.distance > 6)
                        return;

                    OpenTerminal();
                }

                if (hit.collider.CompareTag("Display"))
                {
                    if (hit.distance > 6)
                        return;

                    hit.collider.GetComponent<PostOpener>().OpenPost();
                }
            }
        }
    }

    private void OpenTerminal()
    {
        FreeToUseCursor();
        terminalWindowUI.SetActive(true);
    }

    public void FreeToUseCursor()
    {
        playerInput.cursorInputForLook = false;
        playerInput.cursorLocked = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerInput.canMove = false;
    }

    public void ResetPlayerLookCursor()
    {
        playerInput.cursorInputForLook = true;
        playerInput.cursorLocked = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerInput.canMove = true;
    }

}
