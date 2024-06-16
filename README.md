# Email Data Extraction Web API

This project is a .NET 8 Web API application designed to extract specific data from text blocks received via email, calculate sales tax, and provide the extracted and calculated data for retrieval. The project follows NTire Architecture and adheres to SOLID principles. Unit tests are implemented using xUnit.

## Table of Contents
- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Prerequisites](#prerequisites)
- [Installation](#installation)


## Features
- Accepts a block of text as input.
- Extracts XML content and marked-up fields.
- Calculates sales tax and total excluding tax based on the extracted total.
- Provides extracted and calculated data for retrieval.
- Handles failure conditions such as missing closing tags, total, and cost center.

## Architecture
This project uses an NTire Architecture divided into the following layers:
1. **Presentation Layer**: Contains the API controllers.
2. **Service Layer**: Contains interfaces and service implementations for business logic.
3. **Domain Layer**: Contains entities and domain services.
4. **Data Layer**: Contains data access implementations and repository patterns.

The project adheres to SOLID principles to ensure clean, maintainable, and testable code.

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio or Visual Studio Code
- Git

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/shyamjithcalicut/Flowingly.git
   cd Flowingly
