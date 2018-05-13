using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterMotorMovement : object
{
	public float maxForwardSpeed;
	public float maxSidewaysSpeed;
	public float maxBackwardsSpeed;
	public AnimationCurve slopeSpeedMultiplier;
	public float maxGroundAcceleration;
	public float maxAirAcceleration;
	public float gravity;
	public float maxFallSpeed;
	[System.NonSerialized]
	public CollisionFlags collisionFlags;
	[System.NonSerialized]
	public Vector3 velocity;
	[System.NonSerialized]
	public Vector3 frameVelocity;
	[System.NonSerialized]
	public Vector3 hitPoint;
	[System.NonSerialized]
	public Vector3 lastHitPoint;
	public CharacterMotorMovement()
	{
		this.maxForwardSpeed = 10f;
		this.maxSidewaysSpeed = 10f;
		this.maxBackwardsSpeed = 10f;
		this.slopeSpeedMultiplier = new AnimationCurve(new Keyframe[] {new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0)});
		this.maxGroundAcceleration = 30f;
		this.maxAirAcceleration = 20f;
		this.gravity = 10f;
		this.maxFallSpeed = 20f;
		this.frameVelocity = Vector3.zero;
		this.hitPoint = Vector3.zero;
		this.lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
	}
}

public enum MovementTransferOnJump
{
	None = 0, 
	InitTransfer = 1, 
	PermaTransfer = 2, 
	PermaLocked = 3 
}

[System.Serializable]
public class CharacterMotorJumping : object
{
	public bool enabled;
	public float baseHeight;
	public float extraHeight;
	public float perpAmount;
	public float steepPerpAmount;
	[System.NonSerialized]
	public bool jumping;
	[System.NonSerialized]
	public bool holdingJumpButton;
	[System.NonSerialized]
	public float lastStartTime;
	[System.NonSerialized]
	public float lastButtonDownTime;
	[System.NonSerialized]
	public Vector3 jumpDir;
	public CharacterMotorJumping()
	{
		this.enabled = true;
		this.baseHeight = 1f;
		this.extraHeight = 4.1f;
		this.steepPerpAmount = 0.5f;
		this.lastButtonDownTime = -100;
		this.jumpDir = Vector3.up;
	}
}

[System.Serializable]
public class CharacterMotorMovingPlatform : object
{
	public bool enabled;
	public MovementTransferOnJump movementTransfer;
	[System.NonSerialized]
	public Transform hitPlatform;
	[System.NonSerialized]
	public Transform activePlatform;
	[System.NonSerialized]
	public Vector3 activeLocalPoint;
	[System.NonSerialized]
	public Vector3 activeGlobalPoint;
	[System.NonSerialized]
	public Quaternion activeLocalRotation;
	[System.NonSerialized]
	public Quaternion activeGlobalRotation;
	[System.NonSerialized]
	public Matrix4x4 lastMatrix;
	[System.NonSerialized]
	public Vector3 platformVelocity;
	[System.NonSerialized]
	public bool newPlatform;
	public CharacterMotorMovingPlatform()
	{
		this.enabled = true;
		this.movementTransfer = MovementTransferOnJump.PermaTransfer;
	}
}

[System.Serializable]
public class CharacterMotorSliding : object
{
	public bool enabled;
	public float slidingSpeed;
	public float sidewaysControl;
	public float speedControl;
	public CharacterMotorSliding()
	{
		this.enabled = true;
		this.slidingSpeed = 15;
		this.sidewaysControl = 1f;
		this.speedControl = 0.4f;
	}
}

