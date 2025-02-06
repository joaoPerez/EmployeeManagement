using System;
using System.Collections.Generic;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string DocNumber { get; private set; }
        public DateTime BirthDate { get; private set; }
        public List<string> PhoneNumbers { get; private set; }
        public Guid? ManagerId { get; private set; }
        public EmployeeRole Role { get; private set; }
        public string PasswordHash { get; private set; }

        public Employee? Manager { get; private set; }

        private Employee(string firstName, string lastName, string email, string docNumber, DateTime birthDate, List<string> phoneNumbers, Guid? managerId, EmployeeRole role)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DocNumber = docNumber;
            BirthDate = birthDate;
            PhoneNumbers = phoneNumbers;
            ManagerId = managerId;
            Role = role;
            PasswordHash = string.Empty;
        }

        public void SetPasswordHash(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        /// <summary>
        /// Factory method to create an employee with validations.
        /// </summary>
        public static Result<Employee> Create(
            string firstName,
            string lastName,
            string email,
            string docNumber,
            DateTime birthDate,
            List<string> phoneNumbers,
            Guid? managerId,
            EmployeeRole role)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(firstName))
                errors.Add("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                errors.Add("Last name is required.");
            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required.");
            if (string.IsNullOrWhiteSpace(docNumber))
                errors.Add("Document number is required.");
            if (phoneNumbers == null || phoneNumbers.Count == 0)
                errors.Add("At least one phone number is required.");

            // Validate age (must be at least 18)
            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;
            if (age < 18)
                errors.Add("Employee must be at least 18 years old.");

            if (errors.Count > 0)
                return Result<Employee>.Fail(errors);

            var employee = new Employee(firstName, lastName, email, docNumber, birthDate, phoneNumbers, managerId, role);
            return Result<Employee>.Success(employee);
        }
    }
}