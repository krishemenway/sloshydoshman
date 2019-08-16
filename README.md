# www.SloshyDoshMan.com - Killing Floor 2 Stats Tracker

[![pipeline status](https://gitlab.krishemenway.com/krishemenway/sloshydoshman/badges/master/pipeline.svg)](https://gitlab.krishemenway.com/krishemenway/sloshydoshman/commits/master)
[![coverage report](https://gitlab.krishemenway.com/krishemenway/sloshydoshman/badges/master/coverage.svg)](https://gitlab.krishemenway.com/krishemenway/sloshydoshman/commits/master)

## Service

This service manages receiving game state updates from the client and updates the database to track all the games that are played on as many servers as you have clients. It hosts all the information required for the web project.

## Client

This project is a seperate client that connects to a KF2 server web admin and transmits status updates to the service.