[System.Serializable]
[UnityEngine.RequireComponent(typeof(CharacterController))]
[UnityEngine.AddComponentMenu("Character/Character Motor")]
public partial class CharacterMotor : MonoBehaviour
{
	public bool canControl;
	public bool useFixedUpdate;
	[System.NonSerialized]
	public Vector3 inputMoveDirection;
	[System.NonSerialized]
	public bool inputJump;
	public CharacterMotorMovement movement;
	public CharacterMotorJumping jumping;
	public CharacterMotorMovingPlatform movingPlatform;
	public CharacterMotorSliding sliding;
	[System.NonSerialized]
	public bool grounded;
	[System.NonSerialized]
	public Vector3 groundNormal;
	private Vector3 lastGroundNormal;
	private Transform tr;
	private CharacterController controller;
	public virtual void Awake()
    {
		this.controller = (CharacterController) this.GetComponent(typeof(CharacterController));
		this.tr = this.transform;
	}

	private void UpdateFunction()
	{
		Vector3 velocity = this.movement.velocity;
		velocity = this.ApplyInputVelocityChange(velocity);
		velocity = this.ApplyGravityAndJumping(velocity);
		Vector3 moveDistance = Vector3.zero;
		if (this.MoveWithPlatform())
		{
			Vector3 newGlobalPoint = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint);
			moveDistance = newGlobalPoint - this.movingPlatform.activeGlobalPoint;
			if (moveDistance != Vector3.zero)
			{
				this.controller.Move(moveDistance);
			}
			Quaternion newGlobalRotation = this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation;
			Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation);
			float yRotation = rotationDiff.eulerAngles.y;
			if (yRotation != 0)
			{
				this.tr.Rotate(0, yRotation, 0);
			}
		}
		Vector3 lastPosition = this.tr.position;
		Vector3 currentMovementOffset = velocity * Time.deltaTime;
		float pushDownOffset = Mathf.Max(this.controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
		if (this.grounded)
		{
			currentMovementOffset = currentMovementOffset - (pushDownOffset * Vector3.up);
		}
		this.movingPlatform.hitPlatform = null;
		this.groundNormal = Vector3.zero;
		this.movement.collisionFlags = this.controller.Move(currentMovementOffset);
		this.movement.lastHitPoint = this.movement.hitPoint;
		this.lastGroundNormal = this.groundNormal;
		if (this.movingPlatform.enabled && (this.movingPlatform.activePlatform != this.movingPlatform.hitPlatform))
		{
			if (this.movingPlatform.hitPlatform != null)
			{
				this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
				this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
				this.movingPlatform.newPlatform = true;
			}
		}
		Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
		this.movement.velocity = (this.tr.position - lastPosition) / Time.deltaTime;
		Vector3 newHVelocity = new Vector3(this.movement.velocity.x, 0, this.movement.velocity.z);
		if (oldHVelocity == Vector3.zero)
		{
			this.movement.velocity = new Vector3(0, this.movement.velocity.y, 0);
		}
		else
		{
			float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
			this.movement.velocity = (oldHVelocity * Mathf.Clamp01(projectedNewVelocity)) + (this.movement.velocity.y * Vector3.up);
		}
		if (this.movement.velocity.y < (velocity.y - 0.001f))
		{
			if (this.movement.velocity.y < 0)
			{
				this.movement.velocity.y = velocity.y;
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		if (this.grounded && !this.IsGroundedTest())
		{
			this.grounded = false;
			if (this.movingPlatform.enabled && ((this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
			{
				this.movement.frameVelocity = this.movingPlatform.platformVelocity;
				this.movement.velocity = this.movement.velocity + this.movingPlatform.platformVelocity;
			}
			this.SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			this.tr.position = this.tr.position + (pushDownOffset * Vector3.up);
		}
		else
		{
			if (!this.grounded && this.IsGroundedTest())
			{
				this.grounded = true;
				this.jumping.jumping = false;
				this.StartCoroutine(this.SubtractNewPlatformVelocity());
				this.SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
			}
		}
		if (this.MoveWithPlatform())
		{
			this.movingPlatform.activeGlobalPoint = this.tr.position + (Vector3.up * ((this.controller.center.y - (this.controller.height * 0.5f)) + this.controller.radius));
			this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
			this.movingPlatform.activeGlobalRotation = this.tr.rotation;
			this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
		}
	}

	public virtual void FixedUpdate()
	{
		if (this.movingPlatform.enabled)
		{
			if (this.movingPlatform.activePlatform != null)
			{
				if (!this.movingPlatform.newPlatform)
				{
					this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / Time.deltaTime;
				}
				this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
				this.movingPlatform.newPlatform = false;
			}
			else
			{
				this.movingPlatform.platformVelocity = Vector3.zero;
			}
		}
		if (this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}

	public virtual void Update()
	{
		if (!this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		Vector3 desiredVelocity = default(Vector3);
		if (!this.canControl)
		{
			this.inputMoveDirection = Vector3.zero;
		}
		if (this.grounded && this.TooSteep())
		{
			desiredVelocity = new Vector3(this.groundNormal.x, 0, this.groundNormal.z).normalized;
			Vector3 projectedMoveDir = Vector3.Project(this.inputMoveDirection, desiredVelocity);
			desiredVelocity = (desiredVelocity + (projectedMoveDir * this.sliding.speedControl)) + ((this.inputMoveDirection - projectedMoveDir) * this.sliding.sidewaysControl);
			desiredVelocity = desiredVelocity * this.sliding.slidingSpeed;
		}
		else
		{
			desiredVelocity = this.GetDesiredHorizontalVelocity();
		}
		if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
		{
			desiredVelocity = desiredVelocity + this.movement.frameVelocity;
			desiredVelocity.y = 0;
		}
		if (this.grounded)
		{
			desiredVelocity = this.AdjustGroundVelocityToNormal(desiredVelocity, this.groundNormal);
		}
		else
		{
			velocity.y = 0;
		}
		float maxVelocityChange = this.GetMaxAcceleration(this.grounded) * Time.deltaTime;
		Vector3 velocityChangeVector = desiredVelocity - velocity;
		if (velocityChangeVector.sqrMagnitude > (maxVelocityChange * maxVelocityChange))
		{
			velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
		}
		if (this.grounded || this.canControl)
		{
			velocity = velocity + velocityChangeVector;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min(velocity.y, 0);
		}
		return velocity;
	}

	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		if (!this.inputJump || !this.canControl)
		{
			this.jumping.holdingJumpButton = false;
			this.jumping.lastButtonDownTime = -100;
		}
		if ((this.inputJump && (this.jumping.lastButtonDownTime < 0)) && this.canControl)
		{
			this.jumping.lastButtonDownTime = Time.time;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min(0, velocity.y) - (this.movement.gravity * Time.deltaTime);
		}
		else
		{
			velocity.y = this.movement.velocity.y - (this.movement.gravity * Time.deltaTime);
			if (this.jumping.jumping && this.jumping.holdingJumpButton)
			{
				if (Time.time < (this.jumping.lastStartTime + (this.jumping.extraHeight / this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))))
				{
					velocity = velocity + ((this.jumping.jumpDir * this.movement.gravity) * Time.deltaTime);
				}
			}
			velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
		}
		if (this.grounded)
		{
			if ((this.jumping.enabled && this.canControl) && ((Time.time - this.jumping.lastButtonDownTime) < 0.2f))
			{
				this.grounded = false;
				this.jumping.jumping = true;
				this.jumping.lastStartTime = Time.time;
				this.jumping.lastButtonDownTime = -100;
				this.jumping.holdingJumpButton = true;
				if (this.TooSteep())
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.steepPerpAmount);
				}
				else
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.perpAmount);
				}
				velocity.y = 0;
				velocity = velocity + (this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight));
				if (this.movingPlatform.enabled && ((this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
				{
					this.movement.frameVelocity = this.movingPlatform.platformVelocity;
					velocity = velocity + this.movingPlatform.platformVelocity;
				}
				this.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		return velocity;
	}

	public virtual void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (((hit.normal.y > 0) && (hit.normal.y > this.groundNormal.y)) && (hit.moveDirection.y < 0))
		{
			if (((hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001f) || (this.lastGroundNormal == Vector3.zero))
			{
				this.groundNormal = hit.normal;
			}
			else
			{
				this.groundNormal = this.lastGroundNormal;
			}
			this.movingPlatform.hitPlatform = hit.collider.transform;
			this.movement.hitPoint = hit.point;
			this.movement.frameVelocity = Vector3.zero;
		}
	}

	private IEnumerator SubtractNewPlatformVelocity()
	{
		if (this.movingPlatform.enabled && ((this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer) || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)))
		{
			if (this.movingPlatform.newPlatform)
			{
				Transform platform = this.movingPlatform.activePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if (this.grounded && (platform == this.movingPlatform.activePlatform))
				{
					yield return 1;
				}
			}
			this.movement.velocity = this.movement.velocity - this.movingPlatform.platformVelocity;
		}
	}

	private bool MoveWithPlatform()
	{
		return (this.movingPlatform.enabled && (this.grounded || (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked))) && (this.movingPlatform.activePlatform != null);
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		Vector3 desiredLocalDirection = this.tr.InverseTransformDirection(this.inputMoveDirection);
		float maxSpeed = this.MaxSpeedInDirection(desiredLocalDirection);
		if (this.grounded)
		{
			float movementSlopeAngle = Mathf.Asin(this.movement.velocity.normalized.y) * Mathf.Rad2Deg;
			maxSpeed = maxSpeed * this.movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
		}
		return this.tr.TransformDirection(desiredLocalDirection * maxSpeed);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	}

	private bool IsGroundedTest()
	{
		return this.groundNormal.y > 0.01f;
	}

	public virtual float GetMaxAcceleration(bool grounded)
	{
		if (grounded)
		{
			return this.movement.maxGroundAcceleration;
		}
		else
		{
			return this.movement.maxAirAcceleration;
		}
	}

	public virtual float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt((2 * targetJumpHeight) * this.movement.gravity);
	}

	public virtual bool IsJumping()
	{
		return this.jumping.jumping;
	}

	public virtual bool IsSliding()
	{
		return (this.grounded && this.sliding.enabled) && this.TooSteep();
	}

	public virtual bool IsTouchingCeiling()
	{
		return (this.movement.collisionFlags & CollisionFlags.CollidedAbove) != (CollisionFlags) 0;
	}

	public virtual bool IsGrounded()
	{
		return this.grounded;
	}

	public virtual bool TooSteep()
	{
		return this.groundNormal.y <= Mathf.Cos(this.controller.slopeLimit * Mathf.Deg2Rad);
	}

	public virtual Vector3 GetDirection()
	{
		return this.inputMoveDirection;
	}

	public virtual void SetControllable(bool controllable)
	{
		this.canControl = controllable;
	}

	public virtual float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		if (desiredMovementDirection == Vector3.zero)
		{
			return 0;
		}
		else
		{
			float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? this.movement.maxForwardSpeed : this.movement.maxBackwardsSpeed) / this.movement.maxSidewaysSpeed;
			Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
			float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * this.movement.maxSidewaysSpeed;
			return length;
		}
	}

	public virtual void SetVelocity(Vector3 velocity)
	{
		this.grounded = false;
		this.movement.velocity = velocity;
		this.movement.frameVelocity = Vector3.zero;
		this.SendMessage("OnExternalVelocity");
	}

	public CharacterMotor()
	{
		this.canControl = true;
		this.useFixedUpdate = true;
		this.inputMoveDirection = Vector3.zero;
		this.movement = new CharacterMotorMovement();
		this.jumping = new CharacterMotorJumping();
		this.movingPlatform = new CharacterMotorMovingPlatform();
		this.sliding = new CharacterMotorSliding();
		this.grounded = true;
		this.groundNormal = Vector3.zero;
		this.lastGroundNormal = Vector3.zero;
	}
}