// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable, BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'http://localhost:5000/api/auth';
  private currentTokenSubject: BehaviorSubject<string | null>;
  public currentToken: Observable<string | null>;

  constructor(private http: HttpClient) {
    const token = localStorage.getItem('token');
    this.currentTokenSubject = new BehaviorSubject<string | null>(token);
    this.currentToken = this.currentTokenSubject.asObservable();
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/login`, { email, password })
      .pipe(tap(response => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.currentTokenSubject.next(response.token);
        }
      }));
  }

  logout() {
    localStorage.removeItem('token');
    this.currentTokenSubject.next(null);
  }

  get token(): string | null {
    return localStorage.getItem('token');
  }
}
