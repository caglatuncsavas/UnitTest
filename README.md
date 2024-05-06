# Users API - CRUD Operations

This repository contains the implementation and unit tests for CRUD operations on the Users API using ASP.NET Core. The API supports operations such as creating, reading, updating, and deleting user data.

## Unit Tests

Unit tests are organized under the `Users.API.Tests.Unit` project and cover the following functionalities:

### User Service Tests

- **CreateAsync**: Tests for successful user creation and error scenarios when user details are invalid or non-unique.
- **DeleteByIdAsync**: Tests for successful user deletion and error handling when a user is not found.
- **UpdateAsync**: Tests for successful user update and error scenarios when update details are invalid or non-unique.
- **GetAllAsync**: Tests for retrieving all users and handling scenarios when no users are found.

Each test ensures that the API responds correctly to various scenarios, and proper logging is performed. The test results show that all test cases have passed successfully.

![Unit Tests Results](https://github.com/caglatuncsavas/UnitTest/assets/95507765/6fe9a5de-8e4f-49f3-a6ee-4360d727d9b8)



## Conclusion

This API and unit tests ensure robust management of user data. By covering a range of scenarios, from successful operations to various error conditions, the API remains reliable and easy to maintain.
