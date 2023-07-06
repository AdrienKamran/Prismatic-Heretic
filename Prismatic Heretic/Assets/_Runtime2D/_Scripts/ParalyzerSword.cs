using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class ParalyzerSword : MonoBehaviour
{

    public LayerMask enemyLayer;
    public GameObject player;

    public SelectSword selectionControl;
    public Rigidbody2D body;

    public Transform firePoint;
    public GameObject beamPrefab;

    public GameObject anchor;
    public GameObject spawnPoint;
    public GameObject explosion;

    public GameObject flash;

    public GameObject prepEffect;

    public bool shooting = false;
    public bool placingOrb = false;
    public bool prepped = false;

    private bool canShoot = true;

    //Stuff for triangle aoe move (left click)
    //Allows the player to place down three orbs (each at the current tip of the yellow sword)
    //and damage all enemies in the triangle formed by connecting them
    public GameObject orbPrefab;
    public GameObject orb1;
    public GameObject orb2;
    public GameObject orb3;

    Vector2 edgeA;
    Vector2 edgeB;
    Vector2 edgeC;

    float cVertex1_X;
    float cVertex1_Y;
    float cVertex2_X;
    float cVertex2_Y;

    Vector2 origin;

    public int orbsUsed = 0;

    public LineRenderer lineA;
    public LineRenderer lineB;
    public LineRenderer lineC;

    public GameObject triangleEffect;
    //
    public AudioSource source;
    public AudioClip ability;
    public AudioClip prep;
    public AudioClip shoot;
    public AudioClip orb;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Heretic");
        lineA.gameObject.SetActive(false);
        lineB.gameObject.SetActive(false);
        lineC.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!Player.GameIsPaused)
        {
            if (!Player.inDialog)
            {
                //If yellow is being held (not prepped or tucked away)
                if (selectionControl.currentlyHeld == 3)
                {
                    //Shoot projectile
                    if (Input.GetMouseButtonDown(1) == true && shooting == false && prepped == false && canShoot == true)
                    {
                        canShoot = false;
                        Debug.Log("Pew pew pew");
                        shooting = true;
                        selectionControl.yellowInUse = true;
                        source.PlayOneShot(shoot);
                        Shoot();
                        Reload();
                    }
                    //Prep
                    if (Input.GetKeyDown(KeyCode.Q) && shooting == false && prepped == false)
                    {
                        float dist = Vector3.Distance(player.transform.position, this.transform.position);
                        if (dist < 5)
                        {
                            Debug.Log("Yellow is prepped!");
                            prepped = true;
                            selectionControl.yellowInUse = true;
                            source.PlayOneShot(prep);
                            Prep();
                        }
                    }

                    //Place Orb
                    if (Input.GetMouseButtonDown(0) == true && shooting == false && prepped == false && !(placingOrb) && orbsUsed < 3)
                    {
                        Debug.Log("orb placed");
                        placingOrb = true;
                        selectionControl.yellowInUse = true;
                        source.PlayOneShot(orb);
                        Place();
                    }
                }

                //If yellow is prepped (and is being activated)
                if (Input.GetKeyDown(KeyCode.E) == true && prepped == true)
                {
                    if (Player.yellowPowerDelay == 50)
                    {
                        Debug.Log("Yellow has been activated!");
                        source.PlayOneShot(ability);
                        Activate();
                        Player.yellowPowerDelay = 0;
                    }
                }

                //If yellow is neither shooting nor prepped, check if it should be active depending on whether it is currently meant to be held.
                if (shooting == false && placingOrb == false && prepped == false)
                {
                    float dist = Vector3.Distance(player.transform.position, this.transform.position);
                    if (dist < 5)
                    {
                        selectionControl.yellowInUse = false;
                        if (selectionControl.currentlyHeld == 1 || selectionControl.currentlyHeld == 2)
                        {
                            selectionControl.yellowSword.GetComponent<SpriteRenderer>().enabled = false;
                            
                            
                        }
                    }
                }


            }
            
        }
        
    }

    void Prep()
    {
        body.velocity = new Vector2(0.0f, 0.0f);
    }

    void Activate()
    {
        StartCoroutine(Wait());
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(this.transform.position, 5, enemyLayer);
        foreach (Collider2D col in enemiesInRange)
        {
            Debug.Log("HERE");
           BaseEnemy enemy = (BaseEnemy)col.GetComponent<BaseEnemy>();
            if (!enemy.stunned)
            {
                enemy.SetStunned(true);
                enemy.SetMarked(true);

            }
           
        }
    }

    void Shoot() 
    {
        Instantiate(beamPrefab, firePoint.position, firePoint.rotation);

        Quaternion rotation = anchor.transform.rotation;

        shooting = false;
    }

    void Reload()
    {
        StartCoroutine(beamCooldown());
    //    canShoot = true;
    }

    //Place an Orb
    void Place()
    {
        Debug.Log("Orb successfully placed!");
        if(orbsUsed == 0)
        {
            orb1 = Instantiate(orbPrefab, firePoint.position, firePoint.rotation);
            orbsUsed++;
            Vector2 pos = orb1.GetComponent<Rigidbody2D>().position;
            Debug.Log("orb 1 pos: " + pos);
        }
        else if (orbsUsed == 1)
        {
            orb2 = Instantiate(orbPrefab, firePoint.position, firePoint.rotation);
            orbsUsed++;
            Vector2 pos = orb2.GetComponent<Rigidbody2D>().position;
            Debug.Log("orb 2 pos: " + pos);
        }
        else if (orbsUsed == 2)
        {
            orb3 = Instantiate(orbPrefab, firePoint.position, firePoint.rotation);
            orbsUsed++;
            Vector2 pos = orb3.GetComponent<Rigidbody2D>().position;
            Debug.Log("orb 3 pos: " + pos);
            //For now, placing the third orb causes the effect to occur immediately
            CalculateEdges();
            //Move this later(?):
            //Mmm... Maybe. Depends on if we want the effect to occur immediately or not. 
            AttackInTriangle();
        }

        placingOrb = false;
    }

    /// <summary>
    ///The triangle, as it is defined in the algorithm, is composed of three edges: A, B, and C
    ///How we number the three vertices is also precisely defined as well. In particular:
    ///Vertex 1 is the most elevated on the y-axis, vertex 2 the second highest, and vertex 3 the third highest
    ///Edge A connects 1 and 2, Edge B connects 2 and 3, and Edge C connects 1 and 3
    ///This may seem useless or arbitrary, but defining the triangle this way allows us to make some
    ///useful assumptions later. In addittion, we define our "origin" to be the location of vertex 2.
    ///This function calculates Edges A, B, and C.
    /// </summary>
    void CalculateEdges() 
    {
        float orb1Y = orb1.GetComponent<Rigidbody2D>().position.y;
        float orb1X = orb1.GetComponent<Rigidbody2D>().position.x;
        float orb2Y = orb2.GetComponent<Rigidbody2D>().position.y;
        float orb2X = orb2.GetComponent<Rigidbody2D>().position.x;
        float orb3Y = orb3.GetComponent<Rigidbody2D>().position.y;
        float orb3X = orb3.GetComponent<Rigidbody2D>().position.x;

        //calculate the highest orb in terms of y:
        if(orb1Y >= orb2Y && orb1Y >= orb3Y) //Vertex #1 = orb1
        {
            if(orb2Y >= orb3Y) //Vertex #2 = orb2, Vertex #3 = orb3
            {
                edgeA = new Vector2(orb1X - orb2X, orb1Y - orb2Y);
                edgeB = new Vector2(orb2X - orb3X, orb2Y - orb3Y);
                edgeC = new Vector2(orb1X - orb3X, orb1Y - orb3Y);

                origin = new Vector2(orb2X, orb2Y); //origin is vertex 2 (always)
                cVertex1_X = orb1X;
                cVertex2_X = orb3X;
                cVertex1_Y = orb1Y;
                cVertex2_Y = orb3Y;

            }
            else //Vertex #2 = orb3, Vertex #3 = orb2
            {
                edgeA = new Vector2(orb1X - orb3X, orb1Y - orb3Y);
                edgeB = new Vector2(orb3X - orb2X, orb3Y - orb2Y);
                edgeC = new Vector2(orb1X - orb2X, orb1Y - orb2Y);

                cVertex1_X = orb1X;
                cVertex2_X = orb2X;
                cVertex1_Y = orb1Y;
                cVertex2_Y = orb2Y;

                origin = new Vector2(orb3X, orb3Y); //origin is vertex 2 (always)
            }
        }
        else if (orb2Y >= orb1Y && orb2Y >= orb3Y) //Vertex #1 = orb2
        {
            if (orb1Y >= orb3Y) //Vertex #2 = orb1, Vertex #3 = orb3
            {
                edgeA = new Vector2(orb2X - orb1X, orb2Y - orb1Y);
                edgeB = new Vector2(orb1X - orb3X, orb1Y - orb3Y);
                edgeC = new Vector2(orb2X - orb3X, orb2Y - orb3Y);

                cVertex1_X = orb2X;
                cVertex2_X = orb3X;
                cVertex1_Y = orb2Y;
                cVertex2_Y = orb3Y;

                origin = new Vector2(orb1X, orb1Y); //origin is vertex 2 (always)
            }
            else //Vertex #2 = orb3, Vertex #3 = orb1
            {
                edgeA = new Vector2(orb2X - orb3X, orb2Y - orb3Y);
                edgeB = new Vector2(orb3X - orb1X, orb3Y - orb1Y);
                edgeC = new Vector2(orb2X - orb1X, orb2Y - orb1Y);

                cVertex1_X = orb2X;
                cVertex2_X = orb1X;
                cVertex1_Y = orb2Y;
                cVertex2_Y = orb1Y;

                origin = new Vector2(orb3X, orb3Y); //origin is vertex 2 (always)
            }
        }
        else if (orb3Y >= orb1Y && orb3Y >= orb2Y) //Vertex #1 = orb3
        {
            if (orb1Y >= orb2Y) //Vertex #2 = orb1, Vertex #3 = orb2
            {
                edgeA = new Vector2(orb3X - orb1X, orb3Y - orb1Y);
                edgeB = new Vector2(orb1X - orb2X, orb1Y - orb2Y);
                edgeC = new Vector2(orb3X - orb2X, orb3Y - orb2Y);

                cVertex1_X = orb3X;
                cVertex2_X = orb2X;
                cVertex1_Y = orb3Y;
                cVertex2_Y = orb2Y;
                origin = new Vector2(orb1X, orb1Y); //origin is vertex 2 (always)
            }
            else //Vertex #2 = orb2, Vertex #3 = orb1
            {
                edgeA = new Vector2(orb3X - orb2X, orb3Y - orb2Y);
                edgeB = new Vector2(orb2X - orb1X, orb2Y - orb1Y);
                edgeC = new Vector2(orb3X - orb1X, orb3Y - orb1Y);

                cVertex1_X = orb3X;
                cVertex2_X = orb1X;
                cVertex1_Y = orb3Y;
                cVertex2_Y = orb1Y;
                origin = new Vector2(orb2X, orb2Y); //origin is vertex 2 (always)
            }
        }

    }

    /// <summary>
    /// We will "sweep" our raycasts from Edge A to Edge B, so we must get the angle between them
    /// </summary>
    /// <returns>The angle between Edge B and Edge A</returns>
    float GetAngle()
    {
        float angle = Vector2.Angle(edgeA, edgeB);
        angle = 180 - angle; 
      //  Debug.Log("ANGLE: " + angle);
        return angle;
    }

    /// <summary>
    /// The "sweep" from A to B will be either clockwise or counter-clockwise. The latter occurs in the case where the (unsigned) angle
    /// between B and A, going counter-clockwise from B to A, is greater than 180 degrees.
    /// </summary>
    /// <returns>A bool indicating whether to go clockwise from A to B or not</returns>
    bool GetDirectionOfSweep()
    {
        var sign = Mathf.Sign(edgeA.x * edgeB.y - edgeA.y * edgeB.x);
        if(sign < 0)
        {
            return false; //sweep is counter-clockwise from edge A to edge B
        }
        else
        {
            return true; //sweep is clockwise from edge A to edge B
        }
    }

    /// <summary>
    /// This is where all the magic happens. Namely, we first check if we are going clockwise or counter-clockwise. Then,
    /// in either case, we shoot rays starting from edge A, tilting the ray 1 degree each time until we finally reach edge B.
    /// Of course, this on its own would constitute a set of rays that mimics a semi-cirlce of sorts- but we want a triangle!
    /// Thus, for each current ray, we calculate the point of intersection of the current ray (using the vector equation of a line
    /// to get a random point on it, plus the origin) and edge C (yup! we need edge C after all). After calculating this, we calculate
    /// the distance from the origin to the intersection point, and that gives us the length of the current ray being cast. :]
    /// </summary>
    void AttackInTriangle()
    {
        float angle = GetAngle();
        float angleFilled = 0.0f;
        //check if the angle is "filled in" clockwise or counter-clockwise
        bool clockwise = GetDirectionOfSweep();
        var enemiesList = new ArrayList();

    //    Debug.Log("Clockwise: " + clockwise);
        if (clockwise) //clockwise
        {
            Debug.Log("Clockwise");
            Physics2D.RaycastAll(origin, edgeA, 100.0f);
            Vector2 direction = edgeA;
            direction.Normalize();

            Debug.DrawRay(new Vector2(cVertex2_X, cVertex2_Y), edgeC, Color.blue, 5000.0f, false);
            Debug.DrawRay(origin, edgeA, Color.red, 5000.0f, false);

            while (angleFilled < angle)
            {
                direction = Quaternion.Euler(0, 0, -1.0f) * direction;
                direction.Normalize();

                Vector2 pointOnLine = direction * 5 + origin;
                bool found = false;
                Vector2 poi = GetIntersectionPointCoordinates(origin, pointOnLine, new Vector2(cVertex1_X, cVertex1_Y),
                    new Vector2(cVertex2_X, cVertex2_Y), out found);
                Vector2 length = poi - origin;
                Debug.DrawRay(origin, length, Color.green, 5000.0f, false);

                float lengthMagnitude = Vector2.SqrMagnitude(length);
                RaycastHit2D[] hit = Physics2D.RaycastAll(origin, direction, Mathf.Sqrt(lengthMagnitude));

                foreach (RaycastHit2D thingHit in hit)
                {
                    BaseEnemy enemy = thingHit.transform.GetComponent<BaseEnemy>();
                    if (enemy != null && !enemiesList.Contains(enemy))
                    {
                        enemiesList.Add(enemy);
                    }
                }
                    angleFilled++;
            }
        }
        else //counter-clockwise
        {
            Debug.Log("Counter-clockwise");
            Vector2 direction = edgeA;
            direction.Normalize();

            Debug.DrawRay(new Vector2(cVertex2_X, cVertex2_Y), edgeC, Color.blue, 5000.0f, false);
            Debug.DrawRay(origin, edgeA, Color.red, 5000.0f, false);

            while (angleFilled < angle)
            {
                direction = Quaternion.Euler(0, 0, 1.0f) * direction;
                direction.Normalize();

                Vector2 pointOnLine = direction * 5 + origin;
                bool found = false;
                Vector2 poi = GetIntersectionPointCoordinates(origin, pointOnLine, new Vector2(cVertex1_X, cVertex1_Y),
                    new Vector2(cVertex2_X, cVertex2_Y), out found);
                Vector2 length = poi - origin;

                Debug.DrawRay(origin, length, Color.green, 5000.0f, false);

                float lengthMagnitude = Vector2.SqrMagnitude(length);
                RaycastHit2D[] hit = Physics2D.RaycastAll(origin, direction, Mathf.Sqrt(lengthMagnitude));

                foreach (RaycastHit2D thingHit in hit)
                {

                    BaseEnemy enemy = thingHit.transform.GetComponent<BaseEnemy>();
                    if (enemy != null && !enemiesList.Contains(enemy))
                    {
                        enemiesList.Add(enemy);
                    }
                }

                angleFilled++;
            }
        }

        //Lines appear connecting orbs
        lineA.gameObject.SetActive(true);
        lineA.positionCount = 2;
        lineA.SetPosition(0, new Vector3(orb1.GetComponent<Rigidbody2D>().position.x, orb1.GetComponent<Rigidbody2D>().position.y, 0));
        lineA.SetPosition(1, new Vector3(orb2.GetComponent<Rigidbody2D>().position.x, orb2.GetComponent<Rigidbody2D>().position.y, 0));
       
        lineB.gameObject.SetActive(true);
        lineB.positionCount = 2;
        lineB.SetPosition(0, new Vector3(orb1.GetComponent<Rigidbody2D>().position.x, orb1.GetComponent<Rigidbody2D>().position.y, 0));
        lineB.SetPosition(1, new Vector3(orb3.GetComponent<Rigidbody2D>().position.x, orb3.GetComponent<Rigidbody2D>().position.y, 0));

        lineC.gameObject.SetActive(true);
        lineC.positionCount = 2;
        lineC.SetPosition(0, new Vector3(orb3.GetComponent<Rigidbody2D>().position.x, orb3.GetComponent<Rigidbody2D>().position.y, 0));
        lineC.SetPosition(1, new Vector3(orb2.GetComponent<Rigidbody2D>().position.x, orb2.GetComponent<Rigidbody2D>().position.y, 0));

        StartCoroutine(WaitLines());

        //Effect on found enemies occurs
        int totalEnemiesFound = 0;
        foreach (BaseEnemy enemy in enemiesList)
        {
            totalEnemiesFound++;
            Instantiate(triangleEffect, enemy.transform.position, Quaternion.identity);
            enemy.DealDamage(10);
        //    enemy.SetStunned(true);
        }
        Debug.Log("FOUND " + totalEnemiesFound + " ENEMIES");

    }

    /// <summary>
    /// Gets the coordinates of the intersection point of two lines.
    /// Borrowed from here: https://blog.dakwamine.fr/?p=1943
    /// Note that in our case, we will never run into a "no solution" scenario
    /// </summary>
    /// <param name="A1">A point on the first line.</param>
    /// <param name="A2">Another point on the first line.</param>
    /// <param name="B1">A point on the second line.</param>
    /// <param name="B2">Another point on the second line.</param>
    /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
    /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
    public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            found = false;
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        found = true;

        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }

    private IEnumerator Wait()
    {
        GameObject obj = Instantiate(prepEffect, this.transform.position, Quaternion.identity);
        Destroy(obj, 3f);
        flash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        flash.SetActive(false);
        prepped = false;
    }

    private IEnumerator WaitLines()
    {
        bool ready = false;
   
        while (!ready)
        {
            yield return new WaitForSeconds(0.1f);
            ready = true;
        }
        lineA.gameObject.SetActive(false);
        lineB.gameObject.SetActive(false);
        lineC.gameObject.SetActive(false);

        Destroy(orb1);
        Destroy(orb2);
        Destroy(orb3);
        orbsUsed = 0;
    }

    private IEnumerator beamCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }
}
