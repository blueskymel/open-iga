# AGENTS.md

## Project Guidelines

## 🧠 AI-Assisted Development

This project uses AI-assisted development (OpenAI Codex) with defined engineering guidelines (see AGENTS.md) to ensure clean architecture, maintainability, and consistency.

### Architecture
- Use clean architecture principles (separation of concerns)
- Keep controllers thin and move logic into services
- Use dependency injection

### Coding Patterns
- Use a service layer for business logic
- Use repository pattern for data access (if needed)
- Keep models, DTOs, and entities separate

### API Design
- Follow RESTful conventions
- Use proper HTTP methods (GET, POST, PUT, DELETE)
- Return appropriate status codes (200, 201, 400, 404, etc.)

### Database
- Use PostgreSQL
- Use Entity Framework Core
- Keep migrations clean and consistent

### Security
- Prepare for JWT authentication (Entra ID integration later)
- Validate all inputs
- Avoid exposing sensitive data

### General
- Write clean, readable, maintainable code
- Use meaningful naming conventions
- Add comments where necessary