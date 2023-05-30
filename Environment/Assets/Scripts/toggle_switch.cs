using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using Unity.VisualScripting;

public class toggle_switch : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private bool _isOn = false;
    public bool isOn //allows other scripts to access value without changing it
    {
        get 
        {
            return _isOn;
        }

        set
        {
            _isOn = value;
        }
    }

    [SerializeField]
    private RectTransform toggleIndicator;
    [SerializeField]
    private RectTransform toggleIndicator_off;
    [SerializeField]
    private Image backgroundImage;

    [SerializeField]
    private Color onColor;
    [SerializeField]
    private Color offColor;
    private Vector3 offX;
    private Vector3 onX;

    private AudioSource audioSource;
    public delegate void ValueChanged(bool value);
    public event ValueChanged valueChanged;

    // Start is called before the first frame update
    void Start()
    {
        offX = toggleIndicator.transform.position; //start position
        onX = toggleIndicator_off.transform.position;
        audioSource = this.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Toggle(isOn); //make sure the switch is set correctly
    }

    public void Toggle(bool value, bool playSFX = true)
    {
        if (value != isOn)
        {
            _isOn = value;

            ToggleColor(isOn);
            MoveIndicator(isOn);

            Debug.Log(isOn);

            if (playSFX)
            {
                audioSource.Play();
            }

            if (valueChanged != null)
            {
                valueChanged(isOn);
            }
        }
    }

    private void ToggleColor(bool value)
    {
        if (value) 
        {
            backgroundImage.color = onColor ;
        }
        else 
        {
            backgroundImage.color = offColor;
        }
    }

    private void MoveIndicator(bool value)
    {
        if (value)
        {
            toggleIndicator.transform.position = onX;
        }
        else
        {
            toggleIndicator.transform.position = offX;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Toggle(!isOn); //flips the switch when clicked
    }
}
