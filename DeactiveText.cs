using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveText : MonoBehaviour
{
    Animator anim;
    // OnEnable called only when called
    void OnEnable()
    {

        anim = GetComponent<Animator>();
        AnimatorClipInfo[] info = anim.GetCurrentAnimatorClipInfo(0);
        Invoke(nameof(Deactivate), info[0].clip.length);
    }

    void Deactivate()
    {
        this.gameObject.SetActive(false);
    }
    
}
