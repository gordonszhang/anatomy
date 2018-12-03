using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Clickable : MonoBehaviour
{

    // Use this for initialization

    private List<string> _types = new List<string>() { "Learn", "Exit", "Back", "Easy", "Answer", "Medium", "Hard", "Submit"};
    public Text text;
    private bool _isColliding = false;
    public int behavior;
    public int index;

    void Update()
    {

        if ((Input.GetKeyDown(KeyCode.Z) || OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) && _isColliding)
        {
            
            switch (_types[behavior])
            {
                case "Learn":
                    SceneManager.LoadScene("racketball");
					SceneManager.UnloadSceneAsync("menu");
                    break;
                case "Exit":
                    Debug.Log("Quitting");
					#if UNITY_EDITOR
						// Application.Quit() does not work in the editor so
						// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
						UnityEditor.EditorApplication.isPlaying = false;
					#else
        				Application.Quit();
					#endif
                    break;
				case "Back":
                    Scene currentScene = SceneManager.GetActiveScene();
                    string sceneName = currentScene.name;
                    switch (sceneName)
                    {
                        case "easy":
                            SceneManager.LoadScene("menu");
                            SceneManager.UnloadSceneAsync("easy");
                            break;
                        case "medium":
                            SceneManager.LoadScene("menu");
                            SceneManager.UnloadSceneAsync("medium");
                            break;
                        case "hard":
                            SceneManager.LoadScene("menu");
                            SceneManager.UnloadSceneAsync("hard");
                            break;

                    }
					
                    break;
                case "Easy":
					SceneManager.LoadScene("easy");
					SceneManager.UnloadSceneAsync("menu");
                    break;
                case "Answer":
                    GameObject.Find("SelectionController").GetComponent<QuizController>().EasyAnswer(index);
                    break;
                case "Medium":
					SceneManager.LoadScene("medium");
					SceneManager.UnloadSceneAsync("menu");
                    break;
                case "Hard":
					SceneManager.LoadScene("hard");
					SceneManager.UnloadSceneAsync("menu");
                    break;
                case "Submit":
                    GameObject.Find("SelectionController").GetComponent<QuizController>().HardAnswer();
                    break;
            }
        }
    }

    public void SetIndex(int i) {
        index = i;
    }

    public void SetText(string t) {
        text.text = t;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject o = other.transform.parent.gameObject;

        if (o.name == "GazePointerRing")
        {
            _isColliding = true;
            GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
            Debug.Log(o.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject o = other.transform.parent.gameObject;
        if (o.name == "GazePointerRing")
        {
            GetComponent<Renderer>().material.shader = Shader.Find("Standard");
            _isColliding = false;
        }
    }


}
