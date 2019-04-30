using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    private Transform MissileTransform;
    private Rigidbody MissileRigidBody;
    private Transform TargetTransform;
    public Transform MyCube;

    [SerializeField]
    private CustomMissilePosition ForwardVector;
    [SerializeField]
    private CustomMissilePosition UpwardVector;
    [SerializeField]
    private CustomMissilePosition DownwardVector;
    [SerializeField]
    private Vector3 TestVector;
    private Vector3 TestDownVector;

    public Transform FrontRaycaster;
    public Transform FrontDownAngeledRaycaster;
    public Transform ElevationRayCaster;
    public GameObject Target;
    private GameObject tempTarget;
    public Transform FrontUpAngeledRaycaster;
    public Transform ExplosionMissile;

    public int RayCastFrontalRange;
    private float AngledRaycastLength;

    [SerializeField]
    private Vector3 tempVectorForward;
    [SerializeField]
    private Vector3 tempVectorUp;
    [SerializeField]
    private Vector3 tempVectorDown;

    private float ElevationRaycastData;


    private bool DetourFromTarget;
    private bool tempTargetReached;
    private bool closeToTarget;
    private bool tempDownTargetReached;


    public float MaxSpeed;
    public float MaxPitch;
    public float MaxElevation;
    public float MinimumElevation;

    //temp variables checking raycast


    // Start is called before the first frame update
    void Start()
    {
        float radfromdegree = Convert.ToSingle(FrontUpAngeledRaycaster.transform.rotation.x * Math.PI / 180);
        AngledRaycastLength = RayCastFrontalRange / Mathf.Cos(radfromdegree);
        TargetTransform = Target.GetComponent<Transform>();
        MissileRigidBody = gameObject.GetComponent<Rigidbody>();
        MissileTransform = gameObject.GetComponent<Transform>();
        tempTargetReached = true;
        


        // temp raycast stuff

    }

    private void FixedUpdate()
    {
        //part 0.5 get vector positions of raycast point
        tempVectorForward = FrontRaycaster.GetComponent<Transform>().TransformDirection(Vector3.forward) * RayCastFrontalRange;
        Debug.DrawRay(FrontRaycaster.GetComponent<Transform>().position, tempVectorForward, Color.green);


        tempVectorDown = FrontDownAngeledRaycaster.GetComponent<Transform>().TransformDirection(Vector3.forward) * (float)AngledRaycastLength;
        Debug.DrawRay(FrontDownAngeledRaycaster.GetComponent<Transform>().position, tempVectorDown, Color.blue);

        tempVectorUp = FrontUpAngeledRaycaster.GetComponent<Transform>().TransformDirection(Vector3.forward) * (float)AngledRaycastLength;
        Debug.DrawRay(FrontUpAngeledRaycaster.GetComponent<Transform>().position, tempVectorUp, Color.red);

        //part one - missile flies
        MissileRigidBody.velocity = MissileTransform.forward * MaxSpeed;

        if (!DetourFromTarget)
        {
            var targetRotation = Quaternion.LookRotation(TargetTransform.position - MissileTransform.position);
            MissileRigidBody.MoveRotation(Quaternion.RotateTowards(MissileTransform.rotation, targetRotation, MaxPitch));
        }
        //part two - get data while flying
        ForwardVector = GetRaycastData(FrontRaycaster, RayCastFrontalRange);
        DownwardVector = GetRaycastData(FrontDownAngeledRaycaster, AngledRaycastLength);
        ElevationRaycastData = transform.position.y;
        UpwardVector = GetRaycastData(FrontUpAngeledRaycaster, AngledRaycastLength);

        //part three - Adjust Flight Path
        doStuffWithData(ForwardVector, DownwardVector, UpwardVector, ElevationRaycastData);

    }



    public CustomMissilePosition GetRaycastData(Transform RaycastShooter, float maxdistance)
    {
        CustomMissilePosition temp = new CustomMissilePosition();
        RaycastHit hit;
        //if 
        if (Physics.Raycast(RaycastShooter.transform.position, RaycastShooter.TransformDirection(Vector3.forward), out hit, maxdistance))//(Physics.SphereCast(RaycastShooter.transform.position, 0.30f, RaycastShooter.transform.forward, out hit, maxdistance))
        {
            if(Vector3.Distance(MissileTransform.position, TargetTransform.position) < 100.0f)
            {
                closeToTarget = true;
                DetourFromTarget = false;
            }
            if (hit.collider.gameObject.name != "Cube(Clone)")
            {
                temp.EndOfRayPosition = transform.TransformPoint(hit.point);
                temp.WasHit = true;
                return temp;
            }
            else
            {
                float dist = Vector3.Distance(MissileTransform.position, tempTarget.transform.position);
                if( dist < 5)
                {
                    DestroyTheTemp();
                }
            }
        }
        temp.EndOfRayPosition = transform.TransformPoint(RaycastShooter.TransformDirection(Vector3.forward) * maxdistance);
        temp.WasHit = false;
        return temp;

    }

    public void doStuffWithData(CustomMissilePosition front, CustomMissilePosition downang, CustomMissilePosition upang, float elevation)
    {

        //at this point we know we have something relatively close to us and infront of us so we need to go up to avoid it.
        if (front.WasHit || !tempTargetReached && !closeToTarget)
        {
            
            // if (upang == 0.0F)// here we know that that object infront of us is not very tall and we can rotate 10 degrees to avoid impact
            // {
            DetourFromTarget = true;
            AdjustUp();

            // }
        }
        //else if (!downang.WasHit && tempDownTargetReached)
        //{
        //    DetourFromTarget = true;
        //    AdjustDown();
        //}

    }

    private void AdjustUp()
    {

        if (tempTargetReached)
        {
            TestVector = transform.TransformPoint(tempVectorUp);
            Transform temp = Instantiate(MyCube);
            tempTarget = temp.gameObject;
            tempTargetReached = false;
            temp.position = TestVector;
        }
        Quaternion rotation = Quaternion.LookRotation(TestVector - MissileTransform.position);   //Quaternion.FromToRotation(gameObject.transform.forward, FrontUpAngeledRaycaster.transform.forward);
        MissileRigidBody.MoveRotation(Quaternion.RotateTowards(MissileTransform.rotation, rotation, MaxPitch));


        
        // MyCube.transform.parent = null;
    }

    private void AdjustDown()
    {
        if (tempDownTargetReached)
        {
            TestDownVector = transform.TransformPoint(tempVectorDown);
            Transform temp = Instantiate(MyCube);
            tempTarget = temp.gameObject;
            tempDownTargetReached = false;
            temp.position = TestDownVector;
        }
        Quaternion rotation = Quaternion.LookRotation(TestDownVector - MissileTransform.position);//Quaternion.FromToRotation(gameObject.transform.forward, FrontDownAngeledRaycaster.transform.forward);
        MissileRigidBody.MoveRotation(Quaternion.RotateTowards(MissileTransform.rotation, rotation, MaxPitch));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Target)
        {
            Instantiate(ExplosionMissile).transform.position = this.transform.position;
            Destroy(Target);
            Destroy(this);
        }
        else
        {
            Instantiate(ExplosionMissile).transform.position = this.transform.position;
            Destroy(this);
        }
    }

    public void DestroyTheTemp()
    {
        tempDownTargetReached = true;
        tempTargetReached = true;
        Destroy(GameObject.Find("Cube(Clone)"));
        //Destroy(tempTarget);
        DetourFromTarget = false;
        var targetRotation = Quaternion.LookRotation(TargetTransform.position - MissileTransform.position);
        MissileRigidBody.MoveRotation(Quaternion.RotateTowards(MissileTransform.rotation, targetRotation, MaxPitch));
    }
}


public class CustomMissilePosition
{
    public bool WasHit;
    public Vector3 EndOfRayPosition;
}
