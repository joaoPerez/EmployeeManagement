// src/app/services/employee.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Employee {
  id?: string;
  firstName: string;
  lastName: string;
  email: string;
  documentNumber: string;
  phones: string[];
  role: string;
  birthDate: Date;
  managerId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private baseUrl = 'http://localhost:5000/api/employees';

  constructor(private http: HttpClient) { }

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.baseUrl);
  }

  getEmployee(id: string): Observable<Employee> {
    return this.http.get<Employee>(`${this.baseUrl}/${id}`);
  }

  createEmployee(employee: Employee): Observable<Employee> {
    return this.http.post<Employee>(this.baseUrl, employee);
  }

  updateEmployee(employee: Employee): Observable<Employee> {
    return this.http.put<Employee>(`${this.baseUrl}/${employee.id}`, employee);
  }

  deleteEmployee(id: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${id}`);
  }
}
