using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private Button loadbtn;
    // Start is called before the first frame update
    void Start()
    {
        loadbtn.onClick.AddListener(Load);
    }

    private void Load() {
        SceneManager.LoadScene("IngameScene");
    }
}
