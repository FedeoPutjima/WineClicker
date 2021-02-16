using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeBerry : MonoBehaviour
{
    [HideInInspector] public int clickAmount = 1;
    Animator anim;
    public GameObject popUpTextPrefab;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void Click()
    {
        GameManager.instance.AddBerries(clickAmount);
        anim.SetTrigger("click");

        GameObject pop = Instantiate(popUpTextPrefab, this.transform, false) as GameObject;
        pop.transform.position = Input.mousePosition;

        pop.GetComponent<PopUpText>().ShowInfo(clickAmount);
    }
}
