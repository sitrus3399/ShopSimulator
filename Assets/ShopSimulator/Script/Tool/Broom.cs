using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Broom : ToolShop
{
    public override void Use(GameObject target)
    {
        base.Use(target);

        if (target.CompareTag("Stain"))
        {
            Destroy(target);
        }
    }
}