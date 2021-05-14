using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Birb : MonoBehaviour
{
    Camera mainCam ;
    Rigidbody2D rb;
    SpriteRenderer _spriteRenderer;
    Vector2 _initialPosition;
    bool _wasLaunched;
    private float _timeSittingAround;

    [SerializeField] float _launchPower = 500;
    [SerializeField] float _maxDragDistance = 5;
    private void Awake()
    {
        _initialPosition = transform.position;
        rb = this.GetComponent<Rigidbody2D>();
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
    }
    void Start()
    { 
        rb.isKinematic = true;
        mainCam = Camera.main;
    }
    void Update()
    {
        if(_wasLaunched && rb.velocity.magnitude <=0.1f)
        {
            _timeSittingAround += Time.deltaTime;
        }

        if(Mathf.Abs(transform.position.y) > 20 || Mathf.Abs(transform.position.x) > 20 )
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(3);
        rb.position = _initialPosition;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(ResetAfterDelay());
    }

    void OnMouseDown()
    {
        _spriteRenderer.color = Color.red;
    }

    void OnMouseUp()
    {
        _spriteRenderer.color = Color.white;
        Vector2 currentPosition = GetComponent<Rigidbody2D>().position;
        Vector2 directionToInitialPosition = (_initialPosition - currentPosition); // initial - current
        directionToInitialPosition.Normalize();
        rb.isKinematic = false;
        rb.AddForce(directionToInitialPosition * _launchPower);
        rb.gravityScale = 1;
        _wasLaunched = true;
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 desiredPosition = newPosition;

        float distance = Vector2.Distance(desiredPosition, _initialPosition); //get distance between the initial and the dragged cursor
        if (distance > _maxDragDistance)
        {
            Vector2 direction = (desiredPosition - _initialPosition); //if distance is greater than the permitted distnace
            direction.Normalize();                                             
            desiredPosition = _initialPosition + (direction * _maxDragDistance); //Then move the bird to the "FUTHEST ALLOWED" point
        }
        
        if (desiredPosition.x > _initialPosition.x)
            desiredPosition.x = _initialPosition.x;
        rb.position = desiredPosition;

        //transform.position = new Vector3(newPosition.x , newPosition.y, 0f );
    }

    
}
