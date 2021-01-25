using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{
    public Button[] skillbuttons;
    public Button runButton;
    public Button defendButton;
    public Button switchButton;
    public int buttonPressed;

    public void SetButtonPressed(int num)
    {
        buttonPressed = num;
    }
}
