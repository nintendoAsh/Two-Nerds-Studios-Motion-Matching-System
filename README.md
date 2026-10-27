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
- [Credits and Assets](#credits-and-assets)
- [Related Projects](#related-projects)

## Disclamer
THIS IS NOT PRODUCTION READY!!! 
The system works but only under very spisifc condtions and breaks frequently. feel free to use and improve upon it into a production ready state but if you are expecting something that will work right out of the box you will be very disapointed.
Sorry

## Why Did I make this

I am an overscoping lil game dev and desided I needed the best looking animation for a little prototype of a game I was making. it very quickly spireled out of controll into the biggest and most complex programming project I have ever done so far. 

## What Is Motion Matching

Simply Motion Matching does what the name says. It matches the motion.
There are two main parts that make up motion matching.
1. The Dataset. Motion matching doesnt create new animation frames it simply uses what you give it. you must provide it with one or more animation clips demonstationing diffrent types of movement. useing those animations clips a program takes each frame finds a future trejectory as well as the bone postions relivive to the root bone and things like velocity and roation. All of the data that the program finds is stored in some sort of dataset (in our case a scriptable object) the matcher can look through
2. The Matcher. Useing the dataset that was created the matcher uses a perdicted trejectory based on player speed, current velocity and input as well as the players current limb postions, rotations and velocitys in game to search for the best frame to play.
With those two systems motion matching (should) deliver some of the best and most realistic animation posible

## What Scripts do what

The Two Nerds Studios Motion Matching System is made up of several parts and scripts knowing what they do will hopefuly help you to use it

Lock Root Motion - Locked postional root motion so the animated chaercter doesnt walk away from the root. but unlike disabling root motion in the animator this script still allows root rotation

Motion Matcher Dataset Debugger - Displays all of the data contained in the dataset scriptable object visualiy

Motion Matching - The heart of the system. this script takes in the perdicted future trejectory along with bone postions, roations, and velocities and matches to the frame with the lowest cost value (best frame)

Player Mixed Motion Matching - THIS SCRIPT NAME IS WRONG! ITS NOT THE MATCHER ITS THE TREJECOTRY GENERATOR

Motion Matching Dataset Editor - The Dataset creator. this script handles all of the UI and baking of the dataset in the dataset editor

Movement Provider - A custom class that has all the verables required for the trejectory genorator (PlayerMixedMotionMatching) to geneorate. 

Data Set - A scriptable Object that holds all of the frames, bone, animation clip and more outputted by the dataset editor

Rigidbody Movement Provider - A middle man script that inherits from MovementProvider that passes along data from the rigidbody to the Trejectory Generator (PlayerMixedMotionMatching)

## How to use the Two Nerds Studios Motion Matching System

We have uploaded a video on our youtube channel showing you how to set up and use the Two Nerds Studios Motion Matching System. it is a little bit scuffed but it works: https://youtu.be/EjWfd_qJaKc?si=9hmlMTV1CmTH9x1N

For you More Text based learners here is a basic step by step guide

1. Import the Two Nerds Studios Motion Matching System into unity

2. get a dance card or a locomotion animation clip and a charecter that the animation can play on. both must have and suport root motion

3. make sure the charecter and animation clip share the same avatar in unity (require for root motion)

4. open the dataset editor by going to the top menu bar of unity and looking for the dropdown labled Two Nerds Studios. hover your mouse over it and click Motion Matching Dataset Editor

5. in the window that opens up add the animation clips that you want to bake into the list labled Animation Clips.

6. in the same window under Future Trejectory Points add as many points in time you want the trejecory to be sampled as you want just make sure you remeber how many you added and the seconds value for each one

7. if you havent already add your 3D model into your scene.

8. on the Game Object with the 3D models animator make sure you have your 3D models avatar assinged in the box labled avatar and add the lock Root Motion script to the game object

9. on the main game object for your player/3D model add the Motion Matchin script and the Player Mixed Motion Matching Script (trejectory generator)

10. create a script. this will be your movement provider feel free to name it whatever you want in my case my movement is provided / is controlled by a rigidbody so I'm gonna named it RigidbodyMovementProvider

11. in the script replace the parent class (the : Monobehavoir at the top for your script) with MovementProvider so it should look somthing like public class ScriptName : MovementProvider

12. then you must assine values to PlayerPostion, PlayerVelocity, DesiredVelocity and then define UpdateValues() you dont have to define the verables just make sure you spell the names properly and assine the corect data to them. Most are self explaintory. Player Postion is the players current postion, player velocity is the players current speed/velocity, and Desired Velcotiy is the velocity to wish to reach (normaly done with Speed multiplyed by your input value). write the code for desired velocity in the update fucntion and define UpdateValues by typeing public void override UpdateValues() then put the code to set PlayerPostion and PlayerVelocity in there

13. in the end it should look simler to this:

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
14. return to unity and asigne the new movement provider to the same Game Object with the rest of the scripts and then fufill its required refrences 

15. assine the new Movement provider to the Trejecotry generator (PlayerMixedMotionMatching) 

16. asgine the dataset to the motion matching script. along with the Trejectory maker (PlayerMixedMotionMatching) and the animator of you 3D model. asgine the root and and scale game Obects (MAKE SURE YOU SELECT THE RIGHT ONES. Root is the root bone of your 3D model but for me I always have trouble becusae it keeps changing becuase my models are BAD. Scale is if your 3D model is scaled up or down diffrently from the way it was added to the scene (if the Model was scaled weirdly when added to the scene then the data was created with the scale so there is no need to rescale it but still add the refrence so the code doesnt compleatly freak out))

17. under the bones list asgine the game object for each bone

18. on the trejectory generator (PlayerMixedMotionMatching) under the Future Frames Ahead list (NOT THE POSIBLE FUTURE POSTIONS LIST) add the same data that you entered into the Dataset Future Trejectory points list.

19. weight all of your fetaures (the weights are not normaized so some data nateraly impacts the cost more then normal such as velocitys)

20. enjoy