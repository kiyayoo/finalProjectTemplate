Note on how to run simulation. Requires Unity 2018.3.14 to run. The reason for this is due to time constraints implementing a UI for changing missile values would have not been finished so editing missile values is done in the unity inspector in the editor. The scene to run the Simulation is JaysTest.unity 

Objects in scene:
Terrain - The Terrain is generated from height maps created from http://terrain.party/ and then read from unity script to generate elevation data on the Terrain. This is done by reading the pixel values to get an elevation value.
Cobra - This was a free model asset downloaded from the unity store so that we had something to visualy represent the model. Scripts were attached to the object and then a prefab was created from the model.
T95-1 - This was a free asset downloaded from the assetstore to represent a target. No scripts were added to this object and it only acts to give a position of the target. A collider was added to the target so we could detect if the missile was hit.

Cobra asset - The main script in this simulation is called Missile.cs and host the variables the control the flight path of the missile. The following values can be adjusted:
Target - This is what the Missile will guide towards by default.
Max Speed- this controls the velocity of the missile.
Max Pitch - this is what controls the turn speed of the missile.
Ray Cast Frontal Range - This controls how far in advance the missile can detect obsticles and react to them.
Max Elevation - This is the trigger that would determine if the height limit is exceded durring the simulation.


How it works.
The missile starts of at vector3 position 0,0,0 and uses quaternion functions provided by unity to get the rotation required by the missile to look at the target and then force is applied to the missile. The Missile constantly shoots 3 raycast one at 0 degrees straight infront of the missle one is rotated 10 degrees up and one is rotated 10 degrees down. If you look at the missile in the scene view these raycast are visible for debug purposes. If the frontal raycast hits a target the missile checks the 10 degree upward raycast to check if the path is clear if so it rotates to a temporary position to avoid hitting the target. Then the raycast checks again if this adjustment avoids the environmental obstical. If so after the obsticle is passed the missile resumes tracking the primary target. If not more adjustments are made. If the missile's pitch is not fast enough to avoid hitting the environment the simulation is complete and the missile is unable to reach its target.
