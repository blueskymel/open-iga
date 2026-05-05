# API Design

## Users
POST /users
GET /users
PUT /users/{id}
DELETE /users/{id}

## Roles
POST /roles
GET /roles

## RBAC
POST /users/{id}/roles

## Access Requests
POST /access-requests
POST /access-requests/{id}/approve
POST /access-requests/{id}/reject

## Audit
GET /audit-logs
