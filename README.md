# Two Nerds Studios Motion Matching System
## Created By Asher Randall

Hello and welcome to the Two Nerds Studios Motion Matching Github page. I'm Ash and I will be showing you around today. To get started click on a chapter:

## Table Of Content

[What Is Motion Matching](#what-is-motion-matching)

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

## How The Two Nerds Studios Motion Matching System Works

The Two Nerds Studios Motion Matching System is made up of several parts and scripts knowing what they do will hopefuly help you to use it

Lock Root Motion - Locked postional root motion so the animated chaercter doesnt walk away from the root. but unlike disabling root motion in the animator this script still allows root rotation

Motion Matcher Dataset Debugger - Displays all of the data contained in the dataset scriptable object visualiy

Motion Matching - The heart of the system. this script takes in the perdicted future trejectory along with bone postions, roations, and velocities and matches to the frame with the lowest cost value (best frame)

Player Mixed Motion Matching - THIS SCRIPT NAME IS WRONG! ITS NOT THE MATCHER ITS THE TREJECOTRY GENERATOR

Motion Matching Dataset Editor - The Dataset creator. this script handles all of the UI and baking of the dataset in the dataset editor

Movement Provider - A custom class that has all the verables required for the trejectory genorator (PlayerMixedMotionMatching) to geneorate. 

Data Set - A scriptable Object that holds all of the frames, bone, animation clip and more outputted by the dataset editor

Rigidbody Movement Provider - A middle man script that inherits from MovementProvider that passes along data from the rigidbody to the Trejectory Generator (PlayerMixedMotionMatching)