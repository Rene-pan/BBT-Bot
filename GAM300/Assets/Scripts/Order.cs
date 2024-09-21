using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Order : MonoBehaviour
{
    public Slider DecreasingTimer;
    public float DecreasingDuration;
    public string OrderName;
    public TextMeshProUGUI OrderNameHolder;
    public Sprite[] OrderIngredients;
    public GameObject IngredientList;
    public Vector2 IngredientImageSize;

    public void addIngredientIcons()
    {
        foreach (var item in OrderIngredients)
        {
            GameObject Ingredient = new GameObject();
            var IngredientImageHolder = Ingredient.AddComponent<Image>();
            IngredientImageHolder.sprite = item;
            //change ingredient scale
            IngredientImageHolder.rectTransform.sizeDelta = IngredientImageSize;
            Parent(IngredientList.transform, Ingredient, 0);
        }
    }
    public void UpdateOrderName()
    {
        OrderNameHolder.text = OrderName;
    }
    private void Parent(Transform Parent, GameObject child, int state)
    {
        switch (state)
        {
            case 0:
                child.transform.SetParent(Parent);
                break;
            case 1:
                child.transform.SetParent(null);
                break;
        }
    }

}
