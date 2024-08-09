using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_AutoPlayer : MonoBehaviour
{

    [ContextMenu("START AUTOPLAY")]
    public void StartAutoPlay()
    {
        StartCoroutine(AutoCoroutine());
    }

    List<KeyCode> keys =  new List<KeyCode>();

    private IEnumerator AutoCoroutine()
    {
        keys.Add(KeyCode.Space);
        keys.Add(KeyCode.UpArrow);
        keys.Add(KeyCode.LeftArrow);
        keys.Add(KeyCode.Space);
        keys.Add(KeyCode.Space);
        keys.Add(KeyCode.RightArrow);
        keys.Add(KeyCode.Space);
        keys.Add(KeyCode.RightArrow);
        keys.Add(KeyCode.UpArrow);
        keys.Add(KeyCode.DownArrow);

        int idx = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.9f);
            if (idx < keys.Count)
                GetComponent<BoxController>()._pressedKeyCode = keys[idx++];
        }
    }

}
