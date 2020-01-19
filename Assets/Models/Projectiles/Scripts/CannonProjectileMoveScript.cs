using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectileMoveScript : ProjectileMoveScript {

	public GameObject cannon;
	protected override void Update () {
		base.Update();
		cannon.transform.Rotate(Vector3.up * speed * Time.deltaTime * 5);
	}

	protected override void OnCollisionEnter (Collision collision) {
		// spike
		// MenuManager.Instance._Mode = MenuManager.Mode.Game;

		if (!collision.collider.CompareTag("Projectile") && !collided) {
			collided = true;
			
			if (hitSFX != null) {
				audio.PlayOneShot (hitSFX);
			}

			if (trails.Count > 0) {
				for (int i = 0; i < trails.Count; i++) {
					trails [i].transform.parent = null;
					var ps = trails [i].GetComponent<ParticleSystem> ();
					if (ps != null) {
						ps.Stop ();
						Destroy (ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
					}
				}
			}
		
			speed = 0;
			GetComponent<Rigidbody> ().isKinematic = true;

			ContactPoint contact = collision.contacts [0];
			Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
			Vector3 pos = contact.point;

			if (hitPrefab != null) {
				var hitVFX = Instantiate (hitPrefab, pos, rot);
				var ps = hitVFX.GetComponent<ParticleSystem> ();
				if (ps == null) {
					var psChild = hitVFX.transform.GetChild (0).GetComponent<ParticleSystem> ();
					Destroy (hitVFX, psChild.main.duration);
				} else
					Destroy (hitVFX, ps.main.duration);
			}

			StartCoroutine (DestroyParticle (0f));
		}
	}

	public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		
		yield return new WaitForSeconds (waitTime);
		Destroy (gameObject);
	}
}