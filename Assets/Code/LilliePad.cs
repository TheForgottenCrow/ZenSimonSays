using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilliePad : MonoBehaviour
{
    [SerializeField] int m_Id;
    [SerializeField] GameMaster m_Master;

    private void OnMouseDown()
    {
        m_Master.Log(m_Id.ToString() + "Pressed");
    }
}
