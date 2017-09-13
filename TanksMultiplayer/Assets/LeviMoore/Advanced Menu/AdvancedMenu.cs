using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class AdvancedMenu : MonoBehaviour
{
    [Range(0, 10)]
    public float sensitivity;

    public bool useX;
    public bool reverseX;
    public bool useY;
    public bool reverseY;
    public bool smoorthChange;

    private bool roll;
    public RectTransform credits;
    public GameObject resolutionButton;
    public GameObject resolutionScrollView;
    public GameObject buttonPrefab;
    private bool firstLoadResolution = true;
    private bool toggleFull;

    void Update()
    {
        Vector3 _position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y - Screen.height / 2 + 120, sensitivity));

        int _x = 1;
        if (reverseX) _x = -1;
        if (!useX) _x = 0;
        int _y = 1;
        if (reverseY) _y = -1;
        if (!useY) _y = 0;
        transform.LookAt(new Vector3(_position.x * _x, _position.y * _y, 10));

        if (roll)
        {
            credits.position += new Vector3(0, -40, 0) * Time.deltaTime;
        }
    }


    GameObject showMenu;
    GameObject hodeMenu;
    public void HideMenu(GameObject _go)
    {
        if (smoorthChange)
        {
            showMenu = _go;
            StartCoroutine("SmoorthChangeHideIE");
        }
        else
        {
            CanvasGroup _group = _go.GetComponent<CanvasGroup>();
            _group.alpha = 0;
            _group.blocksRaycasts = false;
            _group.interactable = false;
        }
    }
    public void ShowMenu(GameObject _go)
    {
        if (smoorthChange)
        {
            hodeMenu = _go;
        }
        else
        {
            CanvasGroup _group = _go.GetComponent<CanvasGroup>();
            _group.alpha = 1;
            _group.blocksRaycasts = true;
            _group.interactable = true;
        }
        
        if (_go.name == "Credits")
        {
            roll = true;
            credits.position = new Vector3(0, 120, 0);
        }
        else
        {
            roll = false;
        }
    }

    IEnumerator SmoorthChangeHideIE()
    {
        RectTransform _rect = showMenu.GetComponent<RectTransform>();

        bool _wait = true;
        while (_wait)
        {
            _rect.position = new Vector3(0, Mathf.Lerp(_rect.position.y, -600, Time.deltaTime * 2));
            if (_rect.position.y < -500)
            {
                _wait = false;
                StartCoroutine("SmoorthChangeShowIE");
            }
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator SmoorthChangeShowIE()
    {
        RectTransform _rect = hodeMenu.GetComponent<RectTransform>();
        _rect.position = new Vector3(0, -500, 0);

        CanvasGroup _group = _rect.gameObject.GetComponent<CanvasGroup>();
        _group.alpha = 1;
        _group.blocksRaycasts = true;
        _group.interactable = true;

        bool _wait = true;
        while (_wait)
        {
            _rect.position = new Vector3(0, Mathf.Lerp(_rect.position.y, 500, Time.deltaTime));
            if (_rect.position.y > 0)
            {
                _wait = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SaveMusic(GameObject _go)
    {
        PlayerPrefs.SetInt("music", (int)_go.GetComponent<Slider>().value);
        _go.transform.parent.GetComponent<Text>().text = "Music: " + _go.GetComponent<Slider>().value.ToString();
    }
    public void LoadMusic(GameObject _go)
    {
        _go.GetComponent<Slider>().value = PlayerPrefs.GetInt("music");
        _go.transform.parent.GetComponent<Text>().text = "Music: " + PlayerPrefs.GetInt("music").ToString();
    }

    public void SaveVoice(GameObject _go)
    {
        PlayerPrefs.SetInt("voice", (int)_go.GetComponent<Slider>().value);
        _go.transform.parent.GetComponent<Text>().text = "Voice: " + _go.GetComponent<Slider>().value.ToString();
    }
    public void LoadVoice(GameObject _go)
    {
        _go.GetComponent<Slider>().value = PlayerPrefs.GetInt("voice");
        _go.transform.parent.GetComponent<Text>().text = "Voice: " + PlayerPrefs.GetInt("voice").ToString();
    }

    public void SaveFX(GameObject _go)
    {
        PlayerPrefs.SetInt("fx", (int)_go.GetComponent<Slider>().value);
        _go.transform.parent.GetComponent<Text>().text = "FX: " + _go.GetComponent<Slider>().value.ToString();
    }
    public void LoadFX(GameObject _go)
    {
        _go.GetComponent<Slider>().value = PlayerPrefs.GetInt("fx");
        _go.transform.parent.GetComponent<Text>().text = "FX: " + PlayerPrefs.GetInt("fx").ToString();
    }

    public void SaveQuality(GameObject _go)
    {
        int _value = (int)_go.GetComponent<Slider>().value;
        PlayerPrefs.SetInt("quality", _value);
        if (_value == 0)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Very Low";
        }
        else if (_value == 1)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Low";
        }
        else if (_value == 2)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Normal";
        }
        else if (_value == 3)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: High";
        }
        else if (_value == 4)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Ultra";
        }
    }
    public void LoadQuality(GameObject _go)
    {
        int _value = PlayerPrefs.GetInt("quality");
        _go.GetComponent<Slider>().value = _value;
        if (_value == 0)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Very Low";
        }
        else if (_value == 1)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Low";
        }
        else if (_value == 2)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Normal";
        }
        else if (_value == 3)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: High";
        }
        else if (_value == 4)
        {
            _go.transform.parent.GetComponent<Text>().text = "Quality Level: Ultra";
        }
    }

    public void SaveResolution(int _index)
    {
        Resolution[] _resolution = Screen.resolutions;
        Screen.SetResolution(_resolution[_index].width, _resolution[_index].height, toggleFull);

        resolutionButton.GetComponentInChildren<Text>().text = ResolutionToString(_resolution[_index]);
        resolutionScrollView.SetActive(false);
    }
    public void LoadResolution()
    {
        resolutionButton.GetComponentInChildren<Text>().text = ResolutionToString(Screen.currentResolution);
        if (firstLoadResolution)
        {
            firstLoadResolution = false;
            Resolution[] _resolution = Screen.resolutions;
            for (int i = 0; i < _resolution.Length; i++)
            {
                int _index = i;
                GameObject _go = Instantiate(buttonPrefab) as GameObject;
                _go.GetComponentInChildren<Text>().text = ResolutionToString(_resolution[i]);
                _go.transform.SetParent(resolutionScrollView.transform.Find("Panel").transform);

                _go.GetComponent<Button>().onClick.AddListener(
                    () => { SaveResolution(_index); }
                    );
            }
        }
    }
    string ResolutionToString(Resolution _resolution)
    {
        return _resolution.width + " x " + _resolution.height;
    }

    public void SaveFullscreen(GameObject _go)
    {
        toggleFull = _go.GetComponent<Toggle>().isOn;
        Screen.fullScreen = toggleFull;
    }
    public void LoadFullscreen(GameObject _go)
    {
        toggleFull = Screen.fullScreen;
        _go.GetComponent<Toggle>().isOn = toggleFull;
    }

    public void LoadKey(GameObject _go)
    {
        _go.GetComponentInChildren<Text>().text = PlayerPrefs.GetString(_go.name);
    }
    public void RemapKey(GameObject _go)
    {
        _go.GetComponentInChildren<Text>().text = "...";
        StartCoroutine(RemapKeyIE(_go));
    }
    IEnumerator RemapKeyIE(GameObject _go)
    {
        bool _wait = true;
        bool _ignoreMouse = true;
        while (_wait)
        {
            if (_ignoreMouse)
            {
                if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                {
                    _ignoreMouse = false;
                    yield return true;
                }
            }
            string _return = "";
            if (Input.GetMouseButton(0) && !_ignoreMouse)
			{
                _return = KeyCode.Mouse1.ToString();
			}
            else if (Input.GetMouseButton(1) && !_ignoreMouse)
			{
                _return = KeyCode.Mouse2.ToString();
			}
            else if (Input.anyKey)
            {
                KeyCode _keycode = FetchKey();
                if (_keycode != KeyCode.None)
                {
                    _return = _keycode.ToString();
                }
            }

            if (_return != "")
            {
                PlayerPrefs.SetString(_go.name, _return);
                _go.GetComponentInChildren<Text>().text = _return;
                _wait = false;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    KeyCode FetchKey()
    {
        int e = System.Enum.GetNames(typeof(KeyCode)).Length;
        for (int i = 0; i < e; i++)
        {
            if (Input.GetKey((KeyCode)i))
            {
                return (KeyCode)i;
            }
        }

        return KeyCode.None;
    }

    public void SaveDifficulty(GameObject _go)
    {
        int _value = (int)_go.GetComponent<Slider>().value;
        PlayerPrefs.SetInt("difficulty", _value);
        if (_value == 0)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Easy";
        }
        else if (_value == 1)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Normal";
        }
        else if (_value == 2)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Hard";
        }
        else if (_value == 3)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Very Hard";
        }
    }
    public void LoadDifficulty(GameObject _go)
    {
        int _value = PlayerPrefs.GetInt("difficulty");
        _go.GetComponent<Slider>().value = _value;
        if (_value == 0)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Easy";
        }
        else if (_value == 1)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Normal";
        }
        else if (_value == 2)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Hard";
        }
        else if (_value == 3)
        {
            _go.transform.parent.GetComponent<Text>().text = "Difficulty: Very Hard";
        }
    }

    public void SaveSubtitels(GameObject _go)
    {
        PlayerPrefs.SetInt("subtitel", Convert.ToInt32(_go.GetComponent<Toggle>().isOn));
    }
    public void LoadSubtitels(GameObject _go)
    {
        _go.GetComponent<Toggle>().isOn = Convert.ToBoolean(PlayerPrefs.GetInt("subtitel"));
    }
}
