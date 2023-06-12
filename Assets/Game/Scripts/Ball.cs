using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private bool isHide;
    private Rigidbody rigidbody;
    private Collider collider;

    public LayerMask layer;
    public LayerMask layerBrick;

    public BallType ballType;

    public enum BallType
    {
        red,
        yellow
    }

    public Color paintColor;
    public bool isMoving;

    // public Animator animator;
    public Collider colliderBrick;

    Coroutine moveCoroutine;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        GameManager.enableCollider += ActiveColliderBrick;
        GameManager.disableCollider += DisableColliderBrick;
        GameManager.touchSwipe += Move;
    }

    private void OnDisable()
    {
        GameManager.enableCollider -= ActiveColliderBrick;
        GameManager.disableCollider -= DisableColliderBrick;
        GameManager.touchSwipe -= Move;
    }

    public void Init(Vector3 _position)
    {
        isHide = false;
        colliderBrick = null;
        transform.eulerAngles = Vector3.zero;
        collider.enabled = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;


        _position.y = 1.0f;
        transform.position = _position;
        //  StartCoroutine(C_Init(_position));
    }

    private IEnumerator C_Init(Vector3 _position)
    {
        isHide = false;
        colliderBrick = null;
        transform.eulerAngles = Vector3.zero;
        collider.enabled = true;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;

        yield return null;

        _position.y = 1.0f;
        transform.position = _position;
    }

    public void Move()
    {
        if (gameObject.activeSelf == false) return;

        moveCoroutine = StartCoroutine(C_Move(GameManager.instance.directionSwipe));
    }

    private IEnumerator C_Move(Vector3 direction)
    {
        GameManager.instance._maxD++;

        yield return null;
        isMoving = true;

        bool isExplosion = false;
        bool isMoveAnim = false;


        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, 20.0f, layer);

        var multiHitInfo = hits;
        System.Array.Sort(multiHitInfo, (x, y) => x.distance.CompareTo(y.distance));
        int n = 0;

        foreach (var hit in multiHitInfo)
        {
            if (hit.collider.CompareTag("Brick") || hit.collider.CompareTag("Hole"))
            {
                n++;

                if (hit.collider.CompareTag("Hole"))
                {
                    n += 20;
                    break;
                }
            }
            else if (hit.collider.CompareTag("Tile"))
            {
                break;
            }
        }

        if (n > 0)
        {
            // if (isMoveAnim == false)
            // {
            //     transform.rotation = Quaternion.LookRotation(direction);

            //     isMoveAnim = true;
            //      animator.SetTrigger("Move");
            // }

            isExplosion = true;

            float t = 0.0f;

            Vector3 fromPosition = transform.position;
            Vector3 toPosition = fromPosition + direction * n;

            while (t < 1)
            {
                t += (0.5f) / n;
                transform.position = Vector3.Lerp(fromPosition, toPosition, t);

                yield return null;
            }

            transform.position = toPosition;

        }

        // if (isExplosion)
        // {
        // animator.SetTrigger("Idle");

        //if (ballType == BallType.red)
        //{
        //    GameManager.instance.RedExplosion(transform.position);
        //}
        //else
        //{
        //    GameManager.instance.YellowExplosion(transform.position);
        //}
        // }

        isMoving = false;
        GameManager.instance.ActiveColliderBrick();
    }



    public void ActiveColliderBrick()
    {
        if (colliderBrick != null)
        {
            colliderBrick.enabled = true;
            if (isHide)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void DisableColliderBrick()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 10, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerBrick))
        {
            if (hit.collider != null)
            {
                colliderBrick = hit.collider.gameObject.GetComponent<Collider>();
                colliderBrick.enabled = false;
            }
        }
    }

    public void Hide(Vector3 _posHole)
    {
        StopAllCoroutines();

        StartCoroutine(C_Hide(_posHole));
    }

    private IEnumerator C_Hide(Vector3 _posHole)
    {

        Vector3 _pos = transform.position;
        _pos.x = _posHole.x;
        _pos.z = _posHole.z;
        transform.position = _pos;

        isHide = true;
        isMoving = false;
        collider.enabled = false;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.AddForce(Vector3.down * 500);

        yield return new WaitForSeconds(0.2f);
        GameManager.instance.ActiveColliderBrick();
    }

    public void Dead()
    {
        StopAllCoroutines();
        Jump();
    }

    public void Jump()
    {
        isMoving = false;
        // collider.enabled = false;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.AddTorque(new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10)));
        rigidbody.AddForce(new Vector3(Random.Range(-300, 300), Random.Range(500, 1000), Random.Range(-300, 300)));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Base"))
        {
            print("Sleep");
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
        }
    }
}
