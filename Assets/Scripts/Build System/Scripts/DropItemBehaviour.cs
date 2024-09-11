using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody))]
public class DropItemBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] Collider col;
    
    private Rigidbody rb;

    public float upForce = 5;
    public float sideForceMax = 3;

    [SerializeField] ItemSO itemSO;

    [SerializeField] LayerMask playerLayer;

    public bool pickupOnTriggerEnter = false;

    void Awake()
    {
        transform.SetParent(null); // remove itself from the parent before getting deleted

        if (col == null)
        {
            col = GetComponent<Collider>();
        }
        col.isTrigger = false;
        rb = col.attachedRigidbody;

        playerLayer = LayerMask.GetMask("Player");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.constraints = RigidbodyConstraints.None;
        ApplyRandomForce();
    }


    void ApplyRandomForce()
    {
        Vector3 theforce = new Vector3(Random.Range(-sideForceMax, sideForceMax), upForce, Random.Range(-sideForceMax, sideForceMax));
        rb.AddForce(theforce, ForceMode.Impulse);
        print(theforce);
    }

    public void AddDropToInventory()
    {
        AudioSource.PlayClipAtPoint(itemSO.pickupSFX, transform.position);
        InventoryManager.Instance.AddItem(itemSO);
        Destroy(this.gameObject);
    }

    public void OnInteract(PlayerInteraction interactor)
    {
        AddDropToInventory();
    }

    void OnCollisionStay(Collision other)
    {
        if(rb.isKinematic != true && rb.linearVelocity.y < 0)
        {
            col.isTrigger = true;
            rb.isKinematic = true;
            StartCoroutine(OscilateUpAndDown());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name + " entered trigger");
        if (pickupOnTriggerEnter && ((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            AddDropToInventory();
        }
    }

    IEnumerator OscilateUpAndDown()
    {
        float amplitude = 0.25f;  // The maximum distance to move up and down from the starting position.
        float frequency = 4f;  // Speed of oscillation.
        Vector3 startPosition = transform.position + Vector3.up * amplitude;

        while (true)
        {
            // Calculate the new Y position using a sine wave.
            float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;  // Wait for the next frame.
        }
    }
}