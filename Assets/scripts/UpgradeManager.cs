using System;
using Unity.VisualScripting;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrillController.Instance.SelectDrill(1);
        }
    }

    public void DrillUpgrade()
    {
        DrillController.Instance.SelectDrill(1);
    }
}
