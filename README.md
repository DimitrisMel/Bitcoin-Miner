# Bitcoin-Miner
Simulation of a distributed bitcoin miner in Akka on F#.

This was part of a project for the course Distributed Operating System Principles at the University of Florida.

# Objective
Create a software that simulates the operation of mining bitcoin. The software must be able to run on several cores concurrently.
The user is asked to provide the number of leading zeros of the result. The program generates strings and hashes them with SHA256.
Then, the hashed results are checked and the ones that have the required number of leading zeros are considered bitcoins.
The number of leading zeros simulate the varying difficulty of mining a cryptocoin.

# Example
By giving input "4", the user defines a bitcoin to be any string whose SHA256 hash has 4 leading zeros.
For example, the string dmelissourgos;05cVw3 is hashed to 00007A2AFF860430139252596049973CDB3310290B3AB5F49D59B56FA623BD44.
