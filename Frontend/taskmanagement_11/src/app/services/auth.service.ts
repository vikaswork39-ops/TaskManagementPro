
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = 'https://localhost:7158/api/auth';

  constructor(private http: HttpClient) {}

  // Login API
  login(data: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/login`,
      data
    );
  }

  // Register API
  register(data: any): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/register`,
      data
    );
  }

  // Save JWT Token
  saveToken(token: string): void {
    localStorage.setItem('token', token);
  }

  // Get JWT Token
  getToken(): string | null {
    return localStorage.getItem('token');
  }

  // Save Role from Login API
  saveRole(role: string): void {
    localStorage.setItem('role', role);
  }

  // Get Role
  getRole(): string | null {
    return localStorage.getItem('role');
  }

  // Logout
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
  }
}
