# 🔐 OpenIGA – Lightweight Identity Governance Platform

OpenIGA is a modern, developer-friendly Identity Governance and Administration (IGA) platform built using .NET and PostgreSQL.

It demonstrates core enterprise IAM/IGA capabilities such as identity lifecycle management, role-based access control (RBAC), and access request workflows.

---

## 🧠 Why this project?

Enterprise tools like One Identity Manager and SailPoint provide powerful identity governance solutions, but they are complex and heavy.

This project recreates the **core concepts of IGA** in a simplified, cloud-native, and developer-friendly way.

---

## 🚀 Key Features

### 🔑 Identity Lifecycle Management
- User onboarding (Joiner)
- Role updates (Mover)
- User deactivation (Leaver)

### 🧩 Role-Based Access Control (RBAC)
- Roles mapped to permissions
- Users assigned roles
- Effective permission resolution

### 📩 Access Request Workflow
- Users request access to roles
- Approval / rejection flow
- Automatic role assignment on approval

### 📊 Audit & Compliance
- Tracks access changes
- Logs approvals and actions
- Supports auditability

---

## 🏗 Architecture

- .NET 8 Web API
- PostgreSQL
- Clean architecture (Controllers → Services → Data)
- API-first design

---

## 🧠 Core Concepts Demonstrated

- Identity lifecycle (Joiner / Mover / Leaver)
- RBAC (Role → Permission → User)
- Access governance and approval workflows
- Audit logging for compliance

---

## 🛠 Tech Stack

- .NET 8 (C#)
- Entity Framework Core
- PostgreSQL
- REST APIs

---

## 🤖 AI-Assisted Development

This project was built using AI-assisted development (OpenAI Codex), guided by structured engineering rules defined in `AGENTS.md`.

The goal is to combine:
- Strong system design
- Clean architecture
- Efficient implementation

---

## 🎯 Purpose

This project is designed to:
- Demonstrate identity and security engineering skills
- Model real-world enterprise IAM systems
- Serve as a learning and reference platform for IGA concepts

---

## 📌 Next Steps

- [ ] Entra ID integration (Microsoft Graph API)
- [ ] Frontend (Angular) for access requests
- [ ] Role hierarchy support
- [ ] Policy-based access control (ABAC)

---

## 📖 Notes

See `/docs` for architecture and design details.