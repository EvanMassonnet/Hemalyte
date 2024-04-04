using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic : MonoBehaviour
{

    public bool restart;

    public int nbDetail = 10;
    public int nbLeg = 1;
    public int tentaclesPerLeg = 1;
    public float noise = 0.0f;
    public float footNoise = 0.0f;
    public float amplitude = 1;
    public float speed = 1;

    [Range(0.0f,1.0f)]
    public float deployment = 0.5f;
    public GameObject defaultLeg;
    public GameObject payload;

    public float maxFootDistance = 2.0f;
    public int smoothness = 1;

    public float Position1Height = 0.9f;
    public float Position2Height = 1.5f;
    public float Position2Distance = 0.5f;
    public float Position3Distance = 1f;
    public float Position4Distance = 1.5f;




    private Vector3[][] legPoints;
    private float[] deployments;
    private float time = 0;
    private LineRenderer lr;
    private int stepBetweenDivision;
    private LineRenderer[] lineRenders;
    private Ray[] rays;

    private Vector3 velocity;
    private Vector3 lastVelocity;
    private Vector3 lastBodyPos;

    void Start()
    {
        Init();

    }


    void Init()
    {
        lineRenders = new LineRenderer[nbLeg * tentaclesPerLeg];
        legPoints = new Vector3[nbLeg * tentaclesPerLeg][];
        deployments = new float[nbLeg];
        rays = new Ray[nbLeg];
        rays[0] = new Ray(transform.position + new Vector3(1,2,0), -transform.up);
        rays[1] = new Ray(transform.position + new Vector3(0,2,1), -transform.up);
        rays[2] = new Ray(transform.position + new Vector3(-1,2,0), -transform.up);
        rays[3] = new Ray(transform.position + new Vector3(0,2,-1), -transform.up);

        lastBodyPos = transform.position;

        for(int i = 0; i < nbLeg; ++i){
            deployments[i] = 1;

            for(int j = 0; j < tentaclesPerLeg; ++j){
                GameObject newLeg = Instantiate(defaultLeg, transform.position, Quaternion.Euler(0, i * 360/nbLeg ,0));
                newLeg.transform.parent = gameObject.transform;
                newLeg.SetActive(true);
                LineRenderer lr = newLeg.GetComponent<LineRenderer>();
                lr.positionCount = nbDetail;
                lineRenders[i * tentaclesPerLeg + j] = lr;

                Vector3[] newVecotrs = new Vector3[4];
                newVecotrs[0] = new Vector3(0,Position1Height,0); //transform.position;
                newVecotrs[1] = new Vector3(Position2Distance,Position2Height,0) + new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), Random.Range(-noise, noise)); //transform.position + new Vector3(1,0.5f,0) + new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), Random.Range(-noise, noise));
                newVecotrs[2] = new Vector3(Position3Distance,Position2Height,0) + new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), Random.Range(-noise, noise)); //transform.position + new Vector3(2,0.5f,0) + new Vector3(Random.Range(-noise, noise), Random.Range(-noise, noise), Random.Range(-noise, noise));
                newVecotrs[3] = new Vector3(Position4Distance,0,0) + new Vector3(Random.Range(-footNoise, footNoise), 0, Random.Range(-footNoise, footNoise));
                legPoints[i * tentaclesPerLeg + j] = newVecotrs;
            }
        }
    }


    void FixedUpdate()
    {
        if (restart)
        {
            Init();
            restart = false;
        }

        velocity = transform.position - lastBodyPos;
        //velocity = (velocity + smoothness * lastVelocity) / (smoothness + 1f);

        rays[0].origin += velocity;
        rays[1].origin += velocity;
        rays[2].origin += velocity;
        rays[3].origin += velocity;

        rays[0].direction = transform.up + -2*velocity;
        rays[1].direction = transform.up + -2*velocity;
        rays[2].direction = transform.up + -2*velocity;
        rays[3].direction = transform.up + -2*velocity;


        //block position (update velocity)
        if (velocity.magnitude < 0.000025f)
            velocity = lastVelocity;
        else{
            for(int i = 0; i < nbLeg; ++i){
                for(int j = 0; j < tentaclesPerLeg; ++j){
                    legPoints[i * tentaclesPerLeg + j][3] -= Quaternion.Euler(0, -i * 360/nbLeg, 0) * velocity;
                }
            }

            lastVelocity = velocity;
        }

        //Deploy or retract base on distance
        for(int i = 0; i < nbLeg; ++i){
            if(deployments[i] != 0f && (transform.TransformPoint(legPoints[i * tentaclesPerLeg][3]) - transform.position).magnitude > 3){
                deployments[i] -= 0.05f;
                if(deployments[i] < 0.1f)
                    deployments[i] = 0f;
            }else if(deployments[i] != 1f && (transform.TransformPoint(legPoints[i * tentaclesPerLeg][3]) - transform.position).magnitude < 3){
                deployments[i] += 0.05f;
                if(deployments[i] > 0.99f)
                    deployments[i] = 1f;
            }
        }

        time += 0.01f;

        for(int i =0; i < lineRenders.Length ; ++i){
            lineRenders[i].positionCount = (int)(nbDetail * deployments[i/nbLeg]);
            lineRenders[i].SetPositions(CreatCurve(move(legPoints[i]), deployments[i/nbLeg]));
        }
        payload.transform.position += amplitude*0.0005f * new Vector3(0, Mathf.Sin(time * speed), Mathf.Cos(time * speed));
        lastBodyPos = transform.position;
    }


    private void OnDrawGizmosSelected()
    {
        if (rays != null)
        {
            for(int i = 0; i < rays.Length; ++i){
                Gizmos.color = Color.red;
                Gizmos.DrawRay(rays[i]);
            }
        }

    }

    private Vector3[] move(Vector3[] leg){
        leg[1] += amplitude*0.0005f * new Vector3(0,  Mathf.Sin(time * speed),   Mathf.Cos(time * speed));
        leg[2] += amplitude*0.0005f * new Vector3(0,  Mathf.Sin(time * speed + 1), -Mathf.Cos(time * speed + 1));

        return leg;
    }


    private Vector3[] CreatCurve(Vector3[] bezierPoints, float deployment){
        int size = (int)(nbDetail*deployment);
        Vector3[] result = new Vector3[size];

        for(int i = 0; i < nbDetail; ++i){
            if(i == size){
                break;
            }
            result[i] = EvaluateCubicCurve(bezierPoints[0], bezierPoints[1], bezierPoints[2], bezierPoints[3], (float)i / (nbDetail-1));
        }
        return result;
    }

    public static Vector3 EvaluateCubicCurve (Vector3 a1, Vector3 c1, Vector3 c2, Vector3 a2, float t) {
            t = Mathf.Clamp01 (t);
            return (1 - t) * (1 - t) * (1 - t) * a1 + 3 * (1 - t) * (1 - t) * t * c1 + 3 * (1 - t) * t * t * c2 + t * t * t * a2;
    }


}
