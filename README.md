# Two Nerds Studios Motion Matching System
## Created By Asher Randall

Hello and welcome to the Two Nerds Studios Motion Matching Github page. I'm Ash and I will be showing you around today. To get started click on a chapter:

## Table Of Content

- [Disclaimer](#disclaimer)
- [Why Did I Make This?](#why-did-i-make-this)
- [What Is Motion Matching?](#what-is-motion-matching)
- [What Scripts Do What?](#what-scripts-do-what)
- [How to Use the System](#how-to-use-the-two-nerds-studios-motion-matching-system)
- [Feature Weights and Settings](#feature-weights-and-settings)
- [Limitations and TODO](#limitations-and-todo)
- [Credits and Assets](#Links-Credits-Assets-and-special-thanks)

## Disclamer
THIS IS NOT PRODUCTION READY!!!
The system works but only under very specific conditions and breaks frequently. Feel free to use and improve upon it into a production-ready state, but if you are expecting something that will work right out of the box, you will be very disappointed.
Sorry

## Why Did I make this

I am an overscoping lil game dev and decided I needed the best-looking animation for a little prototype of a game I was making. It very quickly spiralled out of control into the biggest and most complex programming project I have ever done so far.

## What Is Motion Matching

Simply, Motion Matching does what the name says. It matches the motion.
There are two main parts that make up motion matching.

1. The Dataset. Motion matching doesn't create new animation frames; it simply uses what you give it. You must provide it with one or more animation clips demonstrating different types of movement. Using those animation clips, a program takes each frame, finds a future trajectory as well as the bone positions relative to the root bone and things like velocity and rotation. All of the data that the program finds is stored in some sort of dataset (in our case, a ScriptableObject) that the matcher can look through
2. The Matcher. Using the dataset that was created, the matcher uses a predicted trajectory based on player speed, current velocity and input, as well as the player's current limb positions, rotations and velocities in-game to search for the best frame to play.
With those two systems, motion matching (should) deliver some of the best and most realistic animation possible.

## What Scripts do what

The Two Nerds Studios Motion Matching System is made up of several parts and scripts; knowing what they do will hopefully help you to use it.

Lock Root Motion - Locks positional root motion so the animated character doesn't walk away from the root. But unlike disabling root motion in the Animator, this script still allows root rotation.

Motion Matcher Dataset Debugger - Displays all of the data contained in the dataset scriptable object visually.

Motion Matching - The heart of the system. This script takes in the predicted future trajectory along with bone positions, rotations, and velocities and matches to the frame with the lowest cost value (best frame)

Player Mixed Motion Matching - THIS SCRIPT NAME IS WRONG! IT'S NOT THE MATCHER; IT'S THE TRAJECTORY GENERATOR

Motion Matching Dataset Editor - The Dataset creator. This script handles all of the UI and baking of the dataset in the dataset editor.

Movement Provider - A custom class that has all the variables required for the trajectory generator (PlayerMixedMotionMatching) to generate.

Data Set - A Scriptable Object that holds all of the frames, bones, animation clips, and more outputted by the dataset editor

Rigidbody Movement Provider - A middleman script that inherits from MovementProvider that passes along data from the rigidbody to the Trajectory Generator (PlayerMixedMotionMatching)

## How to use the Two Nerds Studios Motion Matching System

We have uploaded a video on our YouTube channel showing you how to set up and use the Two Nerds Studios Motion Matching System. It is a little bit scuffed, but it works: https://youtu.be/EjWfd_qJaKc?si=9hmlMTV1CmTH9x1N

For you more text-based learners, here is a basic step-by-step guide.

1. Import the Two Nerds Studios Motion Matching System into Unity

2. Get a dance card or a locomotion animation clip and a character that the animation can play on. Both must have and support root motion.

3. Make sure the character and animation clip share the same avatar in Unity (required for root motion)

4. Open the dataset editor by going to the top menu bar of Unity and looking for the dropdown labelled Two Nerds Studios. Hover your mouse over it and click Motion Matching Dataset Editor.

5. In the window that opens up, add the animation clips that you want to bake into the list labelled Animation Clips.

6. In the same window, under Future Trajectory Points, add as many points in time as you want the trajectory to be sampled. Just make sure you remember how many you added and the seconds value for each one.

7. If you haven't already, add your 3D model into your scene.

8. On the Game Object with the 3D model's Animator, make sure you have your 3D model's avatar assigned in the box labelled Avatar, and add the Lock Root Motion script to the game object.

9. On the main game object for your player/3D model, add the Motion Matching script and the Player Mixed Motion Matching Script (trajectory generator)

10. Create a script. This will be your movement provider. Feel free to name it whatever you want; in my case, my movement is provided/is controlled by a Rigidbody, so I’m gonna name it RigidbodyMovementProvider.

11. In the script, replace the parent class (the : MonoBehaviour at the top of your script) with MovementProvider, so it should look something like public class ScriptName : MovementProvider.

12. Then you must assign values to PlayerPosition, PlayerVelocity, and DesiredVelocity and then define UpdateValues(). You don't have to define the variables; just make sure you spell the names properly and assign the correct data to them. Most are self-explanatory. Player Postion is the players current postion, player velocity is the players current speed/velocity, and Desired Velcotiy is the velocity to wish to reach (normaly done with Speed multiplyed by your input value). Write the code for desired velocity in the Update function, and define UpdateValues by typing public override void UpdateValues(), then put the code to set PlayerPosition and PlayerVelocity in there.

13. In the end, it should look similar to this:

```csharp 
using UnityEngine;

public class RigidbodyMovementProvider : MovementProvider
{
    public Rigidbody rigidbody;
    public Movement movement;
    void Update()
    {
        DesiredVelocity = new Vector3(movement.speed * movement.MoveX, 0f, movement.speed * movement.MoveZ);
    }

    public override void UpdateValues()
    {
        PlayerPostion = rigidbody.position;
        PlayerVelocity = rigidbody.linearVelocity;
    }
}
```
14. Return to Unity and assign the new movement provider to the same Game Object with the rest of the scripts, and then fulfill its required references.

15. Assign the new Movement provider to the Trajectory generator (PlayerMixedMotionMatching)

16. Assign the dataset to the motion matching script, along with the trajectory maker (PlayerMixedMotionMatching) and the animator of your 3D model. Assign the root and scale game objects (MAKE SURE YOU SELECT THE RIGHT ONES. Root is the root bone of your 3D model, but for me I always have trouble because it keeps changing because my models are BAD. Scale is if your 3D model is scaled up or down differently from the way it was added to the scene (if the Model was scaled weirdly when added to the scene, then the data was created with the scale, so there is no need to rescale it, but still add the reference so the code doesn't completely freak out))

17. Under the bones list, assign the game object for each bone.

18. On the trajectory generator (PlayerMixedMotionMatching), under the Future Frames Ahead list (NOT THE POSSIBLE FUTURE POSITIONS LIST), add the same data that you entered into the Dataset Future Trajectory points list.

19. Weight all of your features (the weights are not normalized, so some data naturally impacts the cost more than normal, such as velocities; this makes it a bit of a pain to tune the system into working)

20. enjoy

## Feature weights and Settings

### Motion Matching Script

Time Between Searches - minimum time elapsed between each search for a new match.

Future trajectory weight - how much the future trajectory influences the cost value (depending on the amount of position features you are matching, this will naturally have stronger influence)

Rotation Weight - how much the current root rotation influences the cost value

Bone Position Weight - how much the current bone position influences the cost value

Bone Rotation Weight - how much the current bone rotation influences the cost value

Bone Velocity Weight - how much the current bone velocity influences the cost value

Far Frame - How far away in the dataset from the last match does a frame have to be to be matched to

Velocity Multiplier - multiplies the velocity that the matcher sees from the runtime. (was for development; unneeded now)

Change Threshold - How high does the current frame's cost have to be to match? If the current frame is lower than that cost, keep playing the current animation. (is disabled in the package)

Clip Change Penalty - if the matcher is searching for a frame and the current frame it's looking at is not from the same animation clip that is currently playing, add this value to the current cost to discourage it from changing clips.

### Trajectory Generator (PlayerMixedMotionMatching)

Future Frames Ahead - works the same as Future Trajectory Points in the dataset editor. Add the points in times you want to sample/predict  the future trajectory. (MUST BE SET THE SAME AS THE FUTURE TRAJECTORY POINT YOU SET IN THE CURRENT DATASET DURING BAKING)

Trajectory Half-life - Control how fast the predicted trajectory's movement converges into the desired velocity (It's just fancy… VERY FANCY blending). A higher half-life will cause it to feel less responsive but more weighty and sometimes more realistic, but a lower half-life will make it snappy and responsive, with the trade-off of being lighter and less realistic.

Responsiveness - set this to higher than your movement speed. It was to add blending to your desired velocity but I quickly realized its actually not doing that and screwing everything up. So set it to like a stupid high number and leave it. It will be gone in the next update.

## Limitations and TODO

This is a very basic implementation of motion matching; because of that, it has some limitations, and it is my first kind of project of this variety; because of that, it also has some things THAT NEED MAJOR WORK.

Todo:

* Add Blending. The matcher doesn't blend between its current frame and its next chosen frame, causing snapping and glitching.
* Remake the Baker. The baker needs rework in how it handles bones, and it can be cleaned up and optimized.
* Fix minor memory leaks.
* Add the ability to add tags.
* Rename the PlayerMixedMotionMatching script to the TrajectoryGenerator.
* Remove the responsiveness slider and other unneeded functions.
* Rework the Cost Threshold.
* Add weight normalization 

Limitations:

* IT WILL STRUGGLE WITH LARGE DATASETS!
* BAKING LARGE DATASETS WILL TAKE A WHILE
* Lots of the things mentioned in TODO don't exist or need fixing, which may be normal or standard on most other motion matching packages.
* Was made by one person in 11 months with no clue what they were doing

## Links, Credits, Assets, and Special Thanks

Thanks A MILLION TO Daniel Holden! For all your work with spring dampers, as well as replying to my EMAIL when I was stuck and confused.
Check out his website here: https://theorangeduck.com/page/projects
Check out the Page where I got my spring damper math from: https://theorangeduck.com/page/spring-roll-call#controllers

Thanks to the University of Saskatchewan, the Shoulder Health and Ergonomic Lab, and all of their team for giving me a tour of their ten-camera VICON setup as well as demoing it on me. I will never forget that for as long as I live. THANK YOU FROM THE BOTTOM OF MY HEART!
Check out their website here: https://research-groups.usask.ca/ergolab/index.php

If you want to play a demo of the Two Nerds Studios Motion Matching System without setting it up in a Unity project, feel free to try it out on Itch.io: https://two-nerds-studios.itch.io/motion-matching-demo
The Repo for the same Demo: https://github.com/nintendoAsh/Two-Nerds-Studios-Motion-Matching-System-DEMO

The 3D Model used in the Demo and Contained within the package: https://free3d.com/3d-model/rigged-male-human-442626.html?dd_referrer=https%3A%2F%2Fwww.google.com%2F

Not used here, but in the demo, thank you to Graphy:  https://github.com/Tayx94/graphy

If you made it this far, I would like to thank you too. It took me forever to make this, and I am very happy to get it out into the world. Your views, comments and feedback mean the world to me, and I would love to hear what you think. I couldn’t have done it without you. Now that you have read through this whole repo, LET'S GO MATCH SOME MOTION!!!

If you have any issues with this software, add it to the issues tab on GitHub!



Once again, thanks for going all the way to the bottom. I would like to now share my favourite quote so far:

“Find what you are willing to die for and live for it”

“The two most important days of your life are the day you were born, and the day you find out why”