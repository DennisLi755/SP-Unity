using UnityEngine;

public class AfterImage : MonoBehaviour {
    private float counter;

    void Start() {
        counter = 0;
    }

    void FixedUpdate() {
        counter++;
        if (counter >= 50)
            Destroy(this.gameObject);
    }
}