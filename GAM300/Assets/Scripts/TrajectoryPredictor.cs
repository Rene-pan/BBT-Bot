using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    #region Members
        [SerializeField] LineRenderer trajectoryLine;
        [SerializeField, Tooltip("The marker will show where the projectile will hit")]
        public Transform hitMarker;
        [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")]
        int maxPoints = 50;
        [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")]
        float increment = 0.025f;
        [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
        float rayOverlap = 1.1f;
        public CamController_v3 cam;
        #endregion

        private void Start()
        {
        cam = FindFirstObjectByType<CamController_v3>();
        if (trajectoryLine == null)
        {
            trajectoryLine = GetComponent<LineRenderer>();
            SetTrajectoryVisible(false);
        }
        }

        public void PredictTrajectory(ProjectileProperties projectile)
        {
        switch (cam.currentState) 
        {
            case CamController_v3.CamState.THIRDPERSON:
                SetTrajectoryVisible(false);
                break;
            case CamController_v3.CamState.OVERSHOULDER:
                SetTrajectoryVisible(true);
                Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);
                Vector3 position = projectile.initialPosition;
                Vector3 nextPosition;
                float overlap;

                UpdateLineRender(maxPoints, (0, position));

                for (int i = 1; i < maxPoints; i++)
                {
                    // Estimate velocity and update next predicted position
                    velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
                    nextPosition = position + velocity * increment;

                    // Overlap our rays by small margin to ensure we never miss a surface
                    overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

                    //When hitting a surface we want to show the surface marker and stop updating our line
                    if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
                    {
                        UpdateLineRender(i, (i - 1, hit.point));
                        print("line off");
                        //MoveHitMarker(hit);
                        break;
                    }

                    //If nothing is hit, continue rendering the arc without a visual marker
                    //hitMarker.gameObject.SetActive(false);
                    position = nextPosition;
                    UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
                }
                break;
        }
        }
        /// <summary>
        /// Allows us to set line count and an induvidual position at the same time
        /// </summary>
        /// <param name="count">Number of points in our line</param>
        /// <param name="pointPos">The position of an induvidual point</param>
        private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
        {
            trajectoryLine.positionCount = count;
            trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
        }

        private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
        {
            velocity += Physics.gravity * increment;
            velocity *= Mathf.Clamp01(1f - drag * increment);
            return velocity;
        }

        //private void MoveHitMarker(RaycastHit hit)
        //{
        //    hitMarker.gameObject.SetActive(true);

        //    // Offset marker from surface
        //    float offset = 0.025f;
        //    hitMarker.position = hit.point + hit.normal * offset;
        //    hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
        //}

        public void SetTrajectoryVisible(bool visible)
        {
        print("hide");
            trajectoryLine.enabled = visible;
            //hitMarker.gameObject.SetActive(visible);
        }
    }
