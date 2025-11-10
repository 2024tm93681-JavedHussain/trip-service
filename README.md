🚖 Trip Service – Ride-Hailing Platform
📘 Overview

The Trip Service is a core microservice within a ride-hailing platform responsible for managing the entire trip lifecycle — from trip creation to completion and fare calculation. It integrates with other platform components such as the Driver Service and Payment Service, ensuring seamless end-to-end trip management.

Key Responsibilities

Handle trip requests from riders.

Automatically assign available drivers.

Manage trip acceptance and completion.

Calculate fares and trigger payments.

Provide trip status and details through APIs.

The service is built using .NET Web API, uses PostgreSQL as its database, and is containerized using Docker for deployment in Kubernetes clusters.

✨ Features

🆕 Create a new trip (initial status: REQUESTED)

🚗 Automatically assign drivers via Driver Service

✅ Accept trips and update status to ACCEPTED

🏁 Complete trips, calculate fares, and update status to COMPLETED

📈 Prometheus metrics integration for performance monitoring

🐳 Fully compatible with Docker and Kubernetes

🧩 API Endpoints
Endpoint	Method	Description	Request Body
/api/v1/trips	POST	Create a new trip	{ "riderId": int, "pickupZone": string, "dropZone": string, "baseFare": decimal, "distanceKm": decimal }
/api/v1/trips/{id}/accept	POST	Accept a trip	{ "driverId": int }
/api/v1/trips/{id}/complete	POST	Complete a trip and calculate fare	{ "distanceKm": decimal }
/api/v1/trips/{id}	GET	Retrieve trip details	–

All requests and responses use JSON format.

🧱 Prerequisites / Tech Stack

.NET 6, 7, or 8 SDK

Docker Desktop

Kubernetes

Minikube (for local cluster testing)

PostgreSQL

Prometheus (for metrics collection)

🐳 Docker and Local Development
Step 1 – Build the Docker Image
docker build -t tripservice-app:latest .

Step 2 – Run Locally

(Ensure PostgreSQL is running locally)

docker run -p 5000:8080 \
  -e ConnectionStrings__Default="Host=localhost;Database=tripdb;Username=tripuser;Password=tripsecret" \
  tripservice-app

Step 3 – Test the API

You can test using Postman or cURL:

http://localhost:5000/api/v1/trips

☸️ Kubernetes Deployment
Configuration

The service uses:

ConfigMap – Environment configuration (ASPNETCORE_ENVIRONMENT)

Secret – Secure storage for sensitive credentials (e.g., DB password)

PersistentVolumeClaim (PVC) – For PostgreSQL data persistence

Services Deployed

tripservice (ClusterIP or NodePort)

tripservice-db (PostgreSQL)

Both services run as separate Pods in the cluster.

📁 Manifests Directory

All Kubernetes YAML files are located under the ./k8s/ directory:

k8s/
├── tripservice-deployment.yml
├── tripservice-service.yml
├── trip-configmap.yml
├── trip-secret.yml
├── postgres-deployment.yml
├── postgres-service.yml
└── postgres-pvc.yml

🚀 Deployment Steps
1. Start Minikube
minikube start

2. Set Docker Context
minikube docker-env

3. Build Image Inside Cluster
docker build -t tripservice-app:latest .

4. Apply All Manifests
kubectl apply -f k8s/

5. Verify Deployment
kubectl get deployments
kubectl get pods
kubectl get svc

6. Access the Service

If using NodePort or Ingress:

minikube service tripservice --url

🌍 Environment Variables (Example)
env:
  - name: ASPNETCORE_ENVIRONMENT
    value: "Production"
  - name: ConnectionStrings__Default
    value: "Host=tripservice-db;Database=tripdb;Username=tripuser;Password=tripsecret"


Notes:

Local DB: tripdb

PostgreSQL service: tripservice-db

Docker/Kubernetes image: tripservice-app

📊 Metrics and Monitoring

The service exposes Prometheus metrics via the /metrics endpoint using the prometheus-net.AspNetCore package.

Custom Metrics
Metric Name	Description
trip_created_total	Total number of trips created
trip_accepted_total	Total number of trips accepted
trip_completed_total	Total number of trips completed
Example Prometheus Scrape Config
scrape_configs:
  - job_name: 'tripservice'
    static_configs:
      - targets: ['tripservice.default.svc.cluster.local:8080']

🧰 Troubleshooting

✅ Verify Docker and Kubernetes contexts before deployment.

🔍 Check logs for issues:

kubectl logs <pod-name>
