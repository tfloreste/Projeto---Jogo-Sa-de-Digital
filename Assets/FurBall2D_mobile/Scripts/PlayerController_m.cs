using UnityEngine;
using System.Collections;

public class PlayerController_m : MonoBehaviour
{
	[SerializeField] private FloatVariable moveSpeed = null;
	[SerializeField] private AudioClip moveSound;


	public float jumpVelocity = 50f;
	public Transform groundCheck;
	public LayerMask whatIsGround;
	public GameObject Boost;
	public GameObject Cloud;

	private Rigidbody2D rb2d;
	private Animator anim;
	private bool isGrounded = false;
	private bool shouldJump = false;

	private bool firstFrameSkipped = false;
	private AudioSource playerAudio;

	void Start()
	{
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		playerAudio = GetComponent<AudioSource>();
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
		if (!EndlessRunnerManager.Instance.gameStarted)
			return;
	
		// Pula o primeiro frame após o jogo iniciar para
		// que o touch que iniciou o jogo não cause um pulo
		if(!firstFrameSkipped)
        {
			firstFrameSkipped = true;

			if (playerAudio && moveSound)
				StartCoroutine(MoveSoundCO());

			return;
        }

		if (isGrounded && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			//rb2d.AddForce(new Vector2(0, jumpForce));
			shouldJump = true;
		}
	}

	void FixedUpdate()
	{
		float speed = moveSpeed == null ? 5.0f : moveSpeed.value;
		isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15F, whatIsGround);

		float verticalVelocity = rb2d.velocity.y;
		if (shouldJump)
        {
			verticalVelocity = jumpVelocity;
			shouldJump = false;
		}
		if(!Mathf.Approximately(verticalVelocity, rb2d.velocity.y) || !Mathf.Approximately(rb2d.velocity.x, 0.0f))
			rb2d.velocity = new Vector2(0.0f, verticalVelocity);

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

	private IEnumerator MoveSoundCO()
    {
		while (true)
        {
			if (isGrounded)
            {
				playerAudio.PlayOneShot(moveSound);
				yield return new WaitForSeconds(0.25f);
			}
			else
            {
				yield return null;
            }
				
		}
		
		
	}
}