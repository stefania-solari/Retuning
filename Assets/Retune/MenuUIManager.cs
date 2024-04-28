using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{

    [SerializeField] private GameObject eventPanelUserInRange;
    [SerializeField] private GameObject eventPanelUserNotInRange;

    bool isUiPanelActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayStartEventPanel()
    {
        if(isUiPanelActive == false)
        {
        eventPanelUserInRange.SetActive(true);
        isUiPanelActive = true;
        }

    }
    public void DisplayUserNotInRangePanel()
    {
        if(isUiPanelActive == false)
        {
        eventPanelUserNotInRange.SetActive(true);
        isUiPanelActive = true;

        }


    }

    public void CloseButtonClick()
    {
        eventPanelUserInRange.SetActive(false);
        eventPanelUserNotInRange.SetActive(false);
        isUiPanelActive = false;
    }
}
