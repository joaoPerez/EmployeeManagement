// src/app/app-routing.module.ts
import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { EmployeeListComponent } from './employee-list/employee-list.component';
import { EmployeeFormComponent } from './employee-form/employee-form.component';
import { AuthGuard } from './_guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'employees', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'employees', component: EmployeeListComponent, canActivate: [AuthGuard] },
  { path: 'employee/new', component: EmployeeFormComponent, canActivate: [AuthGuard] },
  { path: 'employee/edit/:id', component: EmployeeFormComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'employees' }
];
