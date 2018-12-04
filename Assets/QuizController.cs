using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public string selected;
    private GameObject _selectedObject;
    private GameObject clearObject;
    public Text text;
    public Text description;

    public Text score;

    private int points;
    private int question;

    public Text partNames;

    private Dictionary<string, PositionRotation> _modelInfo;

    private List<string> _modelNames = new List<string> { "Brain", "Ribcage", "LeftLung", "RightLung", "Heart", "Backbone" };

    public List<GameObject> models;

    private List<int> usedIndices = new List<int>();

    private Dictionary<string, Material> defaultMaterials;

    private Dictionary<string, Dictionary<string, Material>> nameToMaterialLists;

    private const float NUM_QUESTIONS = 5;

    int correctIndex;

    string correctName;

    public Clickable[] buttons;

    public AudioSource audioSource;

    public string mode;

    private Vector3 startPosition = new Vector3(-2.569f, 1.789f, -0.925f);

    public void Start()
    {
        points = 0;
        question = 1;
        selected = "None";
        _selectedObject = null;
        _modelInfo = new Dictionary<string, PositionRotation>();

        nameToMaterialLists = new Dictionary<string, Dictionary<string, Material>> {
        { "Brain",
            new Dictionary<string, Material> {
                {"TemporalLobe", Resources.Load("Blue") as Material },
                {"ParietalLobe", Resources.Load("Green") as Material },
                {"OccipitalLobe", Resources.Load("Red") as Material },
                {"FrontalLobe", Resources.Load("Orange") as Material },
                {"Cerebellum", Resources.Load("Purple") as Material }
            }
        },
        { "LeftLung",
            new Dictionary<string, Material> {
                {"InferiorLobe", Resources.Load("Blue") as Material },
                {"SuperiorLobe", Resources.Load("Green") as Material },
            }
        },
        { "RightLung",
            new Dictionary<string, Material> {
                {"SuperiorLobe", Resources.Load("Red") as Material },
                {"MiddleLobe", Resources.Load("Orange") as Material },
                {"InferiorLobe", Resources.Load("Purple") as Material }
            }
        },
        };

        foreach (string modelName in _modelNames)
        {
            Vector3 pos = new Vector3();
            Quaternion rot = new Quaternion();
            pos = GameObject.Find(modelName).transform.position;
            rot = GameObject.Find(modelName).transform.rotation;
            _modelInfo[modelName] = new PositionRotation(pos, rot);
            Debug.Log(modelName);
        }
        score.text = "Score: 0";

        if (mode == "easy") EasyQuestion();
        if (mode == "medium") MediumQuestion();
        if (mode == "hard") HardQuestion();
    }

    public void HardQuestion()
    {
        text.text = "Assemble the organs in the correct arrangement!";
        description.text = "Press the Submit button to check your work.";

        for (int i = 0; i < _modelNames.Count; i++)
        {
            GameObject o = models[i];
            o.transform.position = startPosition;
            o.transform.rotation = UnityEngine.Random.rotation;
        }
    }

    public void MediumQuestion()
    {
        text.text = "Place the organ in the correct spot.";
        description.text = "";

        System.Random rnd = new System.Random();

        int r = rnd.Next(_modelNames.Count);
        while (usedIndices.Contains(r))
        {
            r = rnd.Next(_modelNames.Count);
        }

        usedIndices.Add(r);

        for (int i = 0; i < _modelNames.Count; i++)
        {
            GameObject o = models[i];

            if (i == r)
            {
                Debug.Log("active");
                o.SetActive(true);
                o.transform.position = startPosition;
                o.transform.rotation = UnityEngine.Random.rotation;
            }
            else
            {
                Debug.Log("inactive");
                o.SetActive(false);
                o.transform.position = _modelInfo[_modelNames[i]].position;
                o.transform.rotation = _modelInfo[_modelNames[i]].rotation;
            }
        }
        correctName = _modelNames[r];
    }
    public void EasyQuestion()
    {
        if (_selectedObject)
        {
            GameObject ob = _selectedObject;
            if (nameToMaterialLists.ContainsKey(_modelNames[correctIndex]))
            {

                foreach (Transform child in ob.transform)
                {
                    Transform c = child.transform.Find(child.name);
                    c.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                }
            }

            else
            {
                Renderer re = ob.GetComponent<Renderer>();
                while (!re)
                {
                    ob = ob.transform.Find(_modelNames[correctIndex]).gameObject;
                    re = ob.GetComponent<Renderer>();
                }
                re.material.shader = Shader.Find("Standard");
            }
        }

        text.text = "Name the highlighted organ.";
        description.text = "Press the button corresponding to your choice.";

        System.Random rnd = new System.Random();

        int r = rnd.Next(_modelNames.Count);
        while (usedIndices.Contains(r))
        {
            r = rnd.Next(_modelNames.Count);
        }
        correctIndex = r;

        List<int> tempIndices = new List<int>();
        foreach (Clickable c in buttons)
        {

            c.SetText(_modelNames[r]);
            c.SetIndex(r);

            tempIndices.Add(r);
            r = rnd.Next(_modelNames.Count);
     
            while (tempIndices.Contains(r))
            {
                r = rnd.Next(_modelNames.Count);
            }
        }

        usedIndices.Add(correctIndex);

        GameObject o = GameObject.Find(_modelNames[correctIndex]);
        if (nameToMaterialLists.ContainsKey(_modelNames[correctIndex]))
        {

            foreach (Transform child in o.transform)
            {
                Transform c = child.transform.Find(child.name);
                c.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
            }
        }

        else
        {
            Renderer re = o.GetComponent<Renderer>();
            while (!re)
            {
                o = o.transform.Find(_modelNames[correctIndex]).gameObject;
                re = o.GetComponent<Renderer>();
            }
            re.material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
        }

        _selectedObject = o;
    }

    public void EasyAnswer(int i)
    {
        Debug.Log("easy");
        if (i == correctIndex)
        {
            description.text = string.Format("Correct! The answer was {0}.", _modelNames[correctIndex]);
            AudioClip a = Resources.Load<AudioClip>("audio/Correct");
            audioSource.PlayOneShot(a);
            points++;
            score.text = "Score: " + points;
        }
        else
        {
            description.text = string.Format("Incorrect. The answer was {0}.", _modelNames[correctIndex]);
            AudioClip a = Resources.Load<AudioClip>("audio/Incorrect");
            audioSource.PlayOneShot(a);
        }
        question++;
        StartCoroutine(Wait());
    }

    public void MediumAnswer(bool correct)
    {
        if (correct)
        {
            description.text = string.Format("Correct!", _modelNames[correctIndex]);
            AudioClip a = Resources.Load<AudioClip>("audio/Correct");
            audioSource.PlayOneShot(a);
            points++;
            score.text = "Score: " + points;
        }
        else
        {
            description.text = string.Format("Incorrect location.", _modelNames[correctIndex]);
            AudioClip a = Resources.Load<AudioClip>("audio/Incorrect");
            audioSource.PlayOneShot(a);
        }
        question++;
        StartCoroutine(Wait());
    }

    public void HardAnswer()
    {
        
        List<string> missed = new List<string>();
        for (int i = 0; i < _modelNames.Count; i++)
        {
            GameObject o = models[i];
            OVRGrabbable g = o.GetComponent<OVRGrabbable>();
            while (!g) {
                o = o.transform.Find(o.name).gameObject;
                g = o.GetComponent<OVRGrabbable>();
            }
            bool rotationCheck = false;
            float diff = Quaternion.Angle(o.transform.rotation, _modelInfo[o.name].rotation);
            rotationCheck = diff < 60f;
            if (g._willSnap && rotationCheck) points++;
            else missed.Add(o.name);
        }

        score.text = "Score: " + points;

        if (points > 0.7f * models.Count)
        {
            Debug.Log("youwin");
            text.text = "You win!";
            string t = string.Format("You got {0} out of {1} organs correct.", points, models.Count);
            if (missed.Count > 0)
            {
                t += " You missed ";
                for (int i = 0; i < missed.Count; i++)
                {
                    t += missed[i];
                    if (i < missed.Count - 1)
                    {
                        t += ", ";
                    }
                    if (i == missed.Count - 1)
                    {
                        t += ".";
                    }
                }
            }


            description.text = t;
            AudioClip a = Resources.Load<AudioClip>("audio/Correct");
            audioSource.PlayOneShot(a);
        }
        else
        {
            text.text = "Game over!";
            string t = string.Format("You got {0} out of {1} organs correct. You missed ", points, models.Count);

            for (int i = 0; i < missed.Count; i++)
            {
                t += missed[i];
                if (i < missed.Count - 1)
                {
                    t += ", ";
                }
                if (i == missed.Count - 1)
                {
                    t += ".";
                }
            }
            description.text = t;
            AudioClip a = Resources.Load<AudioClip>("audio/Incorrect");
            audioSource.PlayOneShot(a);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(4);
        if (question > NUM_QUESTIONS) EndQuiz();
        else
        {
            if (mode == "easy") EasyQuestion();
            if (mode == "medium") MediumQuestion();
        }
    }

    public void EndQuiz()
    {
        if (points < 0.7f * NUM_QUESTIONS)
        {
            text.text = "Game over!";
            description.text = string.Format("You answered {0} out of {1} questions correctly. Keep practicing and try for a higher score!", points, NUM_QUESTIONS);

        }
        else
        {
            text.text = "You win!";
            description.text = string.Format("You answered {0} out of {1} questions correctly. Great job!", points, NUM_QUESTIONS);
        }

        foreach (Clickable c in buttons)
        {
            if(c != null)
            c.gameObject.SetActive(false);
        }
    }

    public void SnapModel(GameObject o)
    {
        if (mode == "hard") return;
        string s = o.name;
        OVRGrabbable g = o.GetComponent<OVRGrabbable>();
        float diff = Quaternion.Angle(o.transform.rotation, _modelInfo[o.name].rotation);
        if (g._willSnap && diff < 60f) MediumAnswer(true);
        else MediumAnswer(false);
    }
}
