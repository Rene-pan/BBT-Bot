using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class IngredientIndicator : MonoBehaviour
{
    public Image ingredientIcon;
    public Sprite[] ingredientIcons;

    public void UpdateIngredient(int iconID)
    {
        ingredientIcon.sprite = ingredientIcons[iconID];
    }
}
