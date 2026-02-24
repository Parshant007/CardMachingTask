using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{

    private int cardID;
    private int id;
    private bool isFlipped;
    private bool turning;


    private IEnumerator CardFlip(Transform thisTransform, float time, bool changeSprite)
    {
        Quaternion startRotation = thisTransform.rotation;
        Quaternion endRotation = thisTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            thisTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }
        //change sprite and flip another 90degree
        if (changeSprite)
        {
            isFlipped = !isFlipped;
          //      ChangeSprite();
            StartCoroutine(CardFlip(transform, time, false));
        }
        else
            turning = false;
    }
    // perform a 180 degree flip
    public void Flip()
    {
        turning = true;
      //  AudioPlayer.Instance.PlayAudio(0);
        StartCoroutine(CardFlip(transform, 0.25f, true));
    }

    public void CardBtnEvt()
    {
        if (isFlipped || turning) return;
     //   if (!_CardGameManager.Instance.canClick()) return;
        Flip();
        StartCoroutine(SelectionEvent());
    }

    private IEnumerator SelectionEvent()
    {
        yield return new WaitForSeconds(0.5f);
       // _CardGameManager.Instance.cardClicked(spriteID, id);
    }
}
