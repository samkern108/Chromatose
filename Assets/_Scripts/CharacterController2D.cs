#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;


namespace Prime31 {

[RequireComponent( typeof( BoxCollider2D ), typeof( Rigidbody2D ) )]
public class CharacterController2D : MonoBehaviour
{
	#region internal types

	struct CharacterRaycastOrigins
	{
		public Vector3 topLeft, bottomRight, bottomLeft;
	}

	public class CharacterCollisionState2D
	{
		public bool right, left, above, below;
		public bool becameGroundedThisFrame, wasGroundedLastFrame;


		public bool hasCollision()
		{
			return below || right || left || above;
		}


		public void reset()
		{
			right = left = above = below = becameGroundedThisFrame = false;
		}


		public override string ToString()
		{
			return string.Format( "[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, wasGroundedLastFrame: {4}, becameGroundedThisFrame: {5}",
			                     right, left, above, below, wasGroundedLastFrame, becameGroundedThisFrame );
		}
	}

	#endregion


	#region events, properties and fields

	[SerializeField]
	[Range( 0.001f, 0.3f )]
	float _skinWidth = 0.02f;

	/// <summary>
	/// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
	/// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
	/// </summary>
	public float skinWidth
	{
		get { return _skinWidth; }
		set
		{
			_skinWidth = value;
			recalculateDistanceBetweenRays();
		}
	}


	/// <summary>
	/// mask with all layers that the player should interact with
	/// </summary>
	public LayerMask platformMask = 0;

	/// <summary>
	/// mask with all layers that trigger events should fire when intersected
	/// </summary>
	public LayerMask triggerMask = 0;


	[Range( 2, 20 )]
	public int totalHorizontalRays = 8;
	[Range( 2, 20 )]
	public int totalVerticalRays = 4;

	public Vector3 downDirection = Vector3.down;


	[HideInInspector][NonSerialized]
	public new Transform transform;
	[HideInInspector][NonSerialized]
	public BoxCollider2D boxCollider;
	[HideInInspector][NonSerialized]
	public Rigidbody2D rigidBody2D;

	[HideInInspector][NonSerialized]
	public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
	[HideInInspector][NonSerialized]
	public Vector3 velocity;
	[HideInInspector][NonSerialized]
	public Vector3 unrotatedVelocity;
	public bool isGrounded { get { return collisionState.below; } }

	const float kSkinWidthFloatFudgeFactor = 0.001f;

	#endregion


	/// <summary>
	/// holder for our raycast origin corners (TR, TL, BR, BL)
	/// </summary>
	CharacterRaycastOrigins _raycastOrigins;

	/// <summary>
	/// stores our raycast hit during movement
	/// </summary>
	RaycastHit2D _raycastHit;

	/// <summary>
	/// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
	/// horizontally and vertically so that we can send the events after all collision state is set
	/// </summary>
	List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>( 2 );

	// horizontal/vertical movement data
	float _verticalDistanceBetweenRays;
	float _horizontalDistanceBetweenRays;


	#region Monobehaviour

	void Awake()
	{
		// cache some components
		transform = GetComponent<Transform>();
		boxCollider = GetComponent<BoxCollider2D>();
		rigidBody2D = GetComponent<Rigidbody2D>();

		// here, we trigger our properties that have setters with bodies
		skinWidth = _skinWidth;

		// we want to set our CC2D to ignore all collision layers except what is in our triggerMask
		for( var i = 0; i < 32; i++ )
		{
			// see if our triggerMask contains this layer and if not ignore it
			if( ( triggerMask.value & 1 << i ) == 0 )
				Physics2D.IgnoreLayerCollision( gameObject.layer, i );
		}
	}

	#endregion


	[System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
	void DrawRay( Vector3 start, Vector3 dir, Color color )
	{
		Debug.DrawRay( start, dir, color );
	}


	#region Public

	/// <summary>
	/// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
	/// stop when run into.
	/// </summary>
	/// <param name="deltaMovement">Delta movement.</param>
	public void move( Vector3 deltaMovement, float downAngle )
	{
		deltaMovement = deltaMovement.Rotate2D(downAngle);
		downDirection = Vector3.down.Rotate2D(downAngle);

		// save off our current grounded state which we will use for wasGroundedLastFrame and becameGroundedThisFrame
		collisionState.wasGroundedLastFrame = collisionState.below;

		// clear our state
		collisionState.reset();
		_raycastHitsThisFrame.Clear();

		primeRaycastOrigins();

		// now we check movement in the horizontal dir
		if( deltaMovement.x != 0f )
			moveHorizontally( ref deltaMovement );

		// next, check movement in the vertical dir
		if( deltaMovement.y != 0f )
			moveVertically( ref deltaMovement );

		// move then update our state
		deltaMovement.z = 0;
		transform.Translate( deltaMovement, Space.World );

		// only calculate velocity if we have a non-zero deltaTime
		if( Time.deltaTime > 0f ) {
			velocity = deltaMovement / Time.deltaTime;
			deltaMovement = deltaMovement.Rotate2D(360 - downAngle);
			unrotatedVelocity = deltaMovement / Time.deltaTime;
		}

		// set our becameGrounded state based on the previous and current collision state
		if( !collisionState.wasGroundedLastFrame && collisionState.below )
			collisionState.becameGroundedThisFrame = true;
	}


	/// <summary>
	/// moves directly down until grounded
	/// </summary>
	public void warpToGrounded()
	{
		do
		{
			move( Vector3.down, 0 );
		} while( !isGrounded );
	}


	/// <summary>
	/// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
	/// It is also used in the skinWidth setter in case it is changed at runtime.
	/// </summary>
	public void recalculateDistanceBetweenRays()
	{
		// figure out the distance between our rays in both directions
		// horizontal
		var colliderUseableHeight = boxCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
		_verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

		// vertical
		var colliderUseableWidth = boxCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
		_horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
	}

	#endregion


	#region Movement Methods

	/// <summary>
	/// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
	/// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
	/// </summary>
	void primeRaycastOrigins()
	{
		// our raycasts need to be fired from the bounds inset by the skinWidth
		var modifiedBounds = boxCollider.bounds;
		modifiedBounds.Expand( -2f * _skinWidth );

		var extents = modifiedBounds.extents;
        var center = modifiedBounds.center;

		/*_raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
		_raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
		_raycastOrigins.bottomLeft = modifiedBounds.min;*/

		/*Vector3 topLeftOrientation = transform.up - transform.right;
        Vector3 bottomLeftOrientation = -transform.up - transform.right;
        Vector3 topLeftSignedExtents = new Vector3(extents.x * topLeftOrientation.x, extents.y * topLeftOrientation.y);
        Vector3 bottomLeftSignedExtents = new Vector3(extents.x * bottomLeftOrientation.x, extents.y * bottomLeftOrientation.y);
*/
		Vector3 topLeftOrientation = -downDirection - downDirection.Rotate2D(90);
        Vector3 bottomLeftOrientation = downDirection - downDirection.Rotate2D(90);
        Vector3 topLeftSignedExtents = new Vector3(extents.x * topLeftOrientation.x, extents.y * topLeftOrientation.y);
        Vector3 bottomLeftSignedExtents = new Vector3(extents.x * bottomLeftOrientation.x, extents.y * bottomLeftOrientation.y);

        _raycastOrigins.topLeft = center + topLeftSignedExtents;
        _raycastOrigins.bottomRight = center - topLeftSignedExtents;
        _raycastOrigins.bottomLeft = center + bottomLeftSignedExtents;
	}


	/// <summary>
	/// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
	/// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
	/// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
	/// actually moving the player
	/// </summary>
	void moveHorizontally( ref Vector3 deltaMovement )
	{
		Vector2 rightDirection = (Vector2)downDirection.Rotate2D(90);
        var isGoingRight = (rightDirection.x * deltaMovement.x) > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + _skinWidth;
        var rayDirection = isGoingRight ? rightDirection : -rightDirection;
		var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

		for( var i = 0; i < totalHorizontalRays; i++ )
		{
			//var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays );
            float rayOffset = i * _verticalDistanceBetweenRays;
            var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);
            ray += (Vector2)(-downDirection) * rayOffset;

			DrawRay( ray, rayDirection, Color.green );
			//DrawRay( ray, rayDirection * rayDistance, Color.red );

			_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask );

			if( _raycastHit )
			{
				// set our new deltaMovement and recalculate the rayDistance taking it into account
				deltaMovement.x = _raycastHit.point.x - ray.x;
				rayDistance = Mathf.Abs( deltaMovement.x );

				// remember to remove the skinWidth from our deltaMovement
				if( isGoingRight )
				{
					deltaMovement.x -= _skinWidth * rightDirection.x;
					collisionState.right = true;
				}
				else
				{
					deltaMovement.x += _skinWidth * rightDirection.x;
					collisionState.left = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );

				// we add a small fudge factor for the float operations here. if our rayDistance is smaller
				// than the width + fudge bail out because we have a direct impact
				if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					break;
			}
		}
	}

	void moveVertically( ref Vector3 deltaMovement )
	{
		float upSign = -downDirection.y;
		var isGoingUp = deltaMovement.y * upSign > 0;
		var rayDistance = Mathf.Abs( deltaMovement.y ) + _skinWidth;
		var rayDirection = isGoingUp ? -downDirection : downDirection;
		var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

		// apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
		initialRayOrigin.x += deltaMovement.x;

		for( var i = 0; i < totalVerticalRays; i++ )
		{
			//var ray = new Vector2( initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y );

			float rayOffset = i * _horizontalDistanceBetweenRays;
            var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);
            ray += rayOffset * (Vector2)downDirection.Rotate2D(90);

			DrawRay( ray, rayDirection, Color.yellow );
			//DrawRay( ray, rayDirection * rayDistance, Color.red );
			_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask );
			if( _raycastHit )
			{
				// set our new deltaMovement and recalculate the rayDistance taking it into account
				deltaMovement.y = _raycastHit.point.y - ray.y;
				rayDistance = Mathf.Abs( deltaMovement.y );

				// remember to remove the skinWidth from our deltaMovement
				if( isGoingUp )
				{
					deltaMovement.y -= _skinWidth * upSign;
					collisionState.above = true;
				}
				else
				{
					deltaMovement.y += _skinWidth * upSign;
					collisionState.below = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );

				// we add a small fudge factor for the float operations here. if our rayDistance is smaller
				// than the width + fudge bail out because we have a direct impact
				if( rayDistance < _skinWidth + kSkinWidthFloatFudgeFactor )
					break;
			}
		}
	}

	#endregion

	void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_raycastOrigins.topLeft, .05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_raycastOrigins.bottomLeft, .05f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_raycastOrigins.bottomRight, .05f);
    }

}}