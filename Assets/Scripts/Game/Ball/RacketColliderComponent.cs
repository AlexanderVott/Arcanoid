using RedDev.Game;
using RedDev.Kernel.Actors;
using TMPro;
using UnityEditor;
using UnityEngine;

[ComponentData(typeof(RigidBodyData))]
public class RacketColliderComponent : ComponentActor
{
	[SerializeField]
	private string tagRocket;

	[SerializeField]
	private float speed = 1f;

	private RigidBodyData _body;

	public override void OnSetup()
	{
		base.OnSetup();
	}

	public override void OnStart()
	{
		base.OnStart();
		_body = _actor.GetData<RigidBodyData>();
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == tagRocket)
		{
			float factor = GetHitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);
			Vector2 dir = new Vector2(factor, 1).normalized;
			_body.body.velocity = dir * speed;
		}
	}

	private float GetHitFactor(Vector2 selfPos, Vector2 racketPos, float racketWidth)
	{
		return (selfPos.x - racketPos.x) / racketWidth;
	}
}
