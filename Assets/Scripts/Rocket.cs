using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Rocket : MonoBehaviour {

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float delay = 3f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] GameObject spider;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] AudioClip spiderDeathSFX;
    [SerializeField] GameObject button;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isAlive = true;
    bool collisionDisabled = false;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if(isAlive)
        { 
            RespondToMovingUpInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild) // work only when in BuildSettings "Development Build" in ON
        { 
            RespondToDebugKey();
        }

        if(Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isAlive || collisionDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Interact":
                StartCoroutine(DestroySpider());
                button.tag = "Friendly";
                button.GetComponent<ParticleSystem>().Stop();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    IEnumerator DestroySpider()
    {
        isAlive = false;
        audioSource.Stop();
        audioSource.PlayOneShot(spiderDeathSFX);
        explosionParticles.Play();
        Object.Destroy(spider, 1.5f);
        yield return new WaitForSeconds(1);
        isAlive = true;
    }

    private void StartDeathSequence()
    {
        isAlive = false;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", delay);
    }

    private void StartSuccessSequence()
    {
        isAlive = false;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", delay); // delaying loading next scene
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; // loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    void RespondToRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rotationThisFrame);
        }
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        //transform.Rotate(0, 0, 1 * rotationThisFrame);
        rigidBody.freezeRotation = false; //resume physics control of rotation
    }

    private void RespondToMovingUpInput()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyMovingUp();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyMovingUp()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust); // moving up
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToDebugKey() 
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; //toggle
        }
    }
}
