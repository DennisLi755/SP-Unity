using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;

public class SelectContinueButtonOnActive : MonoBehaviour {

    private CanvasGroup cg;
    private bool wasActive;
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private EventSystem es;

    private void Start() {
        cg = GetComponent<CanvasGroup>();
    }

    private void Update() {
        bool isActive = cg.blocksRaycasts;
        if (!wasActive && isActive) {
            es.SetSelectedGameObject(continueButton.gameObject);
        }
        wasActive = isActive;
    }
}