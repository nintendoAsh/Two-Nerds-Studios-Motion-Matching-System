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


