using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RythhmMagic
{
    [RequireComponent(typeof(ControllerLaser))]
    [RequireComponent(typeof(AudioSource))]

    public class VRUIInput : MonoBehaviour
    {
		AudioSource audioSource;
		[SerializeField] AudioClip hoverBtnClip;
		[SerializeField] AudioClip clickBtnClip;

        private ControllerLaser laser;

        private void Awake()
        {
			audioSource = GetComponent<AudioSource>();

			laser = GetComponent<ControllerLaser>();
            laser.PointerIn -= HandlePointerIn;
            laser.PointerIn += HandlePointerIn;

            laser.PointerClick -= HandleTriggerClicked;
            laser.PointerClick += HandleTriggerClicked;

            laser.PointerOut -= HandlePointerOut;
            laser.PointerOut += HandlePointerOut;
        }

        private void HandleTriggerClicked(object sender, PointerEventArgs e)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
        }

        private void HandlePointerIn(object sender, PointerEventArgs e)
        {
            var button = e.target.GetComponent<Button>();
            if (button != null)
            {
                button.Select();
				PlayAudio(hoverBtnClip);
				//Debug.Log("HandlePointerIn", e.target.gameObject);
			}
        }

        private void HandlePointerOut(object sender, PointerEventArgs e)
        {

            var button = e.target.GetComponent<Button>();
            if (button != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
				PlayAudio(clickBtnClip);
				//Debug.Log("HandlePointerOut", e.target.gameObject);
            }
        }

		void PlayAudio(AudioClip clip)
		{
			audioSource.clip = clip;
			audioSource.Play();
		}
    }
}