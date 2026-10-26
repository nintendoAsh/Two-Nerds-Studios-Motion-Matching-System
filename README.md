# Two Nerds Studios Motion Matching System
## Created By Asher Randall

Hello and welcome to the Two Nerds Studios Motion Matching Github page. I'm Ash and I will be showing you around today. To get started click on a chapter:

## Table Of Content

[What Is Motion Matching](#what-is-motion-matching)

## What Is Motion Matching

Simply Motion Matching does what the name says. It matches the motion.
There are two main parts that make up motion matching.
1. The dataset. Motion matching doesnt create new animation frames it simply uses what you give it. you must provide it with one or more animation clips demonstationing diffrent types of movement. useing those animations clips a program takes each frame finds a future trejectory as well as the bone postions relivive to the root bone and things like velocity and roation. All of the data that the program finds is stored in some sort of dataset (in our case a scriptable object) the matcher can look through
2. 