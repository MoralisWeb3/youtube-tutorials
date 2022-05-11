
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Hero : MonoBehaviour
{
	public float m_MoveSpeed;
    public Animator animator;

    Rigidbody2D rb;

    public enum PlayerState { Alive, Dead }
    public PlayerState playerState = PlayerState.Alive;
    public Vector2 lookFacing;
	public Vector2 respawnPoint;
    AudioSource audioSource;
    float dashCooldown = 0f;
    public bool dead = false;

	void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator.SetBool("alive", true);
        audioSource = GetComponent<AudioSource>();
    }
	void Update () 
	{
        if(playerState == PlayerState.Dead) {
            rb.velocity = Vector2.zero;
            return;
        }

		Vector3 tryMove = Vector3.zero;
		
		if (Input.GetKey(KeyCode.LeftArrow))
			tryMove += Vector3Int.left;
		if (Input.GetKey(KeyCode.RightArrow))
			tryMove += Vector3Int.right;
		if (Input.GetKey(KeyCode.UpArrow))
			tryMove += Vector3Int.up;
		if (Input.GetKey(KeyCode.DownArrow))
			tryMove += Vector3Int.down;

        rb.velocity = Vector3.ClampMagnitude(tryMove, 1f) * m_MoveSpeed;
        animator.SetBool("moving", tryMove.magnitude > 0);
        if (Mathf.Abs(tryMove.x) > 0) {
            animator.transform.localScale = tryMove.x < 0f ? new Vector3(-1f, 1f, 1f) : new Vector3(1f, 1f, 1f);
        }
        if(tryMove.magnitude > 0f) {
            lookFacing = tryMove;
        }

        dashCooldown = Mathf.MoveTowards(dashCooldown, 0f, Time.deltaTime);

        if (Input.GetButtonDown("Jump")) {
            if(dashCooldown <= 0f && tryMove.magnitude > 0) {

                var hit = Physics2D.Raycast(transform.position + Vector3.up * .5f, tryMove.normalized, 3.5f, 1 << LayerMask.NameToLayer("Wall"));

                float distance = 3f;
                if(hit.collider != null) {
                    distance = hit.distance - .5f;
                }

                transform.position = rb.position + Vector2.ClampMagnitude(tryMove, 1f) * distance;

                if (audioSource != null) audioSource.Play();
            }
        }

        animator.SetBool("dash_ready", dashCooldown <= 0f);

	}

    public void LevelComplete() {
        Destroy(gameObject);
    }
}
