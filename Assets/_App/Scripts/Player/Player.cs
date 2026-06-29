using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
   [SerializeField] private TMP_Text textBullet;
   [SerializeField] private Animator animator;
   
   private int _bullet;
   public bool IsLoad { get; set; }
   public Animator Animator => animator;

   public void UpdateBullet(int bullet)
   {
      _bullet = bullet;
      textBullet.text = _bullet.ToString();
   }

   public int GetBullet() => _bullet;

}

