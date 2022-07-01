using UnityEngine;
using System.Collections;

public class PlayerController_m : MonoBehaviour
{
	[SerializeField] private FloatVariable moveSpeed = null;

	public float jumpVelocity = 50f;
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public GameObject Boost;
	public GameObject Cloud;
	private Rigidbody2D rb2d;
	private Animator anim;
	private bool isGrounded = false;


	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
	}


	void OnCollisionEnter2D(Collision2D collision2D)
	{

		if (collision2D.relativeVelocity.magnitude > 20)
		{
			Boost = Instantiate(Cloud, transform.position, transform.rotation) as GameObject;
		}
	}

	void Update()
	{
	
		if (isGrounded && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			//rb2d.AddForce(new Vector2(0, jumpForce));
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpVelocity);
		}
	}

	void FixedUpdate()
	{
		float speed = moveSpeed == null ? 5.0f : moveSpeed.value;
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15F, whatIsGround);

		anim.SetFloat("Speed", Mathf.Abs(speed));

		//rb2d.velocity = new Vector2(speed, rb2d.velocity.y);

		anim.SetBool("IsGrounded", isGrounded);

		anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		EndlessRunnerCollectable item = collision.GetComponent<EndlessRunnerCollectable>();
        if(item != null)
        {
			item.GetItem();
        }
    }



}