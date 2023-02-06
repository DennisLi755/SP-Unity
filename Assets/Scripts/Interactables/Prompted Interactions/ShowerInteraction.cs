using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowerInteraction : PromptedInteraction
{
    public override void OnYes()
    {
        UIManager.Instance.FadeToBlack();
        //play shower noise
        StartCoroutine(EndShower());
        UIManager.Instance.interactText.transform.parent.gameObject.SetActive(false);
        UIManager.Instance.Prompt.SetActive(false);

        IEnumerator EndShower() {
            yield return new WaitForSeconds(2f);
            UIManager.Instance.FadeFromBlack();
            base.OnYes();
            UIManager.Instance.ActivateInteractText(base.AfterYesMessage);
        }
    }
}
