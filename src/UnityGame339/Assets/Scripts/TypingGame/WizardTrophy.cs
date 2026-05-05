using UnityEngine;

public class WizardTrophy : MonoBehaviour
{
    private Animator anim;
    void Awake() => anim = GetComponent<Animator>();
    
    public void PlayWizardSuccessAnimation() => anim.SetTrigger("Play");
}
