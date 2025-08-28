// In src/app/core/services/auth.ts

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
// The class name AuthService is descriptive and fine, even if the file is auth.ts
export class AuthService {
  // The URL to your backend's auth controller
  private apiUrl = '/api/auth'; // The proxy will handle the rest
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';

  constructor(private http: HttpClient, private router: Router) { }

  // Method to handle user login
  login(credentials: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => this.setTokens(response.token, response.refreshToken))
    );
  }

  // Method to handle user registration
  register(userInfo: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userInfo);
  }

  // Method to log the user out
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    this.router.navigate(['/login']);
  }

  // --- Token Helper Methods ---

  // Store tokens in localStorage
  private setTokens(token: string, refreshToken: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
  }

  // Get the JWT from storage
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  // Check if the user is currently authenticated
  isAuthenticated(): boolean {
    return !!this.getToken();
  }
} 
