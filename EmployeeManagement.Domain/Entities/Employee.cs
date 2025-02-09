using EmployeeManagement.Domain;

namespace EmployeeManagement.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string DocumentNumber { get; private set; } = string.Empty;
        public List<string> Phones { get; private set; } = [];
        public Role Role { get; private set; }
        public string PasswordHash { get; private set; } = string.Empty;
        public DateTime BirthDate { get; private set; }
        public Guid? ManagerId { get; private set; }
        public Employee? Manager { get; private set; }

        private Employee() { }

        private Employee(
            string firstName,
            string lastName,
            string email,
            string documentNumber,
            List<string> phones,
            Role role,
            string passwordHash,
            DateTime birthDate,
            Guid? managerId)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DocumentNumber = documentNumber;
            Phones = phones;
            Role = role;
            PasswordHash = passwordHash;
            BirthDate = birthDate;
            ManagerId = managerId;
        }

        public static Result<Employee> Create(
            string firstName,
            string lastName,
            string email,
            string documentNumber,
            List<string> phones,
            Role role,
            string passwordHash,
            DateTime birthDate,
            Guid? managerId)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add("First name is required.");
                firstName = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add("Last name is required.");
                lastName = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("Email is required.");
                email = string.Empty;
            }
            if (string.IsNullOrWhiteSpace(documentNumber))
            {
                errors.Add("Document number is required.");
                documentNumber = string.Empty;
            }
            if (phones == null || phones.Count == 0)
            {
                errors.Add("At least one phone number is required.");
                phones = [];
            }

            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age))
                age--;
            if (age < 18)
                errors.Add("Employee must be at least 18 years old.");

            if (errors.Any())
                return Result<Employee>.Failure(errors);

            var employee = new Employee(firstName, lastName, email, documentNumber, phones, role, passwordHash, birthDate, managerId);
            return Result<Employee>.Success(employee);
        }

        public void SetPasswordHash(string hash)
        {
            PasswordHash = hash;
        }

        public void UpdatePersonalInfo(
            string firstName,
            string lastName,
            string email,
            List<string> phones,
            DateTime birthDate,
            Guid? managerId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phones = phones;
            BirthDate = birthDate;
            ManagerId = managerId;
        }

        public void UpdateRole(Role role)
        {
            Role = role;
        }
    }
}