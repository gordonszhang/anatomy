using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionRotation
{
    public Vector3 position;
    public Quaternion rotation;
    public PositionRotation(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

public class SelectionController : MonoBehaviour
{
    public string selected;
    private GameObject _selectedObject;
    private GameObject clearObject;
    public Text text;
    public Text description;

    public Text colors;

    public Text partNames;

    private Dictionary<string, PositionRotation> _modelInfo;

    private List<string> _modelNames = new List<string> { "Brain", "Ribcage", "LeftLung", "RightLung", "Heart" };

    public AudioSource playingAudio;

    private Dictionary<string, string> _descriptions = new Dictionary<string, string> {
        { "Heart", "Placeholder" },
        { "RightLung", "This part of the lung has three lobes and has more segments than the left. These lobes are divided into superior (upper), middle, and inferior (lower), by horizontal and oblique fissures. The right lung has a deep concavity on the inner surface that is called the cardiac impression, at the same level as the heart. It also has two bronchi, airway passages that regulate the flow of air into the lungs. Regarding its size, it is shorter and wider than the left lung." },
        { "LeftLung", "Unlike the right lung, the left lung has only two lobes instead of three, the upper and the lower. It is divided by a deep oblique fissure. With the heart close to the left lung, it partly compresses the anterior border of the left lung and so makes it comparatively smaller than the right lung. While it does not have a middle lobe like the right lung, it does have its ‘lingula’ (little tongue), a tongue-like projection on the superior lobe. Regarding its size, even though it is a bit longer, it is still smaller than the right lung because of the heart occupying space."},
        { "Brain", "The brain is an amazing three-pound organ that controls all functions of the body, interprets information from the outside world, and embodies the essence of the mind and soul. Intelligence, creativity, emotion, and memory are a few of the many things governed by the brain. Protected within the skull, the brain is composed of the cerebrum, cerebellum, and brainstem. The brain receives information through our five senses: sight, smell, touch, taste, and hearing - often many at one time. It assembles the messages in a way that has meaning for us, and can store that information in our memory. The brain controls our thoughts, memory and speech, movement of the arms and legs, and the function of many organs within our body." },
        { "Ribcage", "The rib cage is the arrangement of ribs attached to the vertebral column and sternum in the thorax of most vertebrates, that encloses and protects the heart and lungs. In humans, the rib cage, also known as the thoracic cage, is a bony and cartilaginous structure which surrounds the thoracic cavity and supports the shoulder girdle to form the core part of the human skeleton. A typical human rib cage consists of 24 ribs in 12 pairs, the sternum and xiphoid process, the costal cartilages, and the 12 thoracic vertebrae. The human rib cage is a component of the human respiratory system. It encloses the thoracic cavity, which contains the lungs . An inhalation is accomplished when the muscular diaphragm, at the floor of the thoracic cavity, contracts and flattens, while contraction of intercostal muscles lift the rib cage up and out."},
        { "Backbone", "Placeholder" }
    };

    private Dictionary<string, Material> defaultMaterials;

    private Dictionary<string, Dictionary<string, Material>> nameToMaterialLists;

    public void Start()
    {
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
        defaultMaterials = new Dictionary<string, Material> {
            { "Brain",
                Resources.Load("Brain") as Material
            },
            { "LeftLung",
                Resources.Load("Lungs") as Material
            },
            { "RightLung",
                Resources.Load("Lungs") as Material
            }
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
        //SetSelected(GameObject.Find("Brain"));
        AudioClip clip = Resources.Load<AudioClip>("audio/Welcome");
        playingAudio.PlayOneShot(clip);
    }

    public void SetSelected(GameObject o)
    {
        // Check if object is segmented or not. If it is, then assign colors and tags based on dict defined above
        string s = o.name;

        Debug.Log(string.Format("selected {0}", s));

        if (s == "Description") return;

        colors.text = "";

        if (nameToMaterialLists.ContainsKey(s))
        {
            Dictionary<string, Material> materialList = nameToMaterialLists[s];
            foreach (Transform child in o.transform)
            {
                Transform c = child.transform.Find(child.name);
                //GameObject temp = Instantiate(Resources.Load("Tag"), child.position, Quaternion.identity) as GameObject;
                GameObject tag = child.Find("Tag").gameObject;
                tag.SetActive(true);
                tag.transform.Find("Text").GetComponent<Text>().text = c.name;
                if (materialList.ContainsKey(c.name))
                {
                    c.GetComponent<Renderer>().material = materialList[c.name];
                    //child.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
                    string hex = ColorUtility.ToHtmlStringRGB(c.GetComponent<Renderer>().material.color);
                    colors.text += string.Format("\n<color=#{0}>■</color> {1}", hex, c.name);
                }

            }
        }

        else
        {
            o.GetComponent<Renderer>().material.shader = Shader.Find("Self-Illumin/Outlined Diffuse");
        }

        if (_selectedObject != null && _selectedObject != o)
        {
            if (nameToMaterialLists.ContainsKey(_selectedObject.name))
            {
                Dictionary<string, Material> materialList = nameToMaterialLists[_selectedObject.name];
                foreach (Transform child in _selectedObject.transform)
                {
                    Transform c = child.transform.Find(child.name);
                    //GameObject temp = Instantiate(Resources.Load("Tag"), child.position, Quaternion.identity) as GameObject;
                    GameObject tag = child.Find("Tag").gameObject;
                    tag.SetActive(false);
                    if (materialList.ContainsKey(c.name))
                    {
                        c.GetComponent<Renderer>().material = defaultMaterials[_selectedObject.name];
                        colors.text = "";
                    }

                }
            }
            else
            {
                GameObject ob = _selectedObject;
                Renderer r = ob.GetComponent<Renderer>();
                while (!r)
                {
                    ob = ob.transform.Find(_selectedObject.name).gameObject;
                    r = ob.GetComponent<Renderer>();
                }
                r.material.shader = Shader.Find("Standard");
            }

            // Unhighlight last "clear" organ
            if (clearObject)
            {
                if (nameToMaterialLists.ContainsKey(selected))
                {
                    foreach (Transform child in clearObject.transform)
                    {
                        Transform c = child.transform.Find(child.name);
                        c.GetComponent<Renderer>().material = Resources.Load<Material>("Clear");
                    }
                }
                else
                {
                    GameObject ob = clearObject;
                    Renderer r = ob.GetComponent<Renderer>();
                    while (!r)
                    {
                        ob = ob.transform.Find(clearObject.name).gameObject;
                        r = ob.GetComponent<Renderer>();
                    }
                    r.material = Resources.Load<Material>("Clear");
                }
                clearObject = null;
            }
        }

        _selectedObject = o;
        selected = s;
        text.text = "Selected Object: " + selected;
        description.text = _descriptions[selected];

        // Highlight corresponding "clear" organ
        clearObject = GameObject.Find("Clear" + s);
        if (nameToMaterialLists.ContainsKey(s))
        {
            foreach (Transform child in clearObject.transform)
            {
                Transform c = child.transform.Find(child.name);
                c.GetComponent<Renderer>().material = Resources.Load<Material>("HighlightedClear");
            }
        }
        else
        {
            GameObject ob = clearObject;
            Renderer r = ob.GetComponent<Renderer>();
            while (!r)
            {
                Debug.Log(string.Format("Looking in {0}", s));
                ob = ob.transform.Find("Clear" + s).gameObject;
                r = ob.GetComponent<Renderer>();
            }
            r.material = Resources.Load<Material>("HighlightedClear");
        }

        if (playingAudio && playingAudio.isPlaying)
        {
            playingAudio.Stop();
        }

        AudioClip clip = Resources.Load<AudioClip>("audio/" + s);
        playingAudio.PlayOneShot(clip);
    }

    public void SnapModel(GameObject o)
    {
        string s = o.name;
        o.transform.position = _modelInfo[s].position;
        o.transform.rotation = _modelInfo[s].rotation;
        if (_selectedObject != null && _selectedObject != o)
        {
            if (nameToMaterialLists.ContainsKey(_selectedObject.name))
            {
                Dictionary<string, Material> materialList = nameToMaterialLists[_selectedObject.name];
                foreach (Transform child in _selectedObject.transform)
                {
                    Transform c = child.transform.Find(child.name);
                    //GameObject temp = Instantiate(Resources.Load("Tag"), child.position, Quaternion.identity) as GameObject;
                    GameObject tag = child.Find("Tag").gameObject;
                    tag.SetActive(false);
                    if (materialList.ContainsKey(c.name))
                    {
                        c.GetComponent<Renderer>().material = defaultMaterials[_selectedObject.name];
                        colors.text = "";
                    }

                }
            }
            else
            {
                GameObject ob = _selectedObject;
                Renderer r = ob.GetComponent<Renderer>();
                while (!r)
                {
                    ob = ob.transform.Find(_selectedObject.name).gameObject;
                    r = ob.GetComponent<Renderer>();
                }
                r.material.shader = Shader.Find("Standard");
            }
        }

        if (clearObject)
        {
            if (nameToMaterialLists.ContainsKey(selected))
            {
                foreach (Transform child in clearObject.transform)
                {
                    Transform c = child.transform.Find(child.name);
                    c.GetComponent<Renderer>().material = Resources.Load<Material>("Clear");
                }
            }
            else
            {
                GameObject ob = clearObject;
                Renderer r = ob.GetComponent<Renderer>();
                while (!r)
                {
                    ob = ob.transform.Find(clearObject.name).gameObject;
                    r = ob.GetComponent<Renderer>();
                }
                r.material = Resources.Load<Material>("Clear");
            }
            clearObject = null;
        }

        _selectedObject = null;
        selected = "None";
        text.text = "Selected Object: " + selected;
        description.text = "Select an object to see its description.";

        if (playingAudio && playingAudio.isPlaying)
        {
            playingAudio.Stop();
        }
    
    }
}
