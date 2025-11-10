Trip Service – Ride-Hailing Platform

Description:
The Trip Service is a microservice responsible for managing the complete trip lifecycle in a ride-hailing platform — including trip creation, driver assignment, acceptance, completion, and fare calculation. It acts as one of the core components of the overall dispatch system.

Table of Contents

Overview

Features

API Endpoints

Prerequisites

Docker and Local Development

Kubernetes Deployment

Configuration

Manifests

Deployment Steps

Metrics and Monitoring

Troubleshooting

License

1. Overview

The Trip Service handles all critical workflows in a ride-hailing system:

A rider requests a trip.

The service automatically assigns an available driver (via the Driver Service).

The driver accepts the trip, and the trip status is updated to ACCEPTED.

Upon completion, the fare is calculated and payment is triggered.

Trips can be queried by ID to retrieve their details or current status.

The service is built with .NET Web API, uses PostgreSQL as the database, and is containerized using Docker for deployment on Kubernetes clusters.

2. Features

Create a new trip request (initial status: REQUESTED).

Automatically assign drivers from the Driver Service.

Accept trips and update their status to ACCEPTED.

Complete trips, calculate fares, and update status to COMPLETED.

Integrated Prometheus metrics for monitoring.

Fully compatible with Docker and Kubernetes environments.

3. API Endpoints
Endpoint	Method	Description
POST /api/v1/trips	Create a new trip	Request Body: { "riderId": int, "pickupZone": string, "dropZone": string, "baseFare": decimal, "distanceKm": decimal }
POST /api/v1/trips/{id}/accept	Accept a trip	Request Body: { "driverId": int }
POST /api/v1/trips/{id}/complete	Complete a trip and calculate fare	Request Body: { "distanceKm": decimal }
GET /api/v1/trips/{id}	Retrieve trip details	–

All API requests and responses use JSON format.

4. Prerequisites/Tech Stack

.NET 6, 7, or 8 SDK

Docker Desktop 

Kubernetes

Minikube (for local deployment and testing)

PostgreSQL database

Prometheus for monitoring

5. Docker and Local Development

Step 1 – Build the Docker image

docker build -t tripservice-app:latest .


Step 2 – Run locally (assuming PostgreSQL is running locally)

docker run -p 5000:8080 \
  -e ConnectionStrings__Default="Host=localhost;Database=tripdb;Username=tripuser;Password=tripsecret" \
  tripservice-app


Step 3 – Test the API

Use Postman or Curl to test:

http://localhost:5000/api/v1/trips

6. Kubernetes Deployment
Configuration

Configuration is handled using:

ConfigMap – for environment variables (e.g., ASPNETCORE_ENVIRONMENT).

Secret – for storing sensitive credentials such as the database password.

PersistentVolumeClaim (PVC) – for PostgreSQL data persistence.

Services Deployed:

tripservice (ClusterIP or NodePort)

tripservice-db (PostgreSQL)

Both the Trip Service and PostgreSQL are deployed as separate pods.

Manifests

All Kubernetes manifest files are located under the ./k8s/ directory:

tripservice-deployment.yml

tripservice-service.yml

trip-configmap.yml

trip-secret.yml

postgres-deployment.yml

postgres-service.yml

postgres-pvc.yml

Deployment Steps

Switch Docker context to the cluster
For Minikube:

On Windows Command Prompt
minikube start

minikube docker-env

Build the image inside the cluster

docker build -t tripservice-app:latest .


Apply all manifests

kubectl apply -f k8s/


Verify deployment status

kubectl get deployments
kubectl get pods
kubectl get svc


Access the service

If using NodePort or Ingress:

minikube service tripservice --url

Environment Variables (Deployment)
env:
  - name: ASPNETCORE_ENVIRONMENT
    value: "Production"
  - name: ConnectionStrings__Default
    value: "Host=tripservice-db;Database=tripdb;Username=tripuser;Password=tripsecret"


Note:

Local DB is named tripdb.

The PostgreSQL container in Docker/Kubernetes uses the service name tripservice-db.

The application image is named tripservice-app.

7. Metrics and Monitoring

Monitoring is integrated using prometheus-net.AspNetCore.
The /metrics endpoint is exposed by default for Prometheus to scrape.

Custom metrics include:

trip_created_total

trip_accepted_total

trip_completed_total

Example Prometheus scrape configuration:

scrape_configs:
  - job_name: 'tripservice'
    static_configs:
      - targets: ['tripservice.default.svc.cluster.local:8080']

8. Troubleshooting

Ensure Docker and Kubernetes contexts are correctly configured before deploying.

Check logs using:

kubectl logs <pod-name>


If connection issues occur, verify that:

The tripservice-db service is reachable.

Environment variables are correctly set.

Database credentials in Secret and connection string match.
