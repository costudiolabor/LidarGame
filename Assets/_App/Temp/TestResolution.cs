using UnityEngine;
using UnityEngine.UI;

public class TestResolution : MonoBehaviour
{
   [SerializeField] private RectTransform canvas;
   [SerializeField] private Text textInfo;

   private void Awake()
   {

      Vector2 resolution = canvas.rect.size;
      textInfo.text = resolution.ToString();
   }
}
