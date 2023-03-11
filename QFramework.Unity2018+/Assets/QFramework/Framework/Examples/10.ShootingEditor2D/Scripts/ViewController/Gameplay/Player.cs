using QFramework;
using UnityEngine;

namespace ShootingEditor2D
{
    public class Player : ShootingEditor2DController
    {
        private Rigidbody2D mRigidbody2D;

        private Trigger2DCheck mGroundCheck;

        private Gun mGun;
        
        private void Awake()
        {
            mRigidbody2D = GetComponent<Rigidbody2D>();

            mGroundCheck = transform.Find("GroundCheck").GetComponent<Trigger2DCheck>();

            mGun = transform.Find("Gun").GetComponent<Gun>();
        }

        private bool mJumpPressed;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                mJumpPressed = true;
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                mGun.Shoot();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                mGun.Reload();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                this.SendCommand<ShiftGunCommand>();
            }
        }

        private void FixedUpdate()
        {
            var horizontalMovement = Input.GetAxis("Horizontal");

            if (horizontalMovement > 0 && transform.localScale.x < 0 ||
                horizontalMovement < 0 && transform.localScale.x > 0)
            {
                var localScale = transform.localScale;
                localScale.x = -localScale.x;
                transform.localScale = localScale;
            }

            mRigidbody2D.velocity = new Vector2(horizontalMovement * 5, mRigidbody2D.velocity.y);

            var grouded = mGroundCheck.Triggered;
            
            if (mJumpPressed && grouded)
            {
                mRigidbody2D.velocity = new Vector2(mRigidbody2D.velocity.x, 5);
            }

            mJumpPressed = false;
        }
    }
}
