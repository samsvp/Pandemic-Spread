using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{

    private bool isSick = false;
    private bool isRecovered = false;
    private bool isDead = false;
    private bool isVacinated = false;

    [SerializeField]
    private float timeToChangeDirection = 1f;
    [SerializeField]
    private float speed;
    private Vector3 initialPos;

    public Color sickColor;
    public Color recoveredColor;
    public Color normalColor;
    public Color deadColor;

    private Disease disease = null;

    private SpriteRenderer sR;
    private Rigidbody2D rb2d;
    private CircleCollider2D cc2d;

    private GameObject diseaseCircle;
    private float circleStartScale;

    public bool IsSick { get => isSick; }
    public bool IsRecovered { get => isRecovered; }
    public bool IsDead { get => isDead; }
    public bool IsVacinated { get => isVacinated; }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sR = GetComponent<SpriteRenderer>();
        cc2d = GetComponent<CircleCollider2D>();

        diseaseCircle = transform.GetChild(0).gameObject;
        circleStartScale = diseaseCircle.transform.localScale.x;
        diseaseCircle.SetActive(false);

        sR.color = normalColor;

        ChangeDirection();
    }

    private void Start()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        if (GameManager.instance.pause)
        {
            rb2d.velocity = Vector2.zero;
            return;
        }

        if (diseaseCircle.activeSelf)
        {
            PulseDiseaseCircle();
        }

        if (!isDead)
        {
            HandleDirection();
        }
    }


    /// <summary>
    /// Makes the person contract the disease
    /// </summary>
    /// <param name="_disease"></param>
    /// <param name="force"></param>
    /// <returns></returns>
    public bool SetAsSick(Disease _disease, bool force=false)
    {
        if (disease != null || isDead) return false;

        float infectionRate = isVacinated ?
            _disease.InfectionRate * Vaccine.infectionModifier : _disease.InfectionRate;
        float reinfectionRate = isVacinated ?
            _disease.ReinfectionRate * Vaccine.infectionModifier : _disease.ReinfectionRate;
        

        if (force ||  (!isRecovered && Random.Range(0f, 1f) < infectionRate) || 
            Random.Range(0f, 1f) < reinfectionRate)
        {
            isSick = true;
            diseaseCircle.SetActive(true);
            sR.color = sickColor;
            disease = _disease;
            disease.host = this;
        }

        return isSick;
    }


    /// <summary>
    /// Recovers the character from the disease
    /// </summary>
    public void SetAsRecovered()
    {
        isRecovered = true;
        isSick = false;

        diseaseCircle.SetActive(false);

        if (isVacinated || Random.Range(0f, 1f) > disease.Lethality)
        {
            sR.color = recoveredColor;
        }
        else
        {
            isDead = true;
            sR.color = deadColor;
            rb2d.velocity = Vector2.zero;
            GetComponent<CircleCollider2D>().enabled = false;
        }
        disease = null;
    }


    #region random walk

    private void HandleDirection()
    {
        if (GameManager.instance.isSocialDistancing)
        {
            cc2d.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, initialPos, Time.deltaTime * 2 * speed);
            return;
        }

        timeToChangeDirection -= Time.deltaTime;

        if (timeToChangeDirection <= 0)
        {
            ChangeDirection();
        }

        rb2d.velocity = transform.up * speed;
    }


    private void ChangeDirection()
    {
        float angle = Random.Range(0f, 360f);
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 newUp = quat * Vector3.up;
        newUp.z = 0;
        newUp.Normalize();
        transform.up = newUp;
        timeToChangeDirection = 1.5f;
    }


    private void GoToInitialPosition()
    {
        float angle = Vector2.Angle(transform.position, initialPos);
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 newUp = quat * Vector3.up;
        newUp.z = 0;
        newUp.Normalize();
        transform.up = newUp;
    }

    #endregion random walk


    private void PulseDiseaseCircle()
    {
        float r = 0.5f * Mathf.Cos(2f * Time.time) + circleStartScale;

        diseaseCircle.transform.localScale = new Vector3(r, r, 1);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        transform.up *= -1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isSick && collision.CompareTag("Person"))
        {
            Person person = collision.GetComponent<Person>();
            if (!person.isSick) disease.Spread(person);
        }
    }

}
