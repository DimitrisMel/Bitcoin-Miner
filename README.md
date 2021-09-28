# Bitcoin-Miner
Simulation of a distributed bitcoin miner in Akka on F#.

This was part of a project for the graduate course Distributed Operating System Principles at the University of Florida.

# Objective
Create a software that simulates the operation of mining bitcoin. The software must be able to run on several cores concurrently.
The user is asked to provide the number of leading zeros of the result. The program generates strings and hashes them with SHA256.
Then, the hashed results are checked and the ones that have the required number of leading zeros are considered bitcoins.
The number of leading zeros simulate the varying difficulty of mining a cryptocoin.
The requirement is to use the actor model of Akka on F#, in order to create multiple independent actors that send messages to each other.

# Example
By giving input "4", the user defines a bitcoin to be any string whose SHA256 hash has 4 leading zeros.
For example, the string dmelissourgos;05cVw3 is hashed to 00007A2AFF860430139252596049973CDB3310290B3AB5F49D59B56FA623BD44.

# Implementation
The program takes the input from the user and checks for correctness. It uses this input to check for possible bitcoins found.
A boss actor is created and in turn it spawns a number of other actors (defined by the constant numberOfWorkers), called workers.
The boss actor keeps an index, which it updates after assigning work to each of the workers. The index indicates the starting point for each worker.
The workers independently receive a message with the index from the boss and subsequently generate a sequence of strings.
Each index is translated to base62 arithmetic (10 numbers, 26 lowercase letters and 26 uppercase letters).
This is a string that gets hashed with SHA256 by the worker and tested for bitcoin. If it has the required amount of leading zeros,
it is sent to the boss actor, which prints it. The worker repeats this process for a given number of successive indexes (defined by the constant blockSize).
Each worker works independently of the other workers.
