// src/app/employee-list/employee-list.component.ts
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';  // Ensure RouterModule is imported for routerLink
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { EmployeeService, Employee } from '../services/employee.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,               // for routerLink usage
    MatToolbarModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule,
    MatSnackBarModule
  ]
})
export class EmployeeListComponent implements OnInit {
  // Added "id" to the displayed columns array
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'email', 'role', 'managerId', 'actions'];
  employees: Employee[] = [];

  constructor(
    private employeeService: EmployeeService,
    private snackBar: MatSnackBar,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: data => this.employees = data,
      error: () => {
        this.snackBar.open('Failed to load employees', 'Close', { duration: 3000 });
      }
    });
  }

  editEmployee(id: string): void {
    this.router.navigate(['/employee/edit', id]);
  }

  deleteEmployee(id: string): void {
    this.employeeService.deleteEmployee(id).subscribe({
      next: () => {
        this.snackBar.open('Employee deleted successfully', 'Close', { duration: 3000 });
        this.loadEmployees();
      },
      error: () => {
        this.snackBar.open('Failed to delete employee', 'Close', { duration: 3000 });
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
