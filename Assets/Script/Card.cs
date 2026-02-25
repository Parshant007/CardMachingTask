using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    private int spriteID;
    private int id;
    private bool isFlipped;
    private bool turning;
    [SerializeField]
    private Image img;


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
            ChangeSprite();
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
        if (!CardManager.Instance.canClick()) return;

        Flip();
        StartCoroutine(SelectionEvent());
    }

    private IEnumerator SelectionEvent()
    {
        yield return new WaitForSeconds(0.5f);
        CardManager.Instance.cardClicked(spriteID, id);
    }

    // spriteID getter and setter
    public int SpriteID
    {
        set
        {
            spriteID = value;
            isFlipped = true;
            ChangeSprite();
        }
        get { return spriteID; }
    }

    // card ID getter and setter
    public int ID
    {
        set { id = value; }
        get { return id; }
    }

    // toggle front/back sprite
    private void ChangeSprite()
    {
        if (spriteID == -1 || img == null) return;
        if (isFlipped)
            img.sprite = CardManager.Instance.GetSprite(spriteID);
        else
            img.sprite = CardManager.Instance.CardBack();
    }
    // call fade animation
    public void Inactive()
    {
        StartCoroutine(Fade());
    }
    // play fade animation by changing alpha of img's color
    private IEnumerator Fade()
    {
        float rate = 1.0f / 2.5f;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            img.color = Color.Lerp(img.color, Color.clear, t);

            yield return null;
        }
    }
    // set card to be active color
    public void Active()
    {
        if (img)
            img.color = Color.white;
    }
    // reset card default rotation
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        isFlipped = true;
    }

    public static int CardClick { get;  set; }
    public static int CardMatch { get;  set; }

}
