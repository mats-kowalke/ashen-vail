using System.Collections;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameObject content;
    public GameObject instance;
    public float dropForce;
    private new Rigidbody rigidbody;
    private Vector3 dropDirection;

    public void Start()
    {
        this.rigidbody = this.GetComponent<Rigidbody>();
    }

    public void SetContent(GameObject content)
    {
        this.tag = "Collectible";
        if (this.instance != null) Destroy(this.instance);
        GameObject instance = Instantiate(content);
        instance.transform.SetParent(this.gameObject.transform);
        instance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        this.content = content;
        this.instance = instance;
        this.content.layer = 9; //No Collision
    }

    public void Drop(Vector3 position, Vector3 direction)
    {
        this.transform.position = position;
        this.dropDirection = direction;
        this.StartCoroutine(DropRoutine());
    }

    public IEnumerator DropRoutine()
    {
        yield return new WaitForEndOfFrame();
        this.rigidbody.AddForce(this.dropDirection * this.dropForce);
    }

}
