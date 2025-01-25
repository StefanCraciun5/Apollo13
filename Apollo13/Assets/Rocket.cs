#define TST
  

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    #region my defaults
    [SerializeField] float rcsThrust = 140f;
    [SerializeField] float powerThrust = 15000f;
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] int currentLevel = 0;
    
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip transcendingSound;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem succesParticles;

    const float defaultDrag = 0.75f;
    const float dragStep = 0.05f;
    const float dragMax = 1.5f;
    const float dragMin = 0.1f;
    #endregion

    AudioSource audioSource;
    Rigidbody rigidBody;
    enum State {Alive, Dying, Transcending, Debug}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        print("Debug state: " + Debug.isDebugBuild);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Debug:
            case State.Alive:
                RespondToThrustInput();
                RespondToRotateInput();
                //if (Debug.isDebugBuild)
#if TST
                {
                    RespondToDebugInput();
                }
#endif
                Drag();
                break;
            case State.Dying:              
                break;
            case State.Transcending:
                break;                          
            default:
                break;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if ((state != State.Alive) &
            (state != State.Debug))
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccesSequence();
                break;
            default:
                if (state != State.Debug)
                {
                    StartDeadSequence();
                }
                break;
        }
    }

    private void StartDeadSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("ReloadScene", levelLoadDelay);
    }

    private void StartSuccesSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(transcendingSound);
        succesParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void ReloadScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {        
        if (Input.GetKey(KeyCode.UpArrow) | Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            engineParticles.Stop();
        }        
    }

    private void ApplyThrust()
    {
        float powerThisFrame = powerThrust * Time.deltaTime;

        rigidBody.AddRelativeForce(Vector3.up * powerThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        engineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }            
    }

    private void Drag()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            rigidBody.drag = Math.Min(rigidBody.drag + dragStep, dragMax);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            rigidBody.drag = Math.Max(rigidBody.drag - dragStep, dragMin); 
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            rigidBody.drag = defaultDrag;
        }
    }
    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.C)) 
        {
            if (state == State.Alive)
            {
                state = State.Debug;
            }
            else
            {
                state = State.Alive;
            }
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
    }    
}


