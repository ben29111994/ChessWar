using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    public Color defaultColor;
    private Renderer meshRenderer;
    private MaterialPropertyBlock propBlock;
    public Color white;
    public GameObject fireworkPrefabs;
    private Vector3 offset;
    private int currentPoint;
    private List<int> scores = new List<int>();

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        propBlock = new MaterialPropertyBlock();

        if (meshRenderer == null)
            meshRenderer = GetComponent<Renderer>();
        offset = new Vector3(0, 1f, 0);
    }

    public void Check(Ball ball)
    {
        return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Ball ball = other.GetComponent<Ball>();

            if (ball.ballType.ToString() == "red")
            {
                //  ball.isAtHole = true;
                ball.Dead();
                GameManager.instance.Fail();
            }
            else if (ball.ballType.ToString() == "yellow")
            {
                currentPoint = (int)Mathf.Pow(2f, GameManager.instance.combo);

                GameManager.instance.InHoleEffect(transform.position + Vector3.up * 1.0f);
                GameManager.instance.scores.Add(currentPoint);


                GameManager.instance.count++;
                GameManager.instance.SetPoint(currentPoint);
                GameManager.instance.combo++;
                GameManager.instance.DisplayCombo(transform.position + Vector3.up * 4.0f);
                GameManager.instance.CheckComplete();

                ball.Hide(transform.position);
                // ball.Dead();
            }
        }
    }

    private IEnumerator EnableCollider(Ball ball)
    {
        yield return new WaitForSeconds(0.25f);
        ball.isMoving = false;
        ball.gameObject.SetActive(false);
    //    gameObject.layer = 8;
   //     gameObject.tag = "Hole";
    }

    public void SetBrickColor(bool _isBlack)
    {
        if (_isBlack)
        {
            meshRenderer.material = ColorManager.instance.material_Caro1;
        }
        else
        {
            meshRenderer.material = ColorManager.instance.material_Caro2;
        }
    }
}
