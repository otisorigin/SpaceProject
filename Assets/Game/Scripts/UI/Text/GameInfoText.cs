using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInfoText : MonoBehaviour
{
    [Inject] private GameManager _manager;
    public Text GameInfo;

    void Update()
    {
        GameInfo.text = _manager.CurrentPlayer.Name;
    }
}
