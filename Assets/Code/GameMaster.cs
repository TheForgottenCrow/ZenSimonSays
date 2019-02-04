using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState
{
    ShowAnser = 0,
    inputAnser

}

public class GameMaster : MonoBehaviour
{
    private GameState m_GameState;


    
    void Update()
    {
        switch (m_GameState)
        {
            case GameState.ShowAnser:
                ShowAnswer();
                break;

            case GameState.inputAnser:
                InputAnswer();
                break;
        }
    }

    private void InputAnswer()
    {

    }

    private void ShowAnswer()
    {

    }


    public void Log(string msg)
    {
        Debug.Log(msg);
    }
}
