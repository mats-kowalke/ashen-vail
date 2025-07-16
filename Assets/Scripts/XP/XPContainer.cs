using UnityEngine;

public class XPContainer : MonoBehaviour
{
    public int XPAmount;
    public float XPAttraction;
    private GameObject player;

    private new Rigidbody rigidbody;

    public void Start()
    {
        this.rigidbody = this.transform.parent.GetComponent<Rigidbody>();
        this.player = GameObject.FindWithTag("PlayerTag");
    }

    public void Update()
    {
        Vector3 direction = this.transform.position - this.player.transform.position;
        float distance = direction.magnitude;
        direction.Normalize();
        this.rigidbody.AddForce(-direction * this.XPAttraction * distance);
    }


}