using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessRunnerCollectable : MonoBehaviour
{
    [SerializeField] int scoreToIncrease = 1;
    [SerializeField] AudioClip itemAudioClip = null;

    public void GetItem()
    {
        ScoreManager.GetInstance().IncreaseScore(scoreToIncrease);
        if(itemAudioClip != null)
        {
            SFXManager.GetInstance().PlayClip(itemAudioClip);
        }

        Destroy(this.gameObject);
    }

}
