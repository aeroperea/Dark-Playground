using UnityEngine;

public class DogBehavior : MonoBehaviour, IInteractable
{
    GameObject player;
    AudioSource audio;
    public bool followsplayer;
    UnityEngine.AI.NavMeshAgent Nav;
    public AudioClip voiceline;
    public float followDistance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        followDistance = 10;
        Nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        audio = GetComponent<AudioSource>();
        player = GameManager.Instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (followsplayer)
        {
            Vector3 lineToPlayer = player.transform.position - transform.position;
            Vector3 directiontoplayer = lineToPlayer.normalized;
            Vector3 Destination = transform.position + lineToPlayer - directiontoplayer * followDistance;
            Debug.DrawRay(transform.position, Destination, Color.red);
            Nav.SetDestination(Destination);
            // Nav.SetDestination(transform.position + transform.forward);     
           
        }

    }

    public void OnInteract(PlayerInteraction playerinteraction)
    {
        followsplayer = true;
        audio.PlayOneShot(voiceline);
    }
}
