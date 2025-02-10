// src/app/employee-form/employee-form.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterModule } from '@angular/router';
import { EmployeeService, Employee } from '../services/employee.service';

@Component({
  selector: 'app-employee-form',
  templateUrl: './employee-form.component.html',
  styleUrls: ['./employee-form.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSnackBarModule,
    MatSelectModule,
    MatToolbarModule,
    RouterModule
  ]
})
export class EmployeeFormComponent implements OnInit {
  employeeForm!: FormGroup;
  isEditMode = false;
  employeeId: string | null = null;
  snackBar: any;

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.employeeForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      documentNumber: ['', Validators.required],
      phones: ['', Validators.required],
      role: ['', Validators.required],
      birthDate: ['', Validators.required],
      managerId: ['']
    });

    this.employeeId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.employeeId;

    if (this.isEditMode && this.employeeId) {
      this.employeeService.getEmployee(this.employeeId).subscribe({
        next: (employee) => {
          // Convert the phones array to a comma-separated string
          const patchedEmployee = { ...employee, phones: employee.phones.join(', ') };
          this.employeeForm.patchValue(patchedEmployee);
        },
        error: () => {
          console.error('Failed to fetch employee details.');
        }
      });
    }
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) return;
  
    const formValue = this.employeeForm.value;
  
    // Convert the phones field if it's not already an array.
    formValue.phones = Array.isArray(formValue.phones)
      ? formValue.phones
      : formValue.phones.split(',').map((p: string) => p.trim());
  
    // Convert managerId: if it's an empty string, set it to null.
    if (formValue.managerId === '') {
      formValue.managerId = null;
    }
  
    if (this.isEditMode) {
      formValue.id = this.employeeId;
      this.employeeService.updateEmployee(formValue).subscribe({
        next: () => {
          this.snackBar.open('Employee updated successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/employees']);
        },
        error: () => {
          this.snackBar.open('Failed to update employee', 'Close', { duration: 3000 });
        }
      });
    } else {
      this.employeeService.createEmployee(formValue).subscribe({
        next: () => {
          this.snackBar.open('Employee created successfully', 'Close', { duration: 3000 });
          this.router.navigate(['/employees']);
        },
        error: () => {
          this.snackBar.open('Failed to create employee', 'Close', { duration: 3000 });
        }
      });
    }
  }
}
