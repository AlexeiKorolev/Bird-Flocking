/*
 * BirdMovement.cs
 * Responsible for handling the movement patters of each bird
 * Note: not opitmized for computational speed; has O(n^2) efficiency
 * Last edit: 5/3/23
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{   
    public const float speed = 30f;
    public const float rotationSpeed = 2f;
    public const float OREINTATION_WEIGHT = 0.997f; // Percentage that a bird follows the mean orientation rather than the average position
    public const float ATTRACTION_RANGE = 16f;      // The range that a bird considers when calculating average position 
    public const float ATTRACTION_FACTOR = 1f;      // Strength of attraction between birds. Fights repulsion force and orientation alignment
    public const float REPULSION_WEIGHT = 3f;       // Strength of repulsion against birds.
    public const float REPULSION_RANGE = 5f;        // Range of repulsive force
    public const float BIRD_SIZE = 0.15f;           // Render size of the bird (ratio of original image size)
    public const int Y_BOUND = 100;                 // Max absolute y-value
    public const int X_BOUND = 250;                 // Max absolute x value

    public Transform obj;
    public Vector3 meanPosition;
    public Vector3 dir;
    public Vector3 meanOrientation;
    public Vector3 repulsiveForce;
    private GameObject[] birds;                     

    void Start()
    {
        obj = this.gameObject.GetComponent<Transform>();
    }

    void Update()
    {
        birds =  GetComponentInParent<Setup>().birds; // Retrieving most relevant data
        obj.transform.position += obj.transform.up * Time.deltaTime * -speed;
        UpdateWeightedAvgPosition();

        Quaternion rotationTowardsAverage = PositionToRotation(meanPosition);
        Quaternion prevRotation = obj.transform.rotation;
        obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, rotationTowardsAverage, rotationSpeed * Time.deltaTime); // Rotate toward final calculated rotation.

        Vector3 difference = prevRotation.eulerAngles - obj.transform.rotation.eulerAngles;
        obj.transform.localScale = new Vector3(Mathf.Min(BIRD_SIZE / difference.magnitude, BIRD_SIZE), -BIRD_SIZE, 1); // Shrinks wings on bird turn.
    }

    float distance(Vector3 pos1, Vector3 pos2) { // Euclidean distance between two Vector3 positions.
        return (pos1 - pos2).magnitude;
    }

    void UpdateWeightedAvgPosition() {
        Vector3 deviations = new Vector3(); // Stores cumulative deviation from bird position
        Vector3 orientations = new Vector3(); // Stores cumulative orientation of birds within the ATTRACTION_RANGE
        float divisor = birds.Length;       // Custom divisor to support weighted averages
        repulsiveForce = new Vector3();     // Resetting the repulsive force      
        for (int i = 0; i < birds.Length; i++) {
            Vector3 birdPos = birds[i].transform.position;
            float dist = distance(birdPos, obj.transform.position);
            if (dist > ATTRACTION_RANGE || birds[i] == this.gameObject) {   // Excluding self and birds out of range 
                divisor--;
                continue;
            }
            deviations += ATTRACTION_FACTOR * (birdPos - obj.transform.position);
            orientations += (birds[i].GetComponent<BirdMovement>().dir);

            if (dist < REPULSION_RANGE) {
                repulsiveForce -= (obj.transform.position - birdPos);
            }
        }
        if (divisor == 0) { // If all birds are out of range
            meanOrientation = PositionToRotation(GetComponentInParent<Setup>().avgPos).eulerAngles; //Steer toward cumulative average position
            meanPosition = GetComponentInParent<Setup>().avgPos;    // Target position = average position
            return; // Skip further calculations
        }

        // Calculating means
        Vector3 meanDeviation = deviations / divisor;
        meanOrientation = orientations / divisor;
        meanPosition = meanDeviation + obj.transform.position;

        // Apply positional penalty for going out of bounds
        if (Mathf.Abs(obj.transform.position.y) > Y_BOUND || Mathf.Abs(obj.transform.position.x) > X_BOUND) {
            meanPosition /= 5f;
        }
    }

    Quaternion PositionToRotation(Vector3 target) { // Returns a Quaternion rotation toward a given Vector3 position. Applies all forces
        Debug.DrawLine(obj.transform.position, target, Color.red);
        dir = (obj.transform.position - target); // Direction toward target positon
        dir = dir + (meanOrientation-dir)*OREINTATION_WEIGHT; // Linear weighted interpolation of mean orientation 
        dir = dir + (repulsiveForce)*REPULSION_WEIGHT;  // Applying repulsive force onto final direction
        return Quaternion.LookRotation(Vector3.forward, dir); // Converts Vector3 direction into Quaternion rotation.
    }
}
