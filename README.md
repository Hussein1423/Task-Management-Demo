# Task Management API

A backend learning project built with **ASP.NET Core, C#, PostgreSQL, Docker, automated testing, and Apache Kafka**.

The goal of this project is to learn backend development concepts by implementing them, testing them, and observing how different components work together.

## Project Structure

This solution contains three projects:

### 1. Task Management API

The main ASP.NET Core Web API responsible for managing tasks.

Concepts explored:
- REST API endpoints and controller-based request handling
- C# entities, services, and asynchronous operations
- PostgreSQL database integration
- Entity Framework Core and database queries
- Dependency injection and application configuration
- Docker and Docker Compose for running the API and database
- Kafka event production and messaging integration

### 2. Task Management API Tests

An automated testing project for the API controllers.

Concepts explored:
- Unit testing with xUnit
- Mocking dependencies with Moq
- Testing controller actions and different response scenarios
- Keeping tests independent of one another
- Improving confidence when changing existing code

### 3. Task_Events_Consumer

A separate .NET Worker Service that consumes events from Apache Kafka.

Concepts explored:
- Background services and the worker execution lifecycle
- Kafka producers, consumers, and topics
- Consumer groups and message consumption
- Consumer configuration and offset commits
- Communication between separate application components

## Technologies

- C# and .NET
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core
- Docker and Docker Compose
- Apache Kafka
- xUnit and Moq
- Git and GitHub

## Architecture Overview

The API handles incoming HTTP requests and manages task operations through its application logic and database integration.

When an event is published to Kafka, the separate consumer application can read and process that event independently of the API request-handling process.

The test project verifies selected controller behaviors without requiring every test to depend on a running application or pre-existing database records.

## Learning Goals

This project is helping me practice:

- Designing and implementing backend APIs
- Separating responsibilities between application components
- Working with relational databases and asynchronous operations
- Running applications and infrastructure with Docker
- Understanding asynchronous messaging and event-driven communication
- Writing automated tests and avoiding dependencies between tests
- Using Git to track changes through small, meaningful commits

## Project Status

This is an ongoing learning project. Features and implementation details may evolve as I learn more about backend development, testing, containers, and distributed systems.

## Running the Project

Setup instructions, environment variables, database configuration, Docker commands, and Kafka startup instructions will be documented as the project develops.

**Security note:** Never commit passwords, access tokens, or other secrets. Configure sensitive values through environment variables or local configuration files that are excluded from version control.